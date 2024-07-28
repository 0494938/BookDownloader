using BaseBookDownloader;
using MSHTML;
using System;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.ConstrainedExecution;
using System.Security.Policy;
using System.Text.Json;
using System.Text.RegularExpressions;
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

        public string GetNovelName()
        {
            try
            {
                string sNovelName = "";
                this.Dispatcher.Invoke(() => { sNovelName = txtBookName.Text.Trim(); });
                return sNovelName;
            }
            catch (Exception)
            {
                return "";
            }
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
            bool bIsPrettyChecked = false;
            this.Dispatcher.Invoke(() =>
            {
                bIsPrettyChecked = chkboxPrettyHtml.IsChecked == true; ;
            });

            if (bIsPrettyChecked == false)
                txtWebContents.Text = strBody?.Replace("\r\n\r\n\r\n", "\r\n")?.Replace("\r\n\r\n", "\r\n");
            else
                GetBrowserDocAndPrettyToCtrl();

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
                //webBrowser.Refresh();
                webBrowser.CoreWebView2.Reload();
            });
        }

        public string? GetWebDocHtmlBody(string strUrl, bool bWaitOptoin = true)
        {
            _DocContents doc = new _DocContents();
#if false
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
#else
            string html = "";
            this.Dispatcher.Invoke(() =>
            {
                if (webBrowser != null && webBrowser.CoreWebView2 != null && webBrowser.IsLoaded == true)
                {
                    //html = await webView21.ExecuteScriptAsync("document.documentElement.outerHTML");
                    //html = Regex.Unescape(html);
                    //html = html.Remove(0, 1);
                    //html = html.Remove(html.Length - 1, 1);
                    webBrowser.ExecuteScriptAsync("document.documentElement.outerHTML;").ContinueWith(taskHtml =>
                    {
                        //this.Dispatcher.Invoke(() =>
                        //{
                        doc.sHtml = taskHtml.Result;
                        //});
                    });
                }
            });
            int nMaxRetry = 5 * 20;
            int nRetry = 0;
            while (nRetry < nMaxRetry && string.IsNullOrEmpty(doc.sHtml))
            {
                nRetry++;
                Thread.Sleep(200);
            }
            html = doc.sHtml;
            //html = JsonSerializer.Deserialize(html);
            html = Regex.Unescape(html);
            html = html.Remove(0, 1);
            html = html.Remove(html.Length - 1, 1);
            return html;
#endif
        }

        private void AnalysisURL(string strUrl, bool bWaitOptoin = true)
        {
            Debug.WriteLine("btnAnalysisCurURL_Click invoked...");
            WndContextData? datacontext = null;
            this.Dispatcher.Invoke(() => {
                datacontext = App.Current.MainWindow.DataContext as WndContextData;
            });

            Debug.Assert(datacontext != null);

            string? strBody = GetWebDocHtmlBody(strUrl, bWaitOptoin);
            if (!string.IsNullOrEmpty(strBody?.Trim()))
            {
                string strPrettyHtml = PrettyPrintUtil.PrettyPrintHtml(strBody, true, true);

                this.Dispatcher.Invoke(() => {
                    txtWebContents.Text = strPrettyHtml;
                });
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

        public void UpdateChapterMsg(BaseWndContextData datacontext, string msg, int value)
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

                    txtChapter.AppendText(msg + ((value >= 0) ? ("(" + value + "%)") : "") + "\r\n");
                    txtChapter.CaretIndex = txtLog.Text.Length;
                    txtChapter.ScrollToEnd();

                }
                if (value >= 0)
                    txtProgress.GetBindingExpression(ProgressBar.ValueProperty).UpdateTarget();
            });
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

        public void Back(BaseWndContextData datacontext) {
            this.Dispatcher.Invoke(() =>
            {
                webBrowser.GoBack();
            });
        }

        public void LoadHtmlString(string strHtml, string url)
        {
            //this.Dispatcher.Invoke(() => { webBrowser.CoreWebView2.load(strHtml, url); });
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

        public void GetBrowserDocAndPutToCtrl()
        {
            _DocContents doc = new _DocContents();
            new Thread(() =>
            {
                int nMaxRetry = 10 * 60, nRetry = 0;
                bool bLoaded = false;
                this.Dispatcher.Invoke(() =>
                {
                    bLoaded = webBrowser.IsLoaded;
                });
                while (nRetry < nMaxRetry && bLoaded == false)
                {
                    nRetry++;
                    Thread.Sleep(100);
                    this.Dispatcher.Invoke(() =>
                    {
                        bLoaded = webBrowser.IsLoaded;
                    });
                }
                if (bLoaded)
                {
                    this.Dispatcher.Invoke(() =>
                    {
#if false
                        webBrowser.GetSourceAsync().ContinueWith(taskHtml =>
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                doc.sHtml = taskHtml.Result;
                            });
                        });
#else
                        webBrowser.ExecuteScriptAsync("document.documentElement.outerHTML;").ContinueWith(taskHtml =>
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                doc.sHtml = taskHtml.Result;
                            });
                        });
#endif
                    });

                    while (nRetry < nMaxRetry && string.IsNullOrEmpty(doc.sHtml))
                    {
                        nRetry++;
                        Thread.Sleep(100);
                    }
                    if (!string.IsNullOrEmpty(doc.sHtml))
                    {
                        doc.sHtml = Regex.Unescape(doc.sHtml);
                        doc.sHtml = doc.sHtml.Remove(0, 1);
                        doc.sHtml = doc.sHtml.Remove(doc.sHtml.Length - 1, 1);

                        this.Dispatcher.Invoke(() =>
                        {
                            txtWebContents.Text = doc.sHtml?.Replace("\r\n\r\n\r\n", "\r\n")?.Replace("\r\n\r\n", "\r\n");
                        });
                    }
                }
            }).Start();
        }

        public void GetBrowserDocAndPrettyToCtrl()
        {
            _DocContents doc = new _DocContents();
            new Thread(() =>
            {
                int nMaxRetry = 10 * 60, nRetry = 0;
                bool bLoaded = false;
                this.Dispatcher.Invoke(() =>
                {
                    bLoaded = webBrowser.IsLoaded;
                });
                while (nRetry < nMaxRetry && bLoaded == false)
                {
                    nRetry++;
                    Thread.Sleep(100);
                    this.Dispatcher.Invoke(() =>
                    {
                        bLoaded = webBrowser.IsLoaded;
                    });
                }
                if (bLoaded)
                {
                    this.Dispatcher.Invoke(() =>
                    {
#if false
                        webBrowser.GetSourceAsync().ContinueWith(taskHtml =>
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                doc.sHtml = taskHtml.Result;
                            });
                        });
#else
                        webBrowser.ExecuteScriptAsync("document.documentElement.outerHTML;").ContinueWith(taskHtml =>
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                doc.sHtml = taskHtml.Result;
                            });
                        });
