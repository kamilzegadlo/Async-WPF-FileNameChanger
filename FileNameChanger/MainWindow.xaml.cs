using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FileNameChanger.Service;
using System.Threading;
using System.IO;
using System.Reflection;

namespace FileNameChanger
{
    public partial class MainWindow : Window
    {
        public IFileNameChangerService _fileNameChangerService;
        private CancellationTokenSource _cancellationTokenSource;
        private bool aborted;
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(0,1);

        public MainWindow(IFileNameChangerService fileNameChangerService)
        {
            _fileNameChangerService = fileNameChangerService;
            InitializeComponent();
            Version.Content = String.Format("UI version: {0}, Service version: {1}", Assembly.GetExecutingAssembly().GetName().Version.ToString(), Assembly.GetAssembly(typeof(IFileNameChangerService)).GetName().Version.ToString());
            ClearLogs();
        }

        private async void Start_ButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ExecutionBar.Value = 0;

                SetEnableToAllControls(false);

                AddLog(String.Format("Start execution time: {0}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")));

                _fileNameChangerService = new FileNameChangerService();

                LogInputs();

                int countofFiles = await GetNumberOfFilesAsync();

                AddLog("Number of files to proceed: " + countofFiles);

                ExecutionBar.Maximum = countofFiles;

                ICollection<IMapEntry> map = await ChangeNamesAsync();

