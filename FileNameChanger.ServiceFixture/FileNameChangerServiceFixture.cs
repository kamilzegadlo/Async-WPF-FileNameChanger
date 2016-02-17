using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit;
using NUnit.Framework;
using FileNameChanger.Service;
using System.IO;
using System.Reflection;

namespace FileNameChanger.ServiceFixture
{
    [TestFixture]
    public class FileNameChangerServiceFixture
    {
        IFileNameChangerService _fileNameChangerService;

        [SetUp]
        public void SetUp()
        {
            _fileNameChangerService = new FileNameChangerService();
        }

        [Test]
        [ExpectedException(typeof(InvalidInputValueException), ExpectedMessage="start number",MatchType=MessageMatch.Contains)]
        public void WrongStartNumberEmptyValue()
        {
            _fileNameChangerService.ChangeNames(@"C:\RBS\INPUT", false, @"C:\RBS\OUTPUT", false,
                    false, false, @"*.txt", @"RB[NUMBER].txt", "", "6");
        }

        [Test]
        [ExpectedException(typeof(InvalidInputValueException), ExpectedMessage = "start number", MatchType = MessageMatch.Contains)]
        public void WrongStartNumberWrongChar()
        {
            _fileNameChangerService.ChangeNames(@"C:\RBS\INPUT", false, @"C:\RBS\OUTPUT", false,
                    false, false, @"*.txt", @"RB[NUMBER].txt", "a", "6");
        }

        [Test]
        [ExpectedException(typeof(InvalidInputValueException), ExpectedMessage = "start number", MatchType = MessageMatch.Contains)]
        public void WrongStartNumberNegativeValue()
        {
            _fileNameChangerService.ChangeNames(@"C:\RBS\INPUT", false, @"C:\RBS\OUTPUT", false,
                    false, false, @"*.txt", @"RB[NUMBER].txt", "-1", "6");
        }

        [Test]
        [ExpectedException(typeof(InvalidInputValueException), ExpectedMessage = "number of digits", MatchType = MessageMatch.Contains)]
        public void WrongNumberOfDigitsEmptyValue()
        {
            _fileNameChangerService.ChangeNames(@"C:\RBS\INPUT", false, @"C:\RBS\OUTPUT", false,
                    false, false, @"*.txt", @"RB[NUMBER].txt", "1", "");
        }

        [Test]
        [ExpectedException(typeof(InvalidInputValueException), ExpectedMessage = "number of digits", MatchType = MessageMatch.Contains)]
        public void WrongNumberOfDigitsWrongChar()
        {
            _fileNameChangerService.ChangeNames(@"C:\RBS\INPUT", false, @"C:\RBS\OUTPUT", false,
                    false, false, @"*.txt", @"RB[NUMBER].txt", "1", "a");
        }

        [Test]
        [ExpectedException(typeof(InvalidInputValueException), ExpectedMessage = "number of digits", MatchType = MessageMatch.Contains)]
        public void WrongNumberOfDigitsNegativeValue()
        {
            _fileNameChangerService.ChangeNames(@"C:\RBS\INPUT", false, @"C:\RBS\OUTPUT", false,
                    false, false, @"*.txt", @"RB[NUMBER].txt", "1", "-1");
        }

        [Test]
        [ExpectedException(typeof(InvalidInputValueException), ExpectedMessage = "Output mask", MatchType = MessageMatch.Contains)]
        public void WrongMaskOfTheOutputFileNoKeyWord()
        {
            _fileNameChangerService.ChangeNames(@"C:\RBS\INPUT", false, @"C:\RBS\OUTPUT", false,
                    false, false, @"*.txt", @"RBS.txt", "1", "6");
        }

        [Test]
        [ExpectedException(typeof(InvalidInputValueException), ExpectedMessage = "Output mask", MatchType = MessageMatch.Contains)]
        public void WrongMaskOfTheOutputFileInvalidChar()
        {
            _fileNameChangerService.ChangeNames(@"C:\RBS\INPUT", false, @"C:\RBS\OUTPUT", false,
                    false, false, @"*.txt", @"RB[NUMBER]\.txt", "1", "6");
        }

