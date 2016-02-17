using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;

namespace FileNameChanger.Service
{
    public class FileNameChangerService : IFileNameChangerService
    {
        private IProgress<IUpdateInfo> _progress;
        private CancellationToken _cancellationToken;
        private IUpdateInfo _updateInfo;

        public Task<ICollection<IMapEntry>> ChangeNamesAsync(string inputFolder, bool inputRekursive, string outputFolder, bool restoreFoldersTree, bool countingFromTheBeginning, bool overwriteFiles,
            string inputMask, string outputMask, string startNumber, string numberOfDigits, bool updatePBOnly, IProgress<IUpdateInfo> progress, CancellationToken cancellationToken)
        {
            return Task<ICollection<IMapEntry>>.Factory.StartNew (() =>
            {
                _progress = progress;
                _cancellationToken = cancellationToken;

                return ChangeNames(inputFolder, inputRekursive, outputFolder, restoreFoldersTree, countingFromTheBeginning, overwriteFiles,
                    inputMask, outputMask, startNumber, numberOfDigits, updatePBOnly);
            });
        }

        public Task<ICollection<IMapEntry>> ChangeNamesAsync(string inputFolder, bool inputRekursive, string outputFolder, bool restoreFoldersTree, bool countingFromTheBeginning, bool overwriteFiles,
            string inputMask, string outputMask, string startNumber, string numberOfDigits, bool updatePBOnly)
        {
            return Task<ICollection<IMapEntry>>.Factory.StartNew (() =>
                {
                    return ChangeNames(inputFolder, inputRekursive, outputFolder, restoreFoldersTree, countingFromTheBeginning, overwriteFiles,
                        inputMask, outputMask, startNumber, numberOfDigits, updatePBOnly);
                }
            );
        }

        public ICollection<IMapEntry> ChangeNames(string inputFolder, bool inputRekursive, string outputFolder, bool restoreFoldersTree, bool countingFromTheBeginning, bool overwriteFiles,
            string inputMask, string outputMask, string startNumber, string numberOfDigits, bool updatePBOnly=false)
        {
            int currentNumber = 1;

            CheckInputValues(inputFolder, inputRekursive, ref outputFolder, restoreFoldersTree, countingFromTheBeginning, inputMask, outputMask, startNumber, numberOfDigits, ref currentNumber);

            HashSet<IMapEntry> map = new HashSet<IMapEntry>();

            CopyFiles(new DirectoryInfo(inputFolder), inputMask, outputFolder, ref currentNumber, outputMask, numberOfDigits, inputRekursive, overwriteFiles, restoreFoldersTree, inputFolder, currentNumber, countingFromTheBeginning, map, updatePBOnly);

            if (_progress != null && _updateInfo != null)
                _progress.Report(_updateInfo);

            return map;
        }

        public Task<int> GetNumberOfFilesAsync(string inputFolder, bool inputRekursive, string inputMask)
        {
            return Task.Run(() =>
            {
                return GetNumberOfFiles(inputFolder, inputRekursive, inputMask);
            });
        }

        public int GetNumberOfFiles(string inputFolder, bool inputRekursive, string inputMask)
        {
            int count = 0;
            DirectoryInfo di = new DirectoryInfo(inputFolder);
            GetNumberOfFilesInFolder(di, inputRekursive, inputMask, ref count);
            return count;
        }

        private void CopyFiles(DirectoryInfo di, string inputMask, string outputFolder, ref int currentNumber, string outputMask, string numberOfDigits, bool inputRekursive, bool overwriteFiles, bool restoreFoldersTree, string inputFolder, int startNumber, bool countingFromTheBeginning, HashSet<IMapEntry> map, bool updatePBOnly)
        {
            int currentNumberTemp = currentNumber;

            CopyFilesFromFolder(di, inputMask, outputFolder, ref currentNumberTemp, outputMask, numberOfDigits, overwriteFiles, restoreFoldersTree, inputFolder, startNumber, countingFromTheBeginning, map, updatePBOnly);

            if (inputRekursive)
                di.GetDirectories().OrderBy(d => d.Name).ToList().ForEach(d => CopyFiles(d, inputMask, outputFolder, ref currentNumberTemp, outputMask, numberOfDigits, inputRekursive, overwriteFiles, restoreFoldersTree, inputFolder, startNumber, countingFromTheBeginning, map, updatePBOnly));

            currentNumber = currentNumberTemp;
        }

