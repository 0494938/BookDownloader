using MSHTML;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace BookDownloader
{
    public partial class MainWindow : Window
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

        private void DownloadOneURLAndGetNext(WndContextData? datacontext, MainWindow wndMain,  string strURL)
        {
            if ((datacontext != null) && !datacontext.UnloadPgm)
            {
                try
                {
                    dictDownloadStatus[strURL] = new DownloadStatus { DownloadFinished =false , URL = strURL, NextUrl="", StartTime=DateTime.Now, Depth=0, ThreadNum= dictDownloadStatus.Count+1};
                    webBrowser.Dispatcher.Invoke(() =>
                    {
                        datacontext.PageLoaded = false;
                        datacontext.NextLinkAnalysized = false;
                        btnAnalysisCurURL.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                        btnNextPage.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                        datacontext.PgmNaviUrl = strURL;
                        webBrowser.Navigate(strURL);
                    });
                    UpdateStatusMsg(datacontext, strURL + " : Begin to download ...", (int)((100.0 / DownloadStatus.ThreadMax * (dictDownloadStatus[strURL].ThreadNum - 1))));
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

        void WaitFinishForNext(WndContextData? datacontext, MainWindow wndMain, string strURL, bool bSilenceMode=false)
        {
#if false
            DownloadStatus status = dictDownloadStatus[strURL];
            if (status.DownloadFinished == false)
            {
                this.Dispatcher.Invoke(() =>
                {
                    Task.Factory.StartNew(() => Thread.Sleep(400))
                    .ContinueWith((t) =>
                    {
                        status.Depth = status.Depth + 1;
                        WaitFinish(strURL);
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                });
            }
            else
            {
                status.Depth = status.Depth - 1;

                Debug.WriteLine($"{strURL} : Download Finished, Begin Analysis ...");
                Debug.Assert(webBrowser != null || webBrowser?.Document != null);

                IHTMLDocument2? hTMLDocument2 = webBrowser.Document as IHTMLDocument2;
                IHTMLElement? body = hTMLDocument2?.body as IHTMLElement;
                string? strBody = body?.outerHTML ?? "";
                //string? strHtml = hTMLDocument2.boday
                txtWebContents.Text = strBody;
                WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
                AnalysisHtmlBody(datacontext, true, strURL, strBody, true, status);
            }
#else
            DownloadStatus status = dictDownloadStatus[strURL];

            Thread thread = new Thread(() => WaitAndLaunchAnalsysi(datacontext, wndMain, strURL, bSilenceMode, status));
            thread.Start();

#endif
        }

#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
        public void WaitAndLaunchAnalsysi(WndContextData? datacontext, MainWindow wndMain, string strURL, bool bSilenceMode, DownloadStatus status )
        {
            while (status.DownloadFinished == false && !datacontext.UnloadPgm)
            {
                Thread.Sleep(200);
            }

            status.Depth = status.Depth - 1;

            Debug.WriteLine($"{strURL} : Download Finished, Begin Analysis ...");
            Debug.Assert(webBrowser != null || webBrowser?.Document != null);

            if (!datacontext.UnloadPgm)
            {
                try
                {
                    wndMain.Dispatcher.Invoke(() =>
                    {
                            IHTMLDocument2? hTMLDocument2 = webBrowser.Document as IHTMLDocument2;
                            IHTMLElement? body = hTMLDocument2?.body as IHTMLElement;
                            string? strBody = body?.outerHTML ?? "";
                            //string? strHtml = hTMLDocument2.boday
                            txtWebContents.Text = strBody;
                            AnalysisHtmlBody(datacontext, true, strURL, strBody, true, status);
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.StackTrace);
                }
            }
        }
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。

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
                DownloadStatus.ThreadMax = nMaxPage;

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