        [Test]
        [ExpectedException(typeof(InvalidInputValueException), ExpectedMessage = "Input mask", MatchType = MessageMatch.Contains)]
        public void WrongMaskOfTheInputFileEmpty()
        {
            _fileNameChangerService.ChangeNames(@"C:\RBS\INPUT", false, @"C:\RBS\OUTPUT", false,
                    false, false, @"", @"RB[NUMBER].txt", "1", "6");
        }

        [Test]
        [ExpectedException(typeof(InvalidInputValueException), ExpectedMessage = "Output folder", MatchType = MessageMatch.Contains)]
        public void WrongOutputFolderEmpty()
        {
            _fileNameChangerService.ChangeNames(@"C:\RBS\INPUT", false, @"", false,
                    false, false, @"*.txt", @"RB[NUMBER].txt", "1", "6");
        }

        [Test]
        [ExpectedException(typeof(InvalidInputValueException), ExpectedMessage = "Output folder", MatchType = MessageMatch.Contains)]
        public void WrongOutputFolderInvalidChars()
        {
            _fileNameChangerService.ChangeNames(@"C:\RBS\INPUT", false, @"C:\RBS\OUT<PUT", false,
                    false, false, @"*.txt", @"RB[NUMBER].txt", "1", "6");
        }

        [Test]
        [ExpectedException(typeof(InvalidInputValueException), ExpectedMessage = "Input folder", MatchType = MessageMatch.Contains)]
        public void WrongInputFolderEmpty()
        {
            _fileNameChangerService.ChangeNames(@"", false, @"C:\RBS\OUTPUT", false,
                    false, false, @"*.txt", @"RB[NUMBER].txt", "1", "6");
        }

        [Test]
        [ExpectedException(typeof(InvalidInputValueException), ExpectedMessage = "Input folder", MatchType = MessageMatch.Contains)]
        public void WrongInputFolderInvalidChars()
        {
            _fileNameChangerService.ChangeNames(@"C:\RBS\INP<UT", false, @"C:\RBS\OUTPUT", false,
                    false, false, @"*.txt", @"RB[NUMBER].txt", "1", "6");
        }

        [Test]
        [ExpectedException(typeof(InvalidInputValueException), ExpectedMessage = "Input folder", MatchType = MessageMatch.Contains)]
        public void WrongInputFolderNotExist()
        {
            _fileNameChangerService.ChangeNames(@"C:\ThisFolderShouldHavneNotExisted", false, @"C:\RBS\OUTPUT", false,
                    false, false, @"*.txt", @"RB[NUMBER].txt", "1", "6");
        }

        private string _tempLocation;
        private DirectoryInfo _diInput;
        private DirectoryInfo _diOutput;

        private void PrepareFolders()
        {
            _tempLocation = Assembly.GetExecutingAssembly().Location.Replace(Assembly.GetExecutingAssembly().ManifestModule.Name, "");
            if (Directory.Exists(_tempLocation + @"\UnitTest"))
                Directory.Delete(_tempLocation + @"\UnitTest", true);

            _diInput = Directory.CreateDirectory(_tempLocation + @"\UnitTest\Input");
            _diOutput = Directory.CreateDirectory(_tempLocation + @"\UnitTest\Output");
        }

        private void ClearFolders()
        {
            if (Directory.Exists(_tempLocation + @"\UnitTest"))
                Directory.Delete(_tempLocation + @"\UnitTest", true);
        }

        [Test]
        public void SimpleCopyAFile()
        {
            PrepareFolders();
            File.Create(_diInput.FullName + @"\a.txt").Close();
            
            _fileNameChangerService.ChangeNames(_diInput.FullName, false, _diOutput.FullName, false,
                    false, false, @"*", @"RB[NUMBER].txt", "1", "6");

            Assert.IsTrue(File.Exists(_diOutput.FullName + @"\RB000001.txt"));
            
            ClearFolders();
        }