#endif
                    });
                    Thread.Sleep(100);

                    while (nRetry < nMaxRetry && string.IsNullOrEmpty(doc.sHtml))
                    {
                        nRetry++;
                        Thread.Sleep(100);
                    }
                    if (!string.IsNullOrEmpty(doc.sHtml))
                    {
                        bool bIgnoreScript = false;
                        bool bIgnoreHead = false;
                        this.Dispatcher.Invoke(() =>
                        {
                            bIgnoreScript = chkboxIgnoreScript.IsChecked ?? false;
                            bIgnoreHead = chkboxIgnoreHeader.IsChecked ?? false;
                        });
                        doc.sHtml = Regex.Unescape(doc.sHtml);
                        doc.sHtml = doc.sHtml.Remove(0, 1);
                        doc.sHtml = doc.sHtml.Remove(doc.sHtml.Length - 1, 1);

                        string strPrettyHtml = PrettyPrintUtil.PrettyPrintHtml(doc.sHtml, bIgnoreScript, bIgnoreHead);

                        this.Dispatcher.Invoke(() =>
                        {
                            txtWebContents.Text = strPrettyHtml;
                        });
                    }

                }
            }).Start();
        }

        public async void InitBrowser()
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

#if false
            webBrowser.LoadCompleted += WebBrowser_LoadCompleted;
#else
            await webBrowser.EnsureCoreWebView2Async(null);
            webBrowser.NavigationCompleted += WebBrowser_NavigationCompleted;
            webBrowser.Loaded += WebBrowser_Loaded;
            webBrowser.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;

            await webBrowser.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.chrome.webview.postMessage(window.document.URL);");
            await webBrowser.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.chrome.webview.addEventListener(\'message\', event => alert(event.data));");
#endif
        }

    }
}