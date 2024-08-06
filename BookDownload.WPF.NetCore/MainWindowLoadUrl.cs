using BaseBookDownloader;
using CefSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WpfBookDownloader
{
#pragma warning disable CA1416 // プラットフォームの互換性を検証
#pragma warning disable CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
#pragma warning disable IDE0019 // パターン マッチングを使用します
    public partial class WindowsWPFChrome: Window, IBaseMainWindow
    {
        private void btnInitURL_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("btnInitURL_Click invoked...");
            ClickBtnOpenUrl(txtInitURL.Text);
        }

        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("btnNextPage_Click invoked...");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if ((datacontext != null))
                datacontext.SiteType = (BatchQueryNovelContents)cmbNovelType.SelectedIndex;

            ClickBtnOpenUrl(txtNextUrl.Text);
        }

        private void OnBtnRefreshPage(object sender, RoutedEventArgs e)
        {
            webBrowser.Reload();
        }

        private void btnCurrURL_Click(object sender, RoutedEventArgs e)
        {
            ClickBtnOpenUrl(txtCurURL.Text);
        }

        private void btnAutoURL_Click(object sender, RoutedEventArgs e)
        {
            txtLog.Clear();
            txtChapter.Clear();
            Debug.WriteLine("btnAutoURL_Click invoked...");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if (datacontext != null)
            {
                datacontext.DictDownloadStatus.Clear();
                datacontext.BackGroundNotRunning = false;
                btnInitURL.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                btnAutoDownload.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                datacontext.SiteType = (BatchQueryNovelContents)cmbNovelType.SelectedIndex;
                txtAggregatedContents.Clear();
                UpdateStatusMsg(datacontext, "Selected Site Type: " + cmbNovelType.SelectedIndex, -1);
                int nMaxPage = string.IsNullOrEmpty(txtPages.Text.Trim()) ? 20 : int.Parse(txtPages.Text.Trim());
                DownloadStatus.MaxPageToDownload = nMaxPage;

                DownloadStatus.ContentsWriter = new StreamWriter(
                    File.Open(datacontext.FileSavePath + (string.IsNullOrEmpty(txtOutputFileName.Text.Trim()) ? @"Content" : txtOutputFileName.Text.Trim()) + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss") + ".txt", 
                    FileMode.CreateNew, 
                    FileAccess.ReadWrite,
                    FileShare.Read),
                    Encoding.UTF8
                );

                datacontext.DownloadOneURLAndGetNext(this, txtInitURL.Text, false);
            }
        }
    }
#pragma warning restore IDE0019 // パターン マッチングを使用します
#pragma warning restore CA1416 // プラットフォームの互換性を検証
#pragma warning restore CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
}