        [Test]
        public void SimpleCopyThreeFiles()
        {
            PrepareFolders();

            File.Create(_diInput.FullName + @"\a.txt").Close();
            File.Create(_diInput.FullName + @"\a2.txt").Close();
            File.Create(_diInput.FullName + @"\a3.txt").Close();

            _fileNameChangerService.ChangeNames(_diInput.FullName, false, _diOutput.FullName, false,
                    false, false, @"*", @"RB[NUMBER].txt", "1", "6");

            Assert.IsTrue(File.Exists(_diOutput.FullName + @"\RB000001.txt"));
            Assert.IsTrue(File.Exists(_diOutput.FullName + @"\RB000002.txt"));
            Assert.IsTrue(File.Exists(_diOutput.FullName + @"\RB000003.txt"));
            Assert.AreEqual(3, _diOutput.GetFiles().Count());

            ClearFolders();
        }

        [Test]
        public void CopyFilesWithRecursive()
        {
            PrepareFolders();

            File.Create(_diInput.FullName + @"\a.txt").Close();
            File.Create(_diInput.FullName + @"\a2.txt").Close();
            File.Create(_diInput.FullName + @"\a3.txt").Close();

            DirectoryInfo subFolder1 = Directory.CreateDirectory(_diInput.FullName + @"\subfolder1");
            File.Create(subFolder1.FullName + @"\sb1.txt").Close();
            File.Create(subFolder1.FullName + @"\sb1a.txt").Close();
            DirectoryInfo subFolder2 = Directory.CreateDirectory(subFolder1.FullName + @"\subfolder2");
            File.Create(subFolder2.FullName + @"\sb2.txt").Close();
            File.Create(subFolder2.FullName + @"\sb2a.txt").Close();
            DirectoryInfo subFolder3 = Directory.CreateDirectory(_diInput.FullName + @"\subfolder3");
            File.Create(subFolder3.FullName + @"\sb3.txt").Close();
            File.Create(subFolder3.FullName + @"\sb3a.txt").Close();

            _fileNameChangerService.ChangeNames(_diInput.FullName, true, _diOutput.FullName, false,
                    false, false, @"*", @"RB[NUMBER].txt", "1", "6");

            Assert.IsTrue(File.Exists(_diOutput.FullName + @"\RB000001.txt"));
            Assert.IsTrue(File.Exists(_diOutput.FullName + @"\RB000002.txt"));
            Assert.IsTrue(File.Exists(_diOutput.FullName + @"\RB000003.txt"));
            Assert.IsTrue(File.Exists(_diOutput.FullName + @"\RB000004.txt"));
            Assert.IsTrue(File.Exists(_diOutput.FullName + @"\RB000005.txt"));
            Assert.IsTrue(File.Exists(_diOutput.FullName + @"\RB000006.txt"));
            Assert.IsTrue(File.Exists(_diOutput.FullName + @"\RB000007.txt"));
            Assert.IsTrue(File.Exists(_diOutput.FullName + @"\RB000008.txt"));
            Assert.IsTrue(File.Exists(_diOutput.FullName + @"\RB000009.txt"));

            Assert.AreEqual(9, _diOutput.GetFiles().Count());
            Assert.AreEqual(0, _diOutput.GetDirectories().Count());

            ClearFolders();
        }

        [Test]
        public void CopyFilesWithNoRecursive()
        {
            PrepareFolders();

            File.Create(_diInput.FullName + @"\a.txt").Close();
            File.Create(_diInput.FullName + @"\a2.txt").Close();
            File.Create(_diInput.FullName + @"\a3.txt").Close();

            DirectoryInfo subFolder1 = Directory.CreateDirectory(_diInput.FullName + @"\subfolder1");
            File.Create(subFolder1.FullName + @"\sb1.txt").Close();
            File.Create(subFolder1.FullName + @"\sb1a.txt").Close();

            _fileNameChangerService.ChangeNames(_diInput.FullName, false, _diOutput.FullName, false,
                    false, false, @"*", @"RB[NUMBER].txt", "1", "6");

            Assert.IsTrue(File.Exists(_diOutput.FullName + @"\RB000001.txt"));
            Assert.IsTrue(File.Exists(_diOutput.FullName + @"\RB000002.txt"));
            Assert.IsTrue(File.Exists(_diOutput.FullName + @"\RB000003.txt"));

            Assert.AreEqual(3, _diOutput.GetFiles().Count());
            Assert.AreEqual(0, _diOutput.GetDirectories().Count());

            ClearFolders();
        }

