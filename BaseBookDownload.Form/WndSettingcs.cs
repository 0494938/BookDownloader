using BaseBookDownloader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaseBookDownload.Frm
{
    public partial class WndSettingcs : System.Windows.Forms.Form
    {
        public WndSettingcs()
        {
            InitializeComponent();
        }

        public WndSettingcs(BaseWndContextData contextdata)
        {
            this.datacontext = contextdata;
            InitializeComponent();
        }

        private void btnCancel_click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void Window_Loaded(object sender, EventArgs e)
        {
            txtSavePath.Text = datacontext.FileSavePath.Trim();
            txtTempPath.Text = datacontext.FileTempPath.Trim();
        }

        BaseWndContextData datacontext;

        private void btnSavePathSetting_Click(object sender, EventArgs e)
        {
            Debug.Assert(datacontext != null);
            if (datacontext != null)
            {
                FolderBrowserDialog folderDialog = new FolderBrowserDialog
                {
                    Description = "Select File Save Folder",
                    //InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
                    SelectedPath = (string.IsNullOrEmpty(datacontext.FileSavePath) ? AppDomain.CurrentDomain.BaseDirectory : datacontext.FileSavePath)
                };

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string folderName = folderDialog.SelectedPath;
                    //MessageBox.Show($"You picked ${folderName}!");
                    txtSavePath.Text = folderName + System.IO.Path.DirectorySeparatorChar;
                    datacontext.FileSavePath = folderName + System.IO.Path.DirectorySeparatorChar;

                    Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\zdhe\\batchdownload\\1.0", true);
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

        private void btnTempPathSetting_Click(object sender, EventArgs e)
        {
            Debug.Assert(datacontext != null);
            if (datacontext != null)
            {
                FolderBrowserDialog folderDialog = new FolderBrowserDialog
                {
                    Description = "Select Temporary File Save Folder",
                    //InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
                    SelectedPath = (string.IsNullOrEmpty(datacontext.FileTempPath) ? (AppDomain.CurrentDomain.BaseDirectory + "temp\\") : datacontext.FileTempPath)
                };

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string folderName = folderDialog.SelectedPath;
                    //MessageBox.Show($"You picked ${folderName}!");
                    txtTempPath.Text = folderName + System.IO.Path.DirectorySeparatorChar;
                    datacontext.FileTempPath = folderName + System.IO.Path.DirectorySeparatorChar;

                    Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\zdhe\\batchdownload\\1.0", true);
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

        private void WndSettingcs_Load(object sender, EventArgs e)
        {
            txtSavePath.Text = datacontext.FileSavePath;
            txtTempPath.Text = datacontext.FileTempPath;
        }
    }
}
