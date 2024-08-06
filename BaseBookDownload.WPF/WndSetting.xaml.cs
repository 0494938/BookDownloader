using BaseBookDownloader;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows;
using System.Windows.Shapes;

namespace BaseBookDownload.WPF
{
    /// <summary>
    /// WndSetting.xaml の相互作用ロジック
    /// </summary>
    public partial class WndSetting : Window
    {
        public WndSetting()
        {
            datacontext = new();
            InitializeComponent();
        }
        public WndSetting(BaseWndContextData datacontext)
        {
            InitializeComponent();
            this.datacontext = datacontext;
        }
        BaseWndContextData datacontext;
        
        private void btnSavePathSetting_Click(object sender, RoutedEventArgs e)
        {
            Debug.Assert(datacontext != null);
            if (datacontext != null)
            {
                OpenFolderDialog folderDialog = new OpenFolderDialog
                {
                    Title = "Select File Save Folder",
                    //InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
                    InitialDirectory = (string.IsNullOrEmpty(datacontext.FileSavePath) ? AppDomain.CurrentDomain.BaseDirectory : datacontext.FileSavePath)
                };

                if (folderDialog.ShowDialog() == true)
                {
                    string folderName = folderDialog.FolderName;
                    //MessageBox.Show($"You picked ${folderName}!");
                    txtSavePath.Text = folderName + System.IO.Path.DirectorySeparatorChar;
                    datacontext.FileSavePath = folderName + System.IO.Path.DirectorySeparatorChar;

                    RegistryKey? registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\zdhe\\batchdownload\\1.0", true);
                    if (registryKey == null)
                    {
                        registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\zdhe\\batchdownload\\1.0", true);
                    }
                    if (registryKey != null)
                    {
                        registryKey.SetValue("FileSavePath", datacontext.FileSavePath);
                        registryKey.Close();
                    }
                }
            }
        }

        private void btnTempPathSetting_Click(object sender, RoutedEventArgs e)
        {
            Debug.Assert(datacontext != null);
            if (datacontext != null)
            {
                OpenFolderDialog folderDialog = new OpenFolderDialog
                {
                    Title = "Select Temporary File Save Folder",
                    //InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
                    InitialDirectory = (string.IsNullOrEmpty(datacontext.FileTempPath) ? (AppDomain.CurrentDomain.BaseDirectory + "temp\\") : datacontext.FileTempPath)
                };

                if (folderDialog.ShowDialog() == true)
                {
                    string folderName = folderDialog.FolderName;
                    //MessageBox.Show($"You picked ${folderName}!");
                    txtTempPath.Text = folderName + System.IO.Path.DirectorySeparatorChar;
                    datacontext.FileTempPath = folderName + System.IO.Path.DirectorySeparatorChar;

                    RegistryKey? registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\zdhe\\batchdownload\\1.0", true);
                    if (registryKey == null)
                    {
                        registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\zdhe\\batchdownload\\1.0", true);
                    }
                    if (registryKey != null)
                    {
                        registryKey.SetValue("FileTempPath", datacontext.FileTempPath);
                        registryKey.Close();
                    }
                }
            }
        }

        private void btnCancel_click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnOK_click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtSavePath.Text = datacontext.FileSavePath.Trim();
            txtTempPath.Text = datacontext.FileTempPath.Trim();
        }
    }
}