                if (!aborted)
                {
                    SaveMappingFile(map);
                    AddLog("**Application execution finished successfully**");
                }
            }
            catch(Exception ex)
            {
                AddLog("!!EXCEPTION: ");
                AddLog(ex.Message);

                while(ex.InnerException!=null && !String.IsNullOrEmpty(ex.InnerException.Message))
                {
                    AddLog("!INNER EXCEPTION: ");
                    AddLog(ex.Message);
                    ex=ex.InnerException;
                }
            }
            finally
            {
                SaveLogsToFile();

                AddLog(String.Format("Finish execution time: {0}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")) + 
                    Environment.NewLine + Environment.NewLine + "------------------------");
                SetEnableToAllControls(true);
            }
        }

        private void SaveMappingFile(ICollection<IMapEntry> map)
        {
            string name = Assembly.GetExecutingAssembly().Location.Replace(Assembly.GetExecutingAssembly().ManifestModule.Name, "");
            if (!name.EndsWith(@"\"))
                name += @"\";
            name += "map.csv";

            if (File.Exists(name))
                File.Delete(name);

            StreamWriter sw = File.CreateText(name);

            foreach (IMapEntry mapEntry in map)
                sw.WriteLine(String.Format("\"{0}\",\"{1}\"",mapEntry.SourceFile.FullName,mapEntry.DestinationFile.FullName));
            
            sw.Close();
            sw.Dispose();

            AddLog("Mapping file saved to: " + name);
        }

        private void SaveLogsToFile()
        {
            string name = Assembly.GetExecutingAssembly().Location.Replace(Assembly.GetExecutingAssembly().ManifestModule.Name, "");
            if(!name.EndsWith(@"\"))
                name+=@"\";
            name+="logs.txt";

            if(File.Exists(name))
                File.Delete(name);

            StreamWriter sw= File.CreateText(name);
            sw.Write(LogsTextArea.Text);
            sw.Close();
            sw.Dispose();

            AddLog("Logs saved to: " + name);
        }

        private async Task<int> GetNumberOfFilesAsync()
        {
            return await _fileNameChangerService.GetNumberOfFilesAsync(InputFolder.Text, (bool)InputRekursive.IsChecked, InputMask.Text).ConfigureAwait(false);
        }

        private async Task<ICollection<IMapEntry>> ChangeNamesAsync()
        {
            var progress = new Progress<IUpdateInfo>();
            progress.ProgressChanged += new EventHandler<IUpdateInfo>(UpdateLogs);

            _cancellationTokenSource=new CancellationTokenSource();

            return await _fileNameChangerService.ChangeNamesAsync(InputFolder.Text, (bool)InputRekursive.IsChecked, OutputFolder.Text, (bool)RestoreFoldersTree.IsChecked,
                    (bool)CountingFromTheBeginning.IsChecked, (bool)OverwriteFiles.IsChecked, InputMask.Text, OutputMask.Text, StartNumber.Text, NumberOfDigits.Text, (bool)UpdatePBOnly.IsChecked, progress, _cancellationTokenSource.Token).ConfigureAwait(false);
        }

        private void AddLog(string message, bool emptyLinePre=true, bool emptyLinePost=false)
        {
            semaphore.WaitAsync();

            if (emptyLinePre) LogsTextArea.AppendText(Environment.NewLine);
            LogsTextArea.AppendText(message);
            LogsTextArea.AppendText(Environment.NewLine);
            if (emptyLinePost) LogsTextArea.AppendText(Environment.NewLine);
            LogsTextArea.ScrollToEnd();

            semaphore.Release();
        }

        private void BrowseInputFolder_ButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                InputFolder.Text = dialog.SelectedPath.ToString();
        }

        private void BrowseOutputFolder_ButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                OutputFolder.Text = dialog.SelectedPath.ToString();
        }

        private void UpdateLogs(object sender, IUpdateInfo messages)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ExecutionBar.Value += messages.GetCounter;
                if (!(bool)UpdatePBOnly.IsChecked || ExecutionBar.Value>=ExecutionBar.Maximum) 
                {
                    AddLog(messages.GetMessage);
                    AddLog("Files copied: " + ExecutionBar.Value + "/" + ExecutionBar.Maximum, false);
                }
            }));
        }

        private void ClearLogs_Click(object sender, RoutedEventArgs e)
        {
            ClearLogs();
        }

        private void ClearLogs()
        {
            LogsTextArea.Text = "Logs:";
            AddLog(Version.Content.ToString());
        }

        private void SetEnableToAllControls(bool enable)
        {
            InputFolder.IsEnabled = enable;
            InputRekursive.IsEnabled = enable;
            OutputFolder.IsEnabled = enable;
            RestoreFoldersTree.IsEnabled = enable;
            CountingFromTheBeginning.IsEnabled = enable; ;
            OverwriteFiles.IsEnabled = enable;
            InputMask.IsEnabled = enable;
            OutputMask.IsEnabled = enable;
            StartNumber.IsEnabled = enable;
            NumberOfDigits.IsEnabled = enable;
            StartButton.IsEnabled = enable;
            ClearLogsButton.IsEnabled = enable;
            BrowseInput.IsEnabled = enable;
            BrowseOutput.IsEnabled = enable;
            UpdatePBOnly.IsEnabled = enable;
            Cancel.Visibility = enable ? Visibility.Hidden : Visibility.Visible;
        }

        private void LogInputs()
        {
            AddLog("Given inputs:", true, false);
            AddLog(String.Format("User who run the process: {0}", Environment.UserName),false);
            AddLog(String.Format("Input folder: {0}", InputFolder.Text), false);
            AddLog(String.Format("Rekursion: {0}", (bool)InputRekursive.IsChecked), false);
            AddLog(String.Format("Output folder: {0}", OutputFolder.Text), false);
            AddLog(String.Format("Restore input folder tree structure: {0}", (bool)RestoreFoldersTree.IsChecked), false);
            AddLog(String.Format("Start counting from the beggining for each folder: {0}", (bool)CountingFromTheBeginning.IsChecked), false);
            AddLog(String.Format("Overwrite files: {0}", (bool)OverwriteFiles.IsChecked), false);
            AddLog(String.Format("Mask of the input files: {0}", InputMask.Text), false);
            AddLog(String.Format("Mask of the output files: {0}", OutputMask.Text), false);
            AddLog(String.Format("Start number: {0}", StartNumber.Text), false);
            AddLog(String.Format("Number of digits: {0}", NumberOfDigits.Text), false);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (_cancellationTokenSource != null)
            {
                aborted = true;
                _cancellationTokenSource.Cancel();
                ExecutionBar.Value = 0;
                AddLog("Execution aborted by user.");
            }
        }
    }
}