        [Test]
        public void CopyFilesWithRestroreFolderTree()
        {
            PrepareFolders();

            File.Create(_diInput.FullName + @"\a.txt").Close();
            File.Create(_diInput.FullName + @"\a2.txt").Close(); ;
            File.Create(_diInput.FullName + @"\a3.txt").Close();

            DirectoryInfo subFolder1 = Directory.CreateDirectory(_diInput.FullName + @"\subfolder1");
            File.Create(subFolder1.FullName + @"\sb1.txt").Close();
            File.Create(subFolder1.FullName + @"\sb1a.txt").Close();
            DirectoryInfo subFolder2 = Directory.CreateDirectory(subFolder1.FullName + @"\subfolder2");
            File.Create(subFolder2.FullName + @"\sb2.txt").Close();
            File.Create(subFolder2.FullName + @"\sb2a.txt").Close();
            DirectoryInfo subFolder3 = Directory.CreateDirectory(_diInput.FullName + @"\subfolder3");
            File.Create(subFolder3.FullName + @"\sb3.txt").Close();
            File.Create(subFolder3.FullName + @"\sb3a.txt").Close();

            _fileNameChangerService.ChangeNames(_diInput.FullName, true, _diOutput.FullName, true,
                    false, false, @"*", @"RB[NUMBER].txt", "1", "6");

            DirectoryInfo subFolder1Out = Directory.CreateDirectory(_diOutput.FullName + @"\subfolder1");
            DirectoryInfo subFolder2Out = Directory.CreateDirectory(subFolder1Out.FullName + @"\subfolder2");
            DirectoryInfo subFolder3Out = Directory.CreateDirectory(_diOutput.FullName + @"\subfolder3");

            Assert.IsTrue(File.Exists(_diOutput.FullName + @"\RB000001.txt"));
            Assert.IsTrue(File.Exists(_diOutput.FullName + @"\RB000002.txt"));
            Assert.IsTrue(File.Exists(_diOutput.FullName + @"\RB000003.txt"));
            Assert.IsTrue(File.Exists(subFolder1Out.FullName + @"\RB000004.txt"));
            Assert.IsTrue(File.Exists(subFolder1Out.FullName + @"\RB000005.txt"));
            Assert.IsTrue(File.Exists(subFolder2Out.FullName + @"\RB000006.txt"));
            Assert.IsTrue(File.Exists(subFolder2Out.FullName + @"\RB000007.txt"));
            Assert.IsTrue(File.Exists(subFolder3Out.FullName + @"\RB000008.txt"));
            Assert.IsTrue(File.Exists(subFolder3Out.FullName + @"\RB000009.txt"));

            Assert.AreEqual(3, _diOutput.GetFiles().Count());
            Assert.AreEqual(2, subFolder1Out.GetFiles().Count());
            Assert.AreEqual(2, subFolder2Out.GetFiles().Count());
            Assert.AreEqual(2, subFolder3Out.GetFiles().Count());
            Assert.AreEqual(2, _diOutput.GetDirectories().Count());
            Assert.AreEqual(1, subFolder1Out.GetDirectories().Count());
            Assert.AreEqual(0, subFolder2Out.GetDirectories().Count());
            Assert.AreEqual(0, subFolder3Out.GetDirectories().Count());

            ClearFolders();
        }

