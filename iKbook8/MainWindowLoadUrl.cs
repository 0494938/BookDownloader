using MSHTML;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WebBrowser = System.Windows.Controls.WebBrowser;

namespace iKbook8
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
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if ((datacontext != null))
            {
                datacontext.SiteType = (BatchQueryNovelContents)cmbNovelType.SelectedIndex;
                try
                {
                    webBrowser.Navigate(strUrl);
                    UpdateStatusMsg(datacontext, "Begin to download " + strUrl + "...");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    if (webBrowser == null || webBrowser.Document == null)
                        return;

                    var webBrowserPtr = GetWebBrowserPtr(webBrowser);
                    if (webBrowserPtr.ReadyState != SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
                        return;
                }
            }
        }

        private void DownloadOneURLAndGetNext(string strURL)
        {
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if ((datacontext != null))
            {
                try
                {
                    dictDownloadStatus[strURL] = new DownloadStatus { DownloadFinished =false , URL = strURL, NextUrl="", StartTime=DateTime.Now, Depth=0, ThreadNum= dictDownloadStatus.Count+1};
                    webBrowser.Navigate(strURL);
                    UpdateStatusMsg(datacontext, "Begin to download " + strURL + "...");
                    WaitFinish(strURL);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    if (webBrowser == null || webBrowser.Document == null)
                    {
                        Debug.Assert(false);
                        WaitFinish(strURL);
                    }

                    var webBrowserPtr = GetWebBrowserPtr(webBrowser);
                    if (webBrowserPtr.ReadyState != SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
                        return;
                }
            }
        }

        void WaitFinish(string strURL)
        {
            DownloadStatus status = dictDownloadStatus[strURL];
            if (status.DownloadFinished == false)
            {
                Task.Factory.StartNew(() => Thread.Sleep(400))
                .ContinueWith((t) =>
                {
                    status.Depth = status.Depth + 1;
                    WaitFinish(strURL);
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                status.Depth = status.Depth - 1;

                Debug.WriteLine($"{strURL} download finished, begin analysis...");
                Debug.Assert(webBrowser != null || webBrowser.Document != null);

                IHTMLDocument2? hTMLDocument2 = webBrowser.Document as IHTMLDocument2;
                IHTMLElement? body = hTMLDocument2?.body as IHTMLElement;
                string? strBody = body?.outerHTML ?? "";
                //string? strHtml = hTMLDocument2.boday
                txtWebContents.Text = strBody;
                WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
                AnalysisHtmlBody(ref datacontext, ref strBody, true, status);
                if (status.ThreadNum < DownloadStatus.ThreadMax && !string.IsNullOrEmpty(status.NextUrl))
                {
                    DownloadOneURLAndGetNext(status.NextUrl);
                }
                else {
                    if(!string.IsNullOrEmpty(status.NextUrl))
                        txtInitURL.Text = status.NextUrl;
                    DownloadStatus.ContentsWriter = null;
                }
            }
        }


        private void MainFrameWebLoadCompleted(object sender, NavigationEventArgs e)
        {
            Debug.WriteLine("MainFrameWebLoadCompleted invoked...");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if ((datacontext!=null))
            {
                var browser = sender as WebBrowser;

                if (browser == null || browser.Document == null)
                    return;

                dynamic document = browser.Document;

                var webBrowserPtr = GetWebBrowserPtr(webBrowser);
                if (webBrowserPtr.ReadyState != SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
                    return;

                //int nStep = 0;
                dynamic script = null;
                try
                {
                    script = document.createElement("script");
                    script.type = @"text/javascript";
                    script.text = @"window.onerror = function(msg,url,line){return true;}";
                    document.head.appendChild(script);
                }
                catch (Exception )
                {
                }

                try
                {
                    datacontext.PageLoaded = true;
                    btnAnalysisCurURL.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                    if (dictDownloadStatus.ContainsKey(e.Uri.ToString()))
                    {
                        DownloadStatus status = dictDownloadStatus[e.Uri.ToString()];
                        status.DownloadFinished = true;
                        status.FinishTime = DateTime.Now;
                    }
                    else
                    {
                        AnalysisCurURL();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        private void btnAutoURL_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("btnAutoURL_Click invoked...");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if (datacontext != null)
            {
                datacontext.SiteType = (BatchQueryNovelContents)cmbNovelType.SelectedIndex;
                dictDownloadStatus.Clear();
                txtAggregatedContents.Clear();
                string strNextPage = "";
                int nMaxPage = string.IsNullOrEmpty(txtPages.Text.Trim()) ? 100 : int.Parse(txtPages.Text.Trim());
                DownloadStatus.ThreadMax = nMaxPage;
                DownloadStatus.ContentsWriter = new StreamWriter(File.Open(AppDomain.CurrentDomain.BaseDirectory + (string.IsNullOrEmpty(txtOutputFileName.Text.Trim())? @"Content": txtOutputFileName.Text.Trim()) + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss") + ".txt", FileMode.CreateNew),
                    //Encoding.GetEncoding("iso-8859-1")
                    Encoding.UTF8
                    );

                DownloadOneURLAndGetNext(txtInitURL.Text);
            }
        }
    }
}