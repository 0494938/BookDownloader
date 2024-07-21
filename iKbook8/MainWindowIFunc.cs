using BaseBookDownload;
using MSHTML;
using System;
using System.Diagnostics;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;

namespace BookDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class WPFMainWindow : Window, IBaseMainWindow
    {
        public void UpdateNextPageButton() {
            this.Dispatcher.Invoke(() =>
            {
                btnNextPage.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
            });
        }

        public void UpdateInitPageButton()
        {
            this.Dispatcher.Invoke(() =>
            {
                this.btnInitURL.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
            });
        }

        public void UpdateAutoDownloadPageButton()
        {
            this.Dispatcher.Invoke(() =>
            {
                this.btnAutoDownload.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
            });
        }
        public void UpdateAnalysisPageButton()
        {
            //throw new NotImplementedException();
            this.Dispatcher.Invoke(() =>
            {
                this.btnAnalysisCurURL.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
            });
        }
        public void UpdateAnalysizedContents(string ? strContents)
        {
            this.Dispatcher.Invoke(() =>
            {
                txtAnalysizedContents.Text = strContents;
            });
        }
        
        public void UpdateAggragatedContents(string strContents)
        {
            this.Dispatcher.Invoke(() =>
            {
                txtAggregatedContents.Text += strContents;
                txtAggregatedContents.ScrollToEnd();
            });
        }
        
        public void UpdateAggragatedContentsWithLimit(string strContents)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (txtAggregatedContents.Text.Length > 1024 * 64)
                    txtAggregatedContents.Text = strContents;
                else
                    txtAggregatedContents.Text += strContents;
                txtAggregatedContents.ScrollToEnd();
            });
        }
        public void UpdateWebBodyOuterHtml(string? strBody)
        {
            this.Dispatcher.Invoke(() =>
            {
                txtWebContents.Text = strBody;
            });
        }

        public void UpdateNextUrl(string url)
        {
            this.Dispatcher.Invoke(() =>
            {
                txtNextUrl.Text = url;
            });
        }

        public void UpdateCurUrl(string url)
        {
            this.Dispatcher.Invoke(() =>
            {
                txtCurURL.Text = url;
            });
        }

        public void AnalysisHtmlBody(BaseWndContextData? datacontext, bool bWaitOption, string strURL, string strBody, bool bSilenceMode = false, DownloadStatus? status = null)
        {
            Debug.Assert(datacontext != null);
            try
            {
                Thread thread = new Thread(() => AnalysisHtmlBodyThreadFunc(datacontext, this, strURL, strBody, bSilenceMode, status));
                thread.Start();
            }
            catch (TaskCanceledException)
            {
                //ignore TaskCanceledException
            }
            //AnalysisHtmlBodyThreadFunc(datacontext, this, strURL, strBody, bSilenceMode, status);
        }

        public string? GetWebDocHtmlBody(string strUrl, bool bWaitOptoin = true)
        {

            bool bFailed = false;
            this.Dispatcher.Invoke(() =>
            {
                if (webBrowser == null || webBrowser.Document == null)
                    bFailed = true;
            });

            string? strBody = null;
            if(!bFailed)
            {
                this.Dispatcher.Invoke(() =>
                {
                    var serviceProvider = (IServiceProvider)webBrowser.Document;
                    if (serviceProvider != null)
                    {
                        Guid iid = typeof(SHDocVw.WebBrowser).GUID;
                        SHDocVw.WebBrowser? webBrowserPtr =
                            //serviceProvider.QueryService(SID_SWebBrowserApp, ref iid) as SHDocVw.WebBrowser;
                            GetWebBrowserPtr(webBrowser);
                        if (webBrowserPtr != null)
                        {
                            webBrowserPtr.NewWindow2 += webBrowser1_NewWindow2;
                            webBrowserPtr.NewWindow3 += webBrowser1_NewWindow3;
                        }
                    }

                    IHTMLDocument2? hTMLDocument2 = webBrowser.Document as IHTMLDocument2;
                    IHTMLElement? body = hTMLDocument2?.body as IHTMLElement;
                    strBody = body?.outerHTML;
                });

            }
            return strBody;
        }

        private void AnalysisURL(string strUrl, bool bWaitOptoin = true)
        {
            Debug.WriteLine("btnAnalysisCurURL_Click invoked...");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            Debug.Assert(datacontext != null);

            string? strBody = GetWebDocHtmlBody(strUrl, bWaitOptoin);
            if (!string.IsNullOrEmpty(strBody?.Trim()))
            {
                txtWebContents.Text = strBody;
                AnalysisHtmlBody(datacontext, bWaitOptoin, strUrl, strBody);
            }
        }

        private void webBrowser1_NewWindow2(ref object ppDisp, ref bool Cancel)
        {
            // Handle the event.  
            Cancel = true;
        }

        private void webBrowser1_NewWindow3(ref object ppDisp, ref bool Cancel, uint dwFlags, string bstrUrlContext, string bstrUrl)
        {
            // Handle the event.  
            Cancel = true;
        }

        public void UpdateStatusMsg(BaseWndContextData datacontext, string msg, int value)
        {

            Debug.WriteLine(msg);
            datacontext.StartBarMsg = msg;
            if (value >= 0)
                datacontext.ProcessBarValue = value;
            txtStatus.Dispatcher.Invoke(() =>
            {
                txtStatus.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
                if (!string.IsNullOrEmpty(msg))
                {
                    txtLog.AppendText(msg + ((value >= 0) ? ("(" + value + "%)") : "") + "\r\n");
                    txtLog.CaretIndex = txtLog.Text.Length;
                    txtLog.ScrollToEnd();
                }
                if (value >= 0)
                    txtProgress.GetBindingExpression(ProgressBar.ValueProperty).UpdateTarget();
            });
        }

        public void UpdateStatusProgress(BaseWndContextData datacontext, int value)
        {
            datacontext.ProcessBarValue = value;
            //txtProgress.GetBindingExpression(ProgressBar.ValueProperty).UpdateTarget();
            txtProgress.Dispatcher.Invoke(() =>
            {
                txtProgress.GetBindingExpression(ProgressBar.ValueProperty).UpdateTarget();
            });
        }
    }
}