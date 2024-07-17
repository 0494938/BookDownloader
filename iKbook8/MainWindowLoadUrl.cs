using MSHTML;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WebBrowser = System.Windows.Controls.WebBrowser;

namespace iKbook8
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private void btnInitURL_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("btnInitURL_Click invoked...");
            ClickBtntnInitURL(txtInitURL.Text);
        }

        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("btnNextPage_Click invoked...");
            ClickBtntnInitURL(txtCurURL.Text);
        }

        private void ClickBtntnInitURL(string strUrl)
        {
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if ((datacontext != null))
            {
                try
                {
                    webBrowser.Navigate(strUrl);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    if (webBrowser == null || webBrowser.Document == null)
                        return;

                    var webBrowserPtr = GetWebBrowserPtr(webBrowser);

                    /*dynamic document = webBrowser.Document;

                    if (document.readyState != "complete")
                        return;
                    */

                    if (webBrowserPtr.ReadyState != SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
                        return;
                    
                }
            }
        }

        class DownloadStatus {
            public bool DownloadFinished { get; set; } = false;
            public string URL { get; set; }
            public string NextUrl { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime FinishTime { get; set; }
            public int Depth { get; set; } = 0;
            public int ThreadNum { get; set; }
            public static int ThreadMax { get; set; }
        }

        Dictionary<string, DownloadStatus> dictDownloadStatus = new Dictionary<string, DownloadStatus>();
        private void DownloadOneURLAndGetNext(string strURL)
        {
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if ((datacontext != null))
            {
                try
                {
                    dictDownloadStatus[strURL] = new DownloadStatus { DownloadFinished =false , URL = strURL, NextUrl="", StartTime=DateTime.Now, Depth=0, ThreadNum= dictDownloadStatus.Count+1};
                    webBrowser.Navigate(strURL);
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

                    /*dynamic document = webBrowser.Document;

                    if (document.readyState != "complete")
                        return;
                    */

                    if (webBrowserPtr.ReadyState != SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
                        WaitFinish(strURL);

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
                //IHTMLDocument? hTMLDocument = webBrowser.Document as IHTMLDocument;

                IHTMLElement? body = hTMLDocument2?.body as IHTMLElement;
                string? strBody = body?.outerHTML ?? "";
                //string? strHtml = hTMLDocument2.boday
                txtWebContents.Text = body?.outerHTML;
                WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
                AnalysisHtmlBody(ref datacontext, ref strBody, true, status);
                if(status.ThreadNum < DownloadStatus.ThreadMax && !string.IsNullOrEmpty(status.NextUrl))
                {
                    DownloadOneURLAndGetNext(status.NextUrl);
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

                if (document.readyState != "complete")
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
                catch (Exception ex)
                {
                    //Debug.WriteLine(ex);
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
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        private void btnAutoURL_Click(object sender, RoutedEventArgs e)
        {
            dictDownloadStatus.Clear();
            string strNextPage = "";
            int nMaxPage = string.IsNullOrEmpty(txtPages.Text.Trim()) ? 10 : int.Parse(txtPages.Text.Trim());
            DownloadOneURLAndGetNext(txtInitURL.Text);
            DownloadStatus.ThreadMax = nMaxPage;
            //int i = 0;
            //while (i <= nMaxPage)
            //{
            //    i++;
            //    if (i == 1)
            //    {
            //        strNextPage = DownloadOneURLAndGetNext(txtInitURL.Text);
            //        Debug.WriteLine("Downloadeed <" + txtInitURL.Text + "> and try to get next as <" + strNextPage + ">...");
            //    }
            //    else
            //    {
            //        string sNextPage = DownloadOneURLAndGetNext(strNextPage);
            //        Debug.WriteLine("Downloadeed <" + strNextPage + "> and try to get next as <" + sNextPage + ">...");
            //        strNextPage = sNextPage;
            //    }

            //}
        }
    }
}