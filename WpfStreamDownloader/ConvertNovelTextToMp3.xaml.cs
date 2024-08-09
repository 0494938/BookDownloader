using BaseBookDownloader;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Windows;
using WpfStreamDownloader;

namespace BookStreamDownload.WPF
{
#pragma warning disable CS8604 // Null 参照引数の可能性があります。
#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
#pragma warning disable IDE0044 // 読み取り専用修飾子を追加します
    public partial class ConvertNovelTextToMp3 : Window
    {
        BaseWndContextData? datacontext;
        string? strNovelFilePath = null;
        string? strMp3OutputPath = null;
        public ConvertNovelTextToMp3(BaseWndContextData datacontext)
        {
            this.datacontext = datacontext;
            InitializeComponent();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnNoveltextFilePath_Click(object sender, RoutedEventArgs e)
        {
            Debug.Assert(datacontext != null);
            if (datacontext != null)
            {
                OpenFileDialog file= new OpenFileDialog
                {
                    Title = "Select Mp3 Output Folder",
                    InitialDirectory = (string.IsNullOrEmpty(datacontext.FileSavePath) ? (AppDomain.CurrentDomain.BaseDirectory + "mp3\\") : datacontext.FileSavePath)
                };

                if (file.ShowDialog() == true)
                {
                    txtNovelTextFile.Text = file.FileName;
                    strNovelFilePath = file.FileName;
                }
            }
        }

        private void btnMp3OutputPathSetting_Click(object sender, RoutedEventArgs e)
        {
            Debug.Assert(datacontext != null);
            if (datacontext != null)
            {
                OpenFolderDialog folderDialog = new OpenFolderDialog
                {
                    Title = "Select Mp3 Output Folder",
                    InitialDirectory = (string.IsNullOrEmpty(datacontext.FileSavePath) ? (AppDomain.CurrentDomain.BaseDirectory + "mp3\\") : datacontext.FileSavePath)
                };

                if (folderDialog.ShowDialog() == true)
                {
                    string folderName = folderDialog.FolderName;
                    txtMp3OutputPath.Text = folderName + System.IO.Path.DirectorySeparatorChar;
                    strMp3OutputPath= txtMp3OutputPath.Text;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtNovelTextFile.Text = datacontext?.FileSavePath?.Trim() ?? "";
            strNovelFilePath = datacontext?.FileSavePath?.Trim() ?? "";
            txtMp3OutputPath.Text = datacontext?.FileSavePath?.Trim() ?? "";
            strMp3OutputPath = datacontext?.FileSavePath?.Trim() ?? "";
        }

        private void OnCheckBreakToChapter(object sender, RoutedEventArgs e)
        {

        }

        private void btnConvert_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(strNovelFilePath))
            {
                MessageBox.Show(strNovelFilePath + " not exists, please select a valid UTF8 text file.", "Error", MessageBoxButton.OK);
                return;
            }

            WpfStreamMainWindow? parent = this.Owner as WpfStreamMainWindow;
            if (chkBreakToChapter.IsChecked??false) { 
                new Thread(() => { parent?.BreakChapterAndConvertTextToMp3Thread(datacontext, strNovelFilePath, strMp3OutputPath); }).Start(); 
            } else { 
                new Thread(() => { parent?.ConvertTextToMp3Thread(datacontext, strNovelFilePath, strMp3OutputPath); }).Start(); 
            }

            DialogResult = true;
        }

    }
#pragma warning restore CS8604 // Null 参照引数の可能性があります。
#pragma warning restore IDE0044 // 読み取り専用修飾子を追加します
#pragma warning restore CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
}
