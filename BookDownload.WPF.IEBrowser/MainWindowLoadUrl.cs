using BaseBookDownloader;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WpfIEBookDownloader
{
    public partial class WPFMainWindow : Window, IBaseMainWindow
    {
        private void btnInitURL_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("btnInitURL_Click invoked...");
            ClickBtntnInitURL(txtInitURL.Text);
        }

        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("btnNextPage_Click invoked...");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if ((datacontext != null))
                datacontext.SiteType = (BatchQueryNovelContents)cmbNovelType.SelectedIndex;

            ClickBtntnInitURL(txtCurURL.Text);
        }

        private void ClickBtntnInitURL(string strUrl)
        {
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if ((datacontext != null))
            {
                datacontext.DictDownloadStatus.Clear();
                datacontext.PageLoaded = false;
                datacontext.NextLinkAnalysized = false;
                // btnAnalysisCurURL.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                btnNextPage.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                txtWebContents.Text = "";
                txtAnalysizedContents.Text = "";

                datacontext.SiteType = (BatchQueryNovelContents)cmbNovelType.SelectedIndex;
                try
                {
                    datacontext.PgmNaviUrl = strUrl;
                    //webBrowser.Navigate(strUrl);
                    webBrowser.CoreWebView2.Navigate(strUrl);
                    UpdateStatusMsg(datacontext, strUrl + " : Begin to download ...", 0);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    if (webBrowser == null || webBrowser.CoreWebView2 == null || webBrowser.IsLoaded == false)
                        return;

                    //SHDocVw.WebBrowser? webBrowserPtr = GetWebBrowserPtr(webBrowser);
                    //Debug.WriteLine(strUrl + " : Status <" + webBrowserPtr?.ReadyState.ToString() + ">");
                    //if (webBrowserPtr?.ReadyState != SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
                    //    return;
                }
            }
        }

        public string BatchDownloadNotified(BaseWndContextData datacontext, DownloadStatus status, string sDownloadFileName)
        {
            string strMsgAreaLog="";
            this.Dispatcher.Invoke(() =>
            {
                this.UpdateInitPageButton();
                this.UpdateAutoDownloadPageButton();

                UpdateStatusMsg(datacontext, "Flush Log to file: " + sDownloadFileName + ".log", -1);
                if (!string.IsNullOrEmpty(status?.NextUrl))
                    txtInitURL.Text = status.NextUrl;

                strMsgAreaLog = txtLog.Text;
                MessageBox.Show(this, "Batch download finished...", "Web Novel Downloader", MessageBoxButton.OK);
            });
            return strMsgAreaLog;
        }

        private void btnAutoURL_Click(object sender, RoutedEventArgs e)
        {
            txtLog.Clear();
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
                int nMaxPage = string.IsNullOrEmpty(txtPages.Text.Trim()) ? 20 : int.Parse(txtPages.Text.Trim());
                DownloadStatus.MaxPageToDownload = nMaxPage;

                DownloadStatus.ContentsWriter = new StreamWriter(
                    File.Open( AppDomain.CurrentDomain.BaseDirectory + (string.IsNullOrEmpty(txtOutputFileName.Text.Trim()) ? @"Content" : txtOutputFileName.Text.Trim()) + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss") + ".txt", 
                    FileMode.CreateNew, 
                    FileAccess.ReadWrite,
                    FileShare.Read),
                    Encoding.UTF8
                );

                datacontext.DownloadOneURLAndGetNext(this, txtInitURL.Text, false);
            }
        }
    }
}