        private void CopyFilesFromFolder(DirectoryInfo di, string inputMask, string outputFolder, ref int currentNumber, string outputMask, string numberOfDigits, bool overwriteFiles, bool restoreFoldersTree, string inputFolder, int startNumber, bool countingFromTheBeginning, HashSet<IMapEntry> map, bool updatePBOnly)
        {
            if (restoreFoldersTree)
            {
                outputFolder += di.FullName.Substring(inputFolder.Length);
                if (!outputFolder.EndsWith(@"\"))
                    outputFolder += @"\";

                if (!Directory.Exists(outputFolder))
                    Directory.CreateDirectory(outputFolder);
            }
            if (countingFromTheBeginning)
                currentNumber = startNumber;

            int currentNumberTemp = currentNumber;

            di.GetFiles(inputMask).OrderBy(f => f.Name).ToList().ForEach(f => CopyTo(f, outputFolder, ref currentNumberTemp, outputMask, numberOfDigits, overwriteFiles, map, updatePBOnly));

            currentNumber = currentNumberTemp;
        }

        private string GetNewName(ref int currentNumber, string outputMask, string numberOfDigits)
        {
            if (currentNumber>=Math.Pow(10,Int32.Parse(numberOfDigits)))
                throw new OverflowException("Overflow specified number of digits.");
             
            return outputMask.Replace("[NUMBER]", currentNumber++.ToString("D" + numberOfDigits));
        }

        private void CopyTo(FileInfo f, string outputFolder, ref int currentNumber, string outputMask, string numberOfDigits, bool overwriteFiles, HashSet<IMapEntry> map, bool updatePBOnly)
        {
            if (_cancellationToken != null && !_cancellationToken.IsCancellationRequested)
            {
                int currentNumberTemp = currentNumber;

                string newName = outputFolder + GetNewName(ref currentNumberTemp, outputMask, numberOfDigits);

                f.CopyTo(newName, overwriteFiles);
                map.Add(new MapEntry(new DirectoryInfo(f.FullName), new DirectoryInfo(newName)));

                currentNumber = currentNumberTemp;

                if (_progress != null)
                {
                    if(_updateInfo==null)
                        _updateInfo=new UpdateInfo();
                   
                    _updateInfo.AddMessage(String.Format("File: {0} was copied as: {1}", f.FullName, newName));

                    if (_updateInfo.UpdateTime.AddMilliseconds(200)<System.DateTime.Now)
                    {
                        _progress.Report((IUpdateInfo)_updateInfo.Clone());
                        if (updatePBOnly)
                            _updateInfo.ResetCounter();
                        else
                            _updateInfo = null;
                    }
                }
            }
        }

        private void CheckInputValues(string inputFolder, bool inputRekursive, ref string outputFolder, bool restoreFoldersTree, bool countingFromTheBeginning,
            string inputMask, string outputMask, string startNumber, string numberOfDigits, ref int currentNumber)
        {
            if (!outputFolder.EndsWith(@"\"))
                outputFolder += @"\";

            if (!Int32.TryParse(startNumber, System.Globalization.NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out currentNumber) || currentNumber<0)
                throw new InvalidInputValueException("Invalid input value: start number. Has to be an integer > 0");

            int temp = 0;

            if (!Int32.TryParse(numberOfDigits, System.Globalization.NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out temp) || temp<1)
                throw new InvalidInputValueException("Invalid input value: number of digits. Has to be an integer > 0");

            if (Regex.Matches(outputMask, Regex.Escape("[NUMBER]")).Count < 1)
                throw new InvalidInputValueException("Output mask has to contain [NUMBER]");

            if (Regex.IsMatch(outputMask, "[" + Regex.Escape(new string(Path.GetInvalidFileNameChars())) + "]"))
                throw new InvalidInputValueException(String.Format("Output mask contains invalid char (invalid chars: {0} ))", new string(Path.GetInvalidFileNameChars())));

            if(inputMask.Length==0)
                throw new InvalidInputValueException("Input mask cannot be empty. If you want all files type a star(*)");

            if (outputFolder.Length<2)
                throw new InvalidInputValueException("Output folder not specified.");

            if (Regex.IsMatch(outputFolder, "[" + Regex.Escape(new string(Path.GetInvalidPathChars())) + "]"))
                throw new InvalidInputValueException("Output folder contains invalid chars.");

            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);

            if (!Directory.Exists(inputFolder))
                throw new InvalidInputValueException("Input folder does not exists.");

            if (inputFolder.Length < 2)
                throw new InvalidInputValueException("Input folder not specified.");

            if (Regex.IsMatch(inputFolder, "[" + Regex.Escape(new string(Path.GetInvalidPathChars())) + "]"))
                throw new InvalidInputValueException("Input folder contains invalid chars.");
        }

        private void GetNumberOfFilesInFolder(DirectoryInfo inputFolder, bool inputRekursive, string inputMask, ref int count)
        {
            count += inputFolder.GetFiles(inputMask).Count();

            if (inputRekursive)
            {
                int countTemp = count;
                inputFolder.GetDirectories().ToList().ForEach(d => GetNumberOfFilesInFolder(d, inputRekursive, inputMask, ref countTemp));
                count = countTemp;
            }
        }

    }
}
