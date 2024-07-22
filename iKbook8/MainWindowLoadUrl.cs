using BaseBookDownload;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace BookDownloader
{
    public partial class WPFMainWindow : Window, IBaseMainWindow
    {
        Dictionary<string, DownloadStatus> dictDownloadStatus = new Dictionary<string, DownloadStatus>();

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
            dictDownloadStatus.Clear();
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if ((datacontext != null))
            {
                datacontext.PageLoaded = false;
                datacontext.NextLinkAnalysized = false;
                btnAnalysisCurURL.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                btnNextPage.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                txtWebContents.Text = "";
                txtAnalysizedContents.Text = "";

                datacontext.SiteType = (BatchQueryNovelContents)cmbNovelType.SelectedIndex;
                try
                {
                    datacontext.PgmNaviUrl = strUrl;
                    webBrowser.Navigate(strUrl);
                    UpdateStatusMsg(datacontext, strUrl + " : Begin to download ...", 0);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    if (webBrowser == null || webBrowser.Document == null)
                        return;

                    SHDocVw.WebBrowser? webBrowserPtr = GetWebBrowserPtr(webBrowser);
                    Debug.WriteLine(strUrl + " : Status <" + webBrowserPtr?.ReadyState.ToString() + ">");
                    if (webBrowserPtr?.ReadyState != SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
                        return;
                }
            }
        }

        private void DownloadOneURLAndGetNext(BaseWndContextData? datacontext, IBaseMainWindow wndMain,  string strURL)
        {
            if ((datacontext != null) && !datacontext.UnloadPgm)
            {
                try
                {
                    dictDownloadStatus[strURL] = new DownloadStatus { DownloadFinished =false , URL = strURL, NextUrl="", StartTime=DateTime.Now, PageNum= dictDownloadStatus.Count+1};
                    webBrowser.Dispatcher.Invoke(() =>
                    {
                        datacontext.PageLoaded = false;
                        datacontext.NextLinkAnalysized = false;
                        btnAnalysisCurURL.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                        btnNextPage.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                        datacontext.PgmNaviUrl = strURL;
                        webBrowser.Navigate(strURL);
                    });
                    UpdateStatusMsg(datacontext, strURL + " : Begin to download ...", (int)((100.0 / DownloadStatus.MaxPageToDownload * (dictDownloadStatus[strURL].PageNum - 1))));
                    WaitFinishForNext(datacontext, wndMain, strURL, true);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    if (webBrowser == null || webBrowser.Document == null)
                    {
                        Debug.Assert(false);
                        WaitFinishForNext(datacontext,wndMain, strURL, true);
                    }

                    SHDocVw.WebBrowser? webBrowserPtr = GetWebBrowserPtr(webBrowser);
                    Debug.WriteLine(strURL + " : Status <" + webBrowserPtr?.ReadyState.ToString() + ">");
                    if (webBrowserPtr?.ReadyState != SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
                        return;
                }
            }
            else
            {
                Debug.Assert(true);
            }
        }

        void WaitFinishForNext(BaseWndContextData datacontext, IBaseMainWindow wndMain, string strURL, bool bSilenceMode=false)
        {
            DownloadStatus status = dictDownloadStatus[strURL];

            try
            {
                Thread thread = new Thread(() => WaitAndLaunchAnalsysi(datacontext, wndMain, strURL, bSilenceMode, status));
                thread.Start();
            }
            catch (TaskCanceledException)
            {
                //ignore TaskCanceledException
            }
        }

#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning disable CS8604 // Null 参照引数の可能性があります。
        public void WaitAndLaunchAnalsysi(BaseWndContextData datacontext, IBaseMainWindow wndMain, string strURL, bool bSilenceMode, DownloadStatus status )
        {
            const int MAX_RETRY = 60 * 5 * 2; //wait loading up to 2 minutes.
            int nWaitRetry = 0;
            while (status.DownloadFinished == false && !datacontext.UnloadPgm && nWaitRetry < MAX_RETRY)
            {
                Thread.Sleep(200);
                nWaitRetry++;
            }

            //status.Depth = status.Depth - 1;

            Debug.WriteLine($"{strURL} : Download Finished, Begin Analysis ...");
            Debug.Assert(webBrowser != null || webBrowser?.Document != null);

            if (!datacontext.UnloadPgm)
            {
                try
                {
                    string? strBody = wndMain.GetWebDocHtmlBody(strURL);
                    wndMain.UpdateWebBodyOuterHtml(strBody);
                    AnalysisHtmlBody(datacontext, true, strURL, strBody, true, status);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.StackTrace);
                }
            }
        }
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning restore CS8604 // Null 参照引数の可能性があります。

        private void btnAutoURL_Click(object sender, RoutedEventArgs e)
        {
            txtLog.Clear();
            dictDownloadStatus.Clear();
            Debug.WriteLine("btnAutoURL_Click invoked...");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if (datacontext != null)
            {
                datacontext.BackGroundNotRunning = false;
                btnInitURL.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                btnAutoDownload.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                datacontext.SiteType = (BatchQueryNovelContents)cmbNovelType.SelectedIndex;
                dictDownloadStatus.Clear();
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

                DownloadOneURLAndGetNext(datacontext,this, txtInitURL.Text);
            }
        }
    }
}