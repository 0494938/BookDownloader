using BaseBookDownloader;
using MSHTML;
using System;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;

namespace WpfIEBookDownloader
{
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
            /*
            this.Dispatcher.Invoke(() =>
            {
                this.btnAnalysisCurURL.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
            });
            */
        }
        public void UpdateAnalysizedContents(string ? strContents)
        {
            this.Dispatcher.Invoke(() =>
            {
                txtAnalysizedContents.Text = strContents;
            });
        }

        public string GetLogContents()
        {
            string strLog = "";
            this.Dispatcher.Invoke(() => {
                strLog = txtLog.Text.Replace("\r", "").Replace("\n", "\r\n");
            });
            return strLog;
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

        public void UpdateInitUrl(string url)
        {
            this.Dispatcher.Invoke(() => 
            { 
                this.txtInitURL.Text = url;
            });
        }

        public void UpdateCurUrl(string url)
        {
            this.Dispatcher.Invoke(() =>
            {
                txtCurURL.Text = url;
            });
        }

        public void RefreshPage()
        {
            this.Dispatcher.Invoke(() => {
                webBrowser.Refresh();
            });
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
                datacontext.AnalysisHtmlBody(this, bWaitOptoin, strUrl, strBody);
            }
        }

        private void webBrowser1_NewWindow2(ref object ppDisp, ref bool Cancel)
        {
            Cancel = true;
        }

        private void webBrowser1_NewWindow3(ref object ppDisp, ref bool Cancel, uint dwFlags, string bstrUrlContext, string bstrUrl)
        {
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

        public void InitBrowser()
        {

            //webBrowser.LoadingStateChanged += Browser_LoadingStateChanged;
            //webBrowser.FrameLoadStart += (sender, args) =>
            //{
            //    //MainFrame has started to load, too early to access the DOM, you can add event listeners for DOMContentLoaded etc.
            //    Debug.WriteLine("browser.FrameLoadStart[Frame=" + (string.IsNullOrEmpty(args.Frame.Name) ? "#NONAME" : args.Frame.Name) + "] entered with (IsLoading = " + browser.IsLoading + ")...");
            //    UpdateStatusMsg(datacontext, "Start Frame Load : " + args.Url.ToString() + " ...", 0);

            //    if (args.Frame.IsMain)
            //    {
            //        //const string script = "document.addEventListener('DOMContentLoaded', function(){ alert('DomLoaded'); });";
            //        //args.Frame.ExecuteJavaScriptAsync(script);
            //    }
            //};
            //webBrowser.FrameLoadEnd += new EventHandler<CefSharp.FrameLoadEndEventArgs>(Browser_FrameLoadComplete);
            //webBrowser.AddressChanged += new EventHandler<CefSharp.AddressChangedEventArgs>(Browser_AddressChanged);
            //webBrowser.IsBrowserInitializedChanged += new EventHandler(Browser_IsBrowserInitializedChanged);
            //webBrowser.JavascriptMessageReceived += new EventHandler<CefSharp.JavascriptMessageReceivedEventArgs>(Browser_JavascriptMessageReceived);
            //webBrowser.LocationChanged += new EventHandler(Browser_LocationChanged);
            //webBrowser.RegionChanged += new EventHandler(Browser_RegionChanged);

            //webBrowser.LoadError += new EventHandler<CefSharp.LoadErrorEventArgs>(Browser_LoadError);
            //webBrowser.TitleChanged += new EventHandler<CefSharp.TitleChangedEventArgs>(Browser_TitleChanged);

            //webBrowser.ControlAdded += new ControlEventHandler(Browser_ControlAdded);
            //webBrowser.ControlRemoved += new ControlEventHandler(Browser_ControlRemoved);
            //webBrowser.BindingContextChanged += new EventHandler(Browser_BindingContextChanged);
            webBrowser.LoadCompleted += WebBrowser_LoadCompleted;
            webBrowser.Loaded += WebBrowser_Loaded;
        }

    }
}