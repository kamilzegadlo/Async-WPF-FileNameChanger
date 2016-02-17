using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileNameChanger.Service
{
    public interface IFileNameChangerService
    {
        Task<ICollection<IMapEntry>> ChangeNamesAsync(string inputFolder, bool inputRekursive, string outputFolder, bool restoreFoldersTree, bool countingFromTheBeginning, bool overwriteFiles,
           string inputMask, string outputMask, string startNumber, string numberOfDigits, bool updatePBOnly, IProgress<IUpdateInfo> progress, CancellationToken cancellationToken);

        Task<ICollection<IMapEntry>> ChangeNamesAsync(string inputFolder, bool inputRekursive, string outputFolder, bool restoreFoldersTree, bool countingFromTheBeginning, bool overwriteFiles,
            string inputMask, string outputMask, string startNumber, string numberOfDigits, bool updatePBOnly);

        ICollection<IMapEntry> ChangeNames(string inputFolder, bool inputRekursive, string outputFolder, bool restoreFoldersTree, bool countingFromTheBeginning, bool overwriteFiles,
            string inputMask, string outputMask, string startNumber, string numberOfDigits, bool updatePBOnly=false);

        Task<int> GetNumberOfFilesAsync(string inputFolder, bool inputRekursive, string inputMask);

        int GetNumberOfFiles(string inputFolder, bool inputRekursive, string inputMask);


    }
}
