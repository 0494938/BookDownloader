using BaseBookDownloader;
using System.Diagnostics;
using System.IO;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using VideoLibrary;

namespace WpfStreamDownloader
{

    public partial class WpfStreamMainWindow : Window, IBaseMainWindow
    {
        public void UpdateStatusMsg(BaseWndContextData? datacontext, string msg, int value)
        {
            Debug.WriteLine(msg);
            if (datacontext != null)
            {
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
        }

        public void UpdateStreamMsg(BaseWndContextData? datacontext, string msg, int value)
        {
            Debug.WriteLine(msg);
            if (datacontext != null)
            {
                datacontext.StartBarMsg = msg;
                if (value >= 0)
                    datacontext.ProcessBarValue = value;
                txtStatus.Dispatcher.Invoke(() =>
                {
                    txtStatus.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
                    if (!string.IsNullOrEmpty(msg))
                    {
                        txtStreamLog.AppendText(msg + ((value >= 0) ? ("(" + value + "%)") : "") + "\r\n");
                        txtStreamLog.CaretIndex = txtLog.Text.Length;
                        txtStreamLog.ScrollToEnd();
                    }
                    if (value >= 0)
                        txtProgress.GetBindingExpression(ProgressBar.ValueProperty).UpdateTarget();
                });
            }
        }

        public void Back(BaseWndContextData?     datacontext)
        {
            this.Dispatcher.Invoke(() =>
            {
                webBrowser.GoBack();
            });
        }

        public void LoadHtmlString(string strHtml, string url)
        {
            //this.Dispatcher.Invoke(() => { webBrowser.CoreWebView2.load(strHtml, url); });
        }

        public void UpdateStatusProgress(BaseWndContextData? datacontext, int value)
        {
            if (datacontext != null)
            {
                datacontext.ProcessBarValue = value;
                txtProgress.Dispatcher.Invoke(() =>
                {
                    txtProgress.GetBindingExpression(ProgressBar.ValueProperty).UpdateTarget();
                });
            }
        }

        public void UpdateChapterMsg(BaseWndContextData? datacontext, string msg, int value)
        {
            UpdateStatusMsg(datacontext, msg, value);
        }

        public string GetLogContents()
        {
            string strLog = "";
            this.Dispatcher.Invoke(() => {
                strLog = txtLog.Text.Replace("\r", "").Replace("\n", "\r\n");
            });
            return strLog;
        }

        public void RefreshPage()
        {
            webBrowser.Reload();
        }

        public void LoadUiUrl(BaseWndContextData? datacontext, string strURL)
        {
            throw new NotImplementedException();
        }

        public void UpdateNextPageButton()
        {
            //throw new NotImplementedException();
        }

        public void UpdateWebBodyOuterHtml(string? strHtml)
        {
            bool bIsPrettyChecked = true;


            if (bIsPrettyChecked == false) { 
                txtWebContents.Text = strHtml?.Replace("\r\n\r\n\r\n", "\r\n")?.Replace("\r\n\r\n", "\r\n"); 
            }else { 
                GetBrowserDocAndPrettyToCtrl(strHtml); 
            }
                

        }

        public void GetBrowserDocAndPrettyToCtrl(string ? strHtml=null, string ? strPrettyFileName=null)
        {
            _DocContents doc = new _DocContents();
            new Thread(() =>
            {
                if (string.IsNullOrEmpty(strHtml)){
                    string strPrettyHtml = PrettyPrintUtil.PrettyPrintHtml(doc.sHtml, false, false, false);
                    this.Dispatcher.Invoke(() =>
                    {
                        txtWebContents.Text = strPrettyHtml;
                    });
                    return;
                }
                
                //Need get Contents from Browser.
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
                        webBrowser.ExecuteScriptAsync("document.documentElement.outerHTML;").ContinueWith(taskHtml =>
                        {
                            //this.Dispatcher.Invoke(() =>{
                            doc.sHtml = taskHtml.Result;
                            //});
                        });

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
                        string strPrettyHtml = PrettyPrintUtil.PrettyPrintHtml(doc.sHtml, bIgnoreScript, bIgnoreHead);
                        this.Dispatcher.Invoke(() =>
                        {
                            txtWebContents.Text = strPrettyHtml;
                        });
                    }

                }
            }).Start();
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
        public void UpdateInitPageButton()
        {
            throw new NotImplementedException();
        }

        public void UpdateAutoDownloadPageButton()
        {
            throw new NotImplementedException();
        }

        public void UpdateAnalysisPageButton()
        {
            throw new NotImplementedException();
        }

        public void UpdateNovelName(string sNovelName)
        {
            //throw new NotImplementedException();
        }
       
        public void UpdateAnalysizedContents(string strContents)
        {
            //throw new NotImplementedException();
        }

        public void UpdateAggragatedContents(string strContents)
        {
            //throw new NotImplementedException();
        }

        public void UpdateAggragatedContentsWithLimit(string strContents)
        {
            throw new NotImplementedException();
        }

        public bool isWebBrowserEmpty()
        {
            throw new NotImplementedException();
        }

        public bool isWebPageLoadComplete(string strURL)
        {
            throw new NotImplementedException();
        }

        public void NovelTypeChangeEvent(BaseWndContextData? datacontext, int nIndex)
        {
            if (txtInitURL != null && datacontext!=null)
            {
                //Debug.WriteLine("Select Combox Index : " + cmbNovelType.SelectedIndex);
                UpdateStatusMsg(App.Current.MainWindow.DataContext as WndContextData, "Select Combox Index : " + cmbNovelType.SelectedIndex + " " + ((BatchQueryNovelContents)cmbNovelType.SelectedIndex).ToCode(), -1);
                if (txtInitURL != null)
                {
                    Debug.WriteLine("Select Combox Index : " + cmbNovelType.SelectedIndex);
                    txtInitURL.Text = datacontext.GetDefaultUrlByIdx(cmbNovelType.SelectedIndex);

                    if (string.IsNullOrEmpty(txtCurURL.Text.Trim()))
                    {
                        txtCurURL.Text = txtInitURL.Text;
                    }
                }
            }
        }

        public string BatchDownloadNotified(BaseWndContextData? datacontext, DownloadStatus status, string sDownloadFileName)
        {
            throw new NotImplementedException();
        }

        public string GetNovelName()
        {
            throw new NotImplementedException();
        }

        public bool DownloadFile(BaseWndContextData? datacontext, string sVedioUrl)
        {
            //WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if (datacontext != null)
            {
                try
                {
                    //var VedioUrl = "https://www.youtube.com/embed/" + "0pPPXeXKdfg" + ".mp4";
                    YouTube youTube = YouTube.Default;
                    var video = youTube.GetVideo(sVedioUrl);
                    string strFileName = AppDomain.CurrentDomain.BaseDirectory + video.FullName;

                    DateTime start = DateTime.Now;
                    UpdateStatusMsg(datacontext, "Download Start: " + sVedioUrl.ToString() + " ...", 0);
                    System.IO.File.WriteAllBytes(strFileName, video.GetBytes());
                    TimeSpan span = DateTime.Now - start;
                    UpdateStatusMsg(datacontext, "Download Finished and save to " + strFileName + " in " + span.TotalSeconds + "seconds. ", 100);
                    return true;
                }
                catch (Exception e) {
                    UpdateStatusMsg(datacontext, "Download Finished with Error " + e.ToString(), 100);
                    return false;
                }
            }

            return false;
        }

        public bool DownloadFile(BaseWndContextData? datacontext, System.Collections.Generic.List<string> listUrls)
        {
            throw new NotImplementedException();
        }
    }
}