        [Test]
        public void CopyFilesWithCountingFromtheBeginningForEachFolder()
        {
            PrepareFolders();

            File.Create(_diInput.FullName + @"\a.txt").Close();
            File.Create(_diInput.FullName + @"\a2.txt").Close(); ;
            File.Create(_diInput.FullName + @"\a3.txt").Close();

            DirectoryInfo subFolder1 = Directory.CreateDirectory(_diInput.FullName + @"\subfolder1");
            File.Create(subFolder1.FullName + @"\sb1.txt").Close();
            File.Create(subFolder1.FullName + @"\sb1a.txt").Close();
            DirectoryInfo subFolder2 = Directory.CreateDirectory(subFolder1.FullName + @"\subfolder2");
            File.Create(subFolder2.FullName + @"\sb2.txt").Close();
            File.Create(subFolder2.FullName + @"\sb2a.txt").Close();
            DirectoryInfo subFolder3 = Directory.CreateDirectory(_diInput.FullName + @"\subfolder3");
            File.Create(subFolder3.FullName + @"\sb3.txt").Close();
            File.Create(subFolder3.FullName + @"\sb3a.txt").Close();

            _fileNameChangerService.ChangeNames(_diInput.FullName, true, _diOutput.FullName, true,
                    true, false, @"*", @"RB[NUMBER].txt", "1", "6");

            DirectoryInfo subFolder1Out = Directory.CreateDirectory(_diOutput.FullName + @"\subfolder1");
            DirectoryInfo subFolder2Out = Directory.CreateDirectory(subFolder1Out.FullName + @"\subfolder2");
            DirectoryInfo subFolder3Out = Directory.CreateDirectory(_diOutput.FullName + @"\subfolder3");

            Assert.IsTrue(File.Exists(_diOutput.FullName + @"\RB000001.txt"));
            Assert.IsTrue(File.Exists(_diOutput.FullName + @"\RB000002.txt"));
            Assert.IsTrue(File.Exists(_diOutput.FullName + @"\RB000003.txt"));
            Assert.IsTrue(File.Exists(subFolder1Out.FullName + @"\RB000001.txt"));
            Assert.IsTrue(File.Exists(subFolder1Out.FullName + @"\RB000002.txt"));
            Assert.IsTrue(File.Exists(subFolder2Out.FullName + @"\RB000001.txt"));
            Assert.IsTrue(File.Exists(subFolder2Out.FullName + @"\RB000002.txt"));
            Assert.IsTrue(File.Exists(subFolder3Out.FullName + @"\RB000001.txt"));
            Assert.IsTrue(File.Exists(subFolder3Out.FullName + @"\RB000002.txt"));

            Assert.AreEqual(3, _diOutput.GetFiles().Count());
            Assert.AreEqual(2, subFolder1Out.GetFiles().Count());
            Assert.AreEqual(2, subFolder2Out.GetFiles().Count());
            Assert.AreEqual(2, subFolder3Out.GetFiles().Count());
            Assert.AreEqual(2, _diOutput.GetDirectories().Count());
            Assert.AreEqual(1, subFolder1Out.GetDirectories().Count());
            Assert.AreEqual(0, subFolder2Out.GetDirectories().Count());
            Assert.AreEqual(0, subFolder3Out.GetDirectories().Count());

            ClearFolders();
        }

        [Test]
        public void CopyFilesWithOverwriting()
        {
            PrepareFolders();

            File.Create(_diInput.FullName + @"\a.txt").Close();

            File.Create(_diOutput.FullName + @"\RB000001.txt").Close();

            _fileNameChangerService.ChangeNames(_diInput.FullName, true, _diOutput.FullName, true,
                    false, true, @"*", @"RB[NUMBER].txt", "1", "6");

            Assert.IsTrue(File.Exists(_diOutput.FullName + @"\RB000001.txt"));

            Assert.AreEqual(1, _diOutput.GetFiles().Count());
            Assert.AreEqual(0, _diOutput.GetDirectories().Count());

            ClearFolders();
        }

        [Test]
        [ExpectedException(typeof(IOException), ExpectedMessage = "RB000001", MatchType = MessageMatch.Contains)]
        public void CopyFilesWithNoOverwritingError()
        {
            PrepareFolders();

            File.Create(_diInput.FullName + @"\a.txt").Close();

            File.Create(_diOutput.FullName + @"\RB000001.txt").Close();

            _fileNameChangerService.ChangeNames(_diInput.FullName, true, _diOutput.FullName, true,
                    false, false, @"*", @"RB[NUMBER].txt", "1", "6");

            ClearFolders();
        }

        [Test]
        public void CopyFilesWithInputMaskOnlyOnTxt()
        {
            PrepareFolders();

            File.Create(_diInput.FullName + @"\a.txt").Close();
            File.Create(_diInput.FullName + @"\a2.csv").Close(); ;
            File.Create(_diInput.FullName + @"\a3.txt").Close();

            DirectoryInfo subFolder1 = Directory.CreateDirectory(_diInput.FullName + @"\subfolder1");
            File.Create(subFolder1.FullName + @"\sb1.txt").Close();
            File.Create(subFolder1.FullName + @"\sb1a.txt").Close();
            DirectoryInfo subFolder2 = Directory.CreateDirectory(subFolder1.FullName + @"\subfolder2");
            File.Create(subFolder2.FullName + @"\sb2.txt").Close();
            File.Create(subFolder2.FullName + @"\sb2a.csv").Close();
            DirectoryInfo subFolder3 = Directory.CreateDirectory(_diInput.FullName + @"\subfolder3");
            File.Create(subFolder3.FullName + @"\sb3.csv").Close();
            File.Create(subFolder3.FullName + @"\sb3a.txt").Close();

            _fileNameChangerService.ChangeNames(_diInput.FullName, true, _diOutput.FullName, true,
                    false, true, @"*.txt", @"RB[NUMBER].txt", "1", "6");

            DirectoryInfo subFolder1Out = Directory.CreateDirectory(_diOutput.FullName + @"\subfolder1");
            DirectoryInfo subFolder2Out = Directory.CreateDirectory(subFolder1Out.FullName + @"\subfolder2");
            DirectoryInfo subFolder3Out = Directory.CreateDirectory(_diOutput.FullName + @"\subfolder3");

            Assert.IsTrue(File.Exists(_diOutput.FullName + @"\RB000001.txt"));
            Assert.IsTrue(File.Exists(_diOutput.FullName + @"\RB000002.txt"));
            Assert.IsTrue(File.Exists(subFolder1Out.FullName + @"\RB000003.txt"));
            Assert.IsTrue(File.Exists(subFolder1Out.FullName + @"\RB000004.txt"));
            Assert.IsTrue(File.Exists(subFolder2Out.FullName + @"\RB000005.txt"));
            Assert.IsTrue(File.Exists(subFolder3Out.FullName + @"\RB000006.txt"));

            Assert.AreEqual(2, _diOutput.GetFiles().Count());
            Assert.AreEqual(2, subFolder1Out.GetFiles().Count());
            Assert.AreEqual(1, subFolder2Out.GetFiles().Count());
            Assert.AreEqual(1, subFolder3Out.GetFiles().Count());

            ClearFolders();
        }

        [Test]
        public void CopyFilesWithOutputMaskChanged()
        {
            PrepareFolders();

            File.Create(_diInput.FullName + @"\a.txt").Close();
            File.Create(_diInput.FullName + @"\a2.csv").Close(); ;
            File.Create(_diInput.FullName + @"\a3.txt").Close();

            DirectoryInfo subFolder1 = Directory.CreateDirectory(_diInput.FullName + @"\subfolder1");
            File.Create(subFolder1.FullName + @"\sb1.txt").Close();
            File.Create(subFolder1.FullName + @"\sb1a.txt").Close();
            DirectoryInfo subFolder2 = Directory.CreateDirectory(subFolder1.FullName + @"\subfolder2");
            File.Create(subFolder2.FullName + @"\sb2.txt").Close();
            File.Create(subFolder2.FullName + @"\sb2a.csv").Close();
            DirectoryInfo subFolder3 = Directory.CreateDirectory(_diInput.FullName + @"\subfolder3");
            File.Create(subFolder3.FullName + @"\sb3.csv").Close();
            File.Create(subFolder3.FullName + @"\sb3a.txt").Close();

            _fileNameChangerService.ChangeNames(_diInput.FullName, true, _diOutput.FullName, true,
                    false, true, @"*.txt", @"OBA[NUMBER]T.txt", "1", "6");

            DirectoryInfo subFolder1Out = Directory.CreateDirectory(_diOutput.FullName + @"\subfolder1");
            DirectoryInfo subFolder2Out = Directory.CreateDirectory(subFolder1Out.FullName + @"\subfolder2");
            DirectoryInfo subFolder3Out = Directory.CreateDirectory(_diOutput.FullName + @"\subfolder3");

            Assert.IsTrue(File.Exists(_diOutput.FullName + @"\OBA000001T.txt"));
            Assert.IsTrue(File.Exists(_diOutput.FullName + @"\OBA000002T.txt"));
            Assert.IsTrue(File.Exists(subFolder1Out.FullName + @"\OBA000003T.txt"));
            Assert.IsTrue(File.Exists(subFolder1Out.FullName + @"\OBA000004T.txt"));
            Assert.IsTrue(File.Exists(subFolder2Out.FullName + @"\OBA000005T.txt"));
            Assert.IsTrue(File.Exists(subFolder3Out.FullName + @"\OBA000006T.txt"));

            Assert.AreEqual(2, _diOutput.GetFiles().Count());
            Assert.AreEqual(2, subFolder1Out.GetFiles().Count());
            Assert.AreEqual(1, subFolder2Out.GetFiles().Count());
            Assert.AreEqual(1, subFolder3Out.GetFiles().Count());

            ClearFolders();
        }

        [Test]
        public void CopyFilesWitStartNumberEqual3()
        {
            PrepareFolders();

            File.Create(_diInput.FullName + @"\a.txt").Close();
            File.Create(_diInput.FullName + @"\a2.csv").Close(); ;
            File.Create(_diInput.FullName + @"\a3.txt").Close();

            DirectoryInfo subFolder1 = Directory.CreateDirectory(_diInput.FullName + @"\subfolder1");
            File.Create(subFolder1.FullName + @"\sb1.txt").Close();
            File.Create(subFolder1.FullName + @"\sb1a.txt").Close();
            DirectoryInfo subFolder2 = Directory.CreateDirectory(subFolder1.FullName + @"\subfolder2");
            File.Create(subFolder2.FullName + @"\sb2.txt").Close();
            File.Create(subFolder2.FullName + @"\sb2a.csv").Close();
            DirectoryInfo subFolder3 = Directory.CreateDirectory(_diInput.FullName + @"\subfolder3");
            File.Create(subFolder3.FullName + @"\sb3.csv").Close();
            File.Create(subFolder3.FullName + @"\sb3a.txt").Close();

            _fileNameChangerService.ChangeNames(_diInput.FullName, true, _diOutput.FullName, true,
                    false, true, @"*.txt", @"OBA[NUMBER]T.txt", "3", "6");

            DirectoryInfo subFolder1Out = Directory.CreateDirectory(_diOutput.FullName + @"\subfolder1");
            DirectoryInfo subFolder2Out = Directory.CreateDirectory(subFolder1Out.FullName + @"\subfolder2");
            DirectoryInfo subFolder3Out = Directory.CreateDirectory(_diOutput.FullName + @"\subfolder3");

            Assert.IsTrue(File.Exists(_diOutput.FullName + @"\OBA000003T.txt"));
            Assert.IsTrue(File.Exists(_diOutput.FullName + @"\OBA000004T.txt"));
            Assert.IsTrue(File.Exists(subFolder1Out.FullName + @"\OBA000005T.txt"));
            Assert.IsTrue(File.Exists(subFolder1Out.FullName + @"\OBA000006T.txt"));
            Assert.IsTrue(File.Exists(subFolder2Out.FullName + @"\OBA000007T.txt"));
            Assert.IsTrue(File.Exists(subFolder3Out.FullName + @"\OBA000008T.txt"));

            Assert.AreEqual(2, _diOutput.GetFiles().Count());
            Assert.AreEqual(2, subFolder1Out.GetFiles().Count());
            Assert.AreEqual(1, subFolder2Out.GetFiles().Count());
            Assert.AreEqual(1, subFolder3Out.GetFiles().Count());

            ClearFolders();
        }

        [Test]
        public void CopyFilesWitNumberOfDigitsEqual2()
        {
            PrepareFolders();

            File.Create(_diInput.FullName + @"\a.txt").Close();
            File.Create(_diInput.FullName + @"\a2.csv").Close(); ;
            File.Create(_diInput.FullName + @"\a3.txt").Close();

            DirectoryInfo subFolder1 = Directory.CreateDirectory(_diInput.FullName + @"\subfolder1");
            File.Create(subFolder1.FullName + @"\sb1.txt").Close();
            File.Create(subFolder1.FullName + @"\sb1a.txt").Close();
            DirectoryInfo subFolder2 = Directory.CreateDirectory(subFolder1.FullName + @"\subfolder2");
            File.Create(subFolder2.FullName + @"\sb2.txt").Close();
            File.Create(subFolder2.FullName + @"\sb2a.csv").Close();
            DirectoryInfo subFolder3 = Directory.CreateDirectory(_diInput.FullName + @"\subfolder3");
            File.Create(subFolder3.FullName + @"\sb3.csv").Close();
            File.Create(subFolder3.FullName + @"\sb3a.txt").Close();

            _fileNameChangerService.ChangeNames(_diInput.FullName, true, _diOutput.FullName, true,
                    false, true, @"*.txt", @"OBA[NUMBER]T.txt", "3", "2");

            DirectoryInfo subFolder1Out = Directory.CreateDirectory(_diOutput.FullName + @"\subfolder1");
            DirectoryInfo subFolder2Out = Directory.CreateDirectory(subFolder1Out.FullName + @"\subfolder2");
            DirectoryInfo subFolder3Out = Directory.CreateDirectory(_diOutput.FullName + @"\subfolder3");

            Assert.IsTrue(File.Exists(_diOutput.FullName + @"\OBA03T.txt"));
            Assert.IsTrue(File.Exists(_diOutput.FullName + @"\OBA04T.txt"));
            Assert.IsTrue(File.Exists(subFolder1Out.FullName + @"\OBA05T.txt"));
            Assert.IsTrue(File.Exists(subFolder1Out.FullName + @"\OBA06T.txt"));
            Assert.IsTrue(File.Exists(subFolder2Out.FullName + @"\OBA07T.txt"));
            Assert.IsTrue(File.Exists(subFolder3Out.FullName + @"\OBA08T.txt"));

            Assert.AreEqual(2, _diOutput.GetFiles().Count());
            Assert.AreEqual(2, subFolder1Out.GetFiles().Count());
            Assert.AreEqual(1, subFolder2Out.GetFiles().Count());
            Assert.AreEqual(1, subFolder3Out.GetFiles().Count());

            ClearFolders();
        }

        [Test]
        [ExpectedException(typeof(OverflowException), ExpectedMessage = "Overflow specified number of digits", MatchType = MessageMatch.Contains)]
        public void CopyFilesWitTooSmallNumberOfDigits()
        {
            PrepareFolders();

            File.Create(_diInput.FullName + @"\a.txt").Close();
            File.Create(_diInput.FullName + @"\a2.csv").Close(); ;
            File.Create(_diInput.FullName + @"\a3.txt").Close();

            DirectoryInfo subFolder1 = Directory.CreateDirectory(_diInput.FullName + @"\subfolder1");
            File.Create(subFolder1.FullName + @"\sb1.txt").Close();
            File.Create(subFolder1.FullName + @"\sb1a.txt").Close();
            DirectoryInfo subFolder2 = Directory.CreateDirectory(subFolder1.FullName + @"\subfolder2");
            File.Create(subFolder2.FullName + @"\sb2.txt").Close();
            File.Create(subFolder2.FullName + @"\sb2a.csv").Close();
            DirectoryInfo subFolder3 = Directory.CreateDirectory(_diInput.FullName + @"\subfolder3");
            File.Create(subFolder3.FullName + @"\sb3.csv").Close();
            File.Create(subFolder3.FullName + @"\sb3a.txt").Close();

            _fileNameChangerService.ChangeNames(_diInput.FullName, true, _diOutput.FullName, true,
                    false, true, @"*", @"OBA[NUMBER]T.txt", "8", "1");

            ClearFolders();
        }
    }
}
