using BaseBookDownloader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace WpfStreamDownloader
{
#pragma warning disable SYSLIB0014 // 型またはメンバーが旧型式です
#pragma warning disable CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。
    public partial class WpfStreamMainWindow : Window, IBaseMainWindow
    {
        public void UpdateStatusMsg(BaseWndContextData? datacontext, string msg, int value)
        {
            Debug.WriteLine(msg);
            try
            {
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
            catch (TaskCanceledException)
            {

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
                        txtLog.AppendText(msg + ((value >= 0) ? ("(" + value + "%)") : "") + "\r\n");
                        txtLog.CaretIndex = txtLog.Text.Length;
                        txtLog.ScrollToEnd();
                    }
                    if (value >= 0)
                        txtProgress.GetBindingExpression(ProgressBar.ValueProperty).UpdateTarget();
                });
            }
        }

        public void Back(BaseWndContextData? datacontext)
        {
            this.Dispatcher.Invoke(() =>
            {
                webBrowser.GoBack();
            });
        }

        public void LoadHtmlString(string strHtml, string url, BaseWndContextData? datacontext = null)
        {
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

        public string GetLogContents(BaseWndContextData? datacontext = null)
        {
            string strLog = "";
            this.Dispatcher.Invoke(() => {
                strLog = txtLog.Text.Replace("\r", "").Replace("\n", "\r\n");
            });
            return strLog;
        }

        public void RefreshPage(BaseWndContextData? datacontext = null)
        {
            webBrowser.Reload();
        }

        public void LoadUiUrl(BaseWndContextData? datacontext, string strURL)
        {
            if (datacontext != null)
            {
                webBrowser.Dispatcher.Invoke(() =>
                {
                    datacontext.PageLoaded = false;
                    datacontext.NextLinkAnalysized = false;
                    //btnAnalysisCurURL.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                    UpdateNextPageButton();
                    UpdateInitPageButton();
                    UpdateAutoDownloadPageButton();

                    datacontext.PgmNaviUrl = strURL;
                    webBrowser.CoreWebView2.Navigate(strURL);
                });
            }
        }

        public void UpdateWebBodyOuterHtml(string? strHtml, BaseWndContextData? datacontext = null)
        {
            bool bIsPrettyChecked = true;

            bool bOutputHtml = false;
            this.Dispatcher.Invoke(() => { return bOutputHtml = chkShowHtml.IsChecked??false; });
            if (bOutputHtml)
            {
                if (bIsPrettyChecked == false)
                {
                    this.Dispatcher.Invoke(() => { txtWebContents.Text = strHtml?.Replace("\r\n\r\n\r\n", "\r\n")?.Replace("\r\n\r\n", "\r\n"); });
                }
                else
                {
                    GetBrowserDocAndPrettyToCtrl(strHtml);
                }
            }
        }

        public void GetBrowserDocAndPrettyToCtrl(string ? strHtml=null, string ? strPrettyFileName=null)
        {
            bool bOutputHtml = false;
            this.Dispatcher.Invoke(() => { return bOutputHtml = chkShowHtml.IsChecked ?? false; });
            _DocContents doc = new _DocContents();
            if (bOutputHtml)
            {
                new Thread(() =>
                {
                    if (string.IsNullOrEmpty(strHtml))
                    {
                        string strPrettyHtml = PrettyPrintUtil.PrettyPrintHtml(doc.sHtml, false, false, false);
                        this.Dispatcher.Invoke(() =>
                        {
                            txtWebContents.Text = strPrettyHtml;
                        });
                        return;
                    }

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
                                doc.sHtml = taskHtml.Result;
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
        }

        public void UpdateNextUrl(string url, BaseWndContextData? datacontext = null)
        {
            this.Dispatcher.Invoke(() =>
            {
                txtNextUrl.Text = url;
            });
        }

        public void UpdateInitUrl(string url, BaseWndContextData? datacontext = null)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.txtInitURL.Text = url;
            });
        }

        public void UpdateCurUrl(string url, BaseWndContextData? datacontext = null)
        {
            if (!string.IsNullOrEmpty(url))
            {
                Debug.WriteLine("UpdateCurUrl : " + url);
                this.Dispatcher.Invoke(() =>
                {
                    txtCurURL.Text = url;
                });
            }
        }
        public void UpdateInitPageButton(BaseWndContextData? datacontext = null)
        {
            this.Dispatcher.Invoke(() =>
            {
                btnInitURL.GetBindingExpression(Button.IsEnabledProperty)?.UpdateTarget();
            });
        }

        public void UpdateNextPageButton(BaseWndContextData? datacontext = null)
        {
            this.Dispatcher.Invoke(() =>
            {
                btnNextPage.GetBindingExpression(Button.IsEnabledProperty)?.UpdateTarget();
            });
        }

        public void UpdateAutoDownloadPageButton(BaseWndContextData? datacontext = null)
        {
            this.Dispatcher.Invoke(() =>
            {
                btnAutoDownload.GetBindingExpression(Button.IsEnabledProperty)?.UpdateTarget();
            });
        }

        public void UpdateAnalysisPageButton(BaseWndContextData? datacontext = null)
        {
        }

        public void UpdateNovelName(string sNovelName, BaseWndContextData? datacontext = null)
        {
            this.Dispatcher.Invoke(() => { txtBookName.Text = sNovelName; });
            
        }
       
        public void UpdateAnalysizedContents(string strContents, BaseWndContextData? datacontext = null)
        {
            try
            {
                JObject? data = JsonConvert.DeserializeObject(strContents) as JObject;
                JObject? nextContents = data?["contents"] as JObject;
                if (nextContents != null)
                {
                    JObject? twoColumnWatchNextResults = nextContents["twoColumnWatchNextResults"] as JObject;
                    if (twoColumnWatchNextResults != null)
                    {
                        JObject? secondaryResults = twoColumnWatchNextResults["secondaryResults"] as JObject;
                        secondaryResults = secondaryResults?["secondaryResults"] as JObject;
                        JArray? results = secondaryResults?["results"] as JArray;
                        if (results != null)
                        {

                            txtYouTubLinks.Dispatcher.Invoke(() => {
                                FlowDocument doc = new FlowDocument();
                                txtYouTubLinks.Document = doc;
                                txtYouTubLinks.IsReadOnly = true;
                                Paragraph para = new Paragraph();
                                doc.Blocks.Add(para);
                                int nIdx = 0;
                                foreach (JObject jLink in results)
                                {
                                    nIdx++;
                                    JObject? compactVideoRenderer = jLink["compactVideoRenderer"] as JObject;
                                    string strTitle = ((compactVideoRenderer?["title"] as JObject)?["simpleText"])?.ToString() ?? "";
                                    string strLen = ((compactVideoRenderer?["lengthText"] as JObject)?["simpleText"])?.ToString() ?? "";
                                    string strIdentify = compactVideoRenderer?["videoId"]?.ToString() ?? "";

                                    Hyperlink link = new Hyperlink();
                                    link.IsEnabled = true;
                                    link.Inlines.Add(nIdx.ToString() + " : " + strTitle);
                                    link.NavigateUri = new Uri("https://www.youtube.com/watch?v=" + strIdentify);
                                    link.Click += new RoutedEventHandler(this.link_Click);
                                    para.Inlines.Add(link);
                                    para.Inlines.Add(new Run("  "));
                                    para.Inlines.Add(new Bold(new Run(strLen)));
                                    para.Inlines.Add(new Run("\r\n"));
                                }
                            });
                        }
                    }
                }
            }
            catch(Exception e)
            {
                UpdateStatusMsg(datacontext, e.Message, -1);
            }
        }

        protected void link_Click(object sender, RoutedEventArgs e)
        {
            string? sRef= (sender as Hyperlink)?.NavigateUri.ToString();
            if(!string.IsNullOrEmpty(sRef))
            {
                webBrowser.CoreWebView2.Navigate(sRef);
            }
            //MessageBox.Show("Clicked link!");
        }

        public void UpdateAggragatedContents(string strContents, BaseWndContextData? datacontext = null)
        {
            //throw new NotImplementedException();
        }

        public void UpdateAggragatedContentsWithLimit(string strContents, BaseWndContextData? datacontext = null)
        {
            //throw new NotImplementedException();
        }

        public bool isWebBrowserEmpty(BaseWndContextData? datacontext = null)
        {
            try
            {
                bool bLoadUnFinish = false;
                //this.Dispatcher.Invoke(() => { bRet = webBrowser == null || webBrowser.Document == null; });
                this.Dispatcher.Invoke(() => { bLoadUnFinish = webBrowser == null || webBrowser.CoreWebView2 == null || webBrowser.IsLoaded == false; });
                return bLoadUnFinish;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool isWebPageLoadComplete(string strURL, BaseWndContextData? datacontext = null)
        {
            try
            {
                bool bLoadFinish = false;
                //this.Dispatcher.Invoke(() => { bRet = webBrowser == null || webBrowser.Document == null; });
                this.Dispatcher.Invoke(() => { bLoadFinish = webBrowser != null && webBrowser.CoreWebView2 != null || webBrowser.IsLoaded == true; });
                return bLoadFinish;
            }
            catch (Exception)
            {
                return false;
            }
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
                    txtInitURL.Text = BaseWndContextData.GetDefaultUrlByIdx(cmbNovelType.SelectedIndex);

                    if (string.IsNullOrEmpty(txtCurURL.Text.Trim()))
                    {
                        txtCurURL.Text = txtInitURL.Text;
                    }
                }
            }
            if ((datacontext != null))
            {
                datacontext.SelectedIdx = nIndex;

                btnDownloadYouTube.GetBindingExpression(Button.VisibilityProperty).UpdateTarget();
                btnDownloadPornHub.GetBindingExpression(Button.VisibilityProperty).UpdateTarget();
            }
            UpdateInitPageButton();
            UpdateNextPageButton();
            UpdateAutoDownloadPageButton();
        }

        public string BatchDownloadNotified(BaseWndContextData? datacontext, DownloadStatus status, string sDownloadFileName)
        {
            string strMsgAreaLog = "";
            if (datacontext != null)
            {
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
            }

            return strMsgAreaLog;
        }

        public string GetNovelName(BaseWndContextData? datacontext = null)
        {
            try
            {
                string sNovelName = "";
                this.Dispatcher.Invoke(() => { sNovelName = txtBookName.Text.Trim(); });
                return sNovelName;
            }
            catch (Exception)
            {
                return "DefaultNovel";
            }
        }

        private void Hyperlink_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            var hyperlink = (Hyperlink)sender;
            Process.Start(hyperlink.NavigateUri.ToString());
        }
        bool DownloadTsFileAndMergeInAir(BaseWndContextData? datacontext, System.Collections.Generic.List<string> lstVideoSpices)
        {
            //ConfigureHWDecoder(out var deviceType);

            WebClient webClient = new WebClient();
            int nFileIdx = 0;
            foreach (string sFileUrl in lstVideoSpices)
            {
                DateTime start = DateTime.Now;
                //UpdateStatusMsg(datacontext, "Download Start: " + sFileUrl.ToString() + " ...", 0);
                nFileIdx++;
                string strTsFileName = string.Format("{0:0000}", nFileIdx) + ".ts";
                byte[] fileContent = webClient.DownloadData(sFileUrl);
                //System.IO.File.WriteAllBytes(strTsFileName, fileContent);

               

                TimeSpan span = DateTime.Now - start;
                UpdateStatusMsg(datacontext, "Download Finished and save to " + strTsFileName + " in " + string.Format("{0:#.##}", span.TotalSeconds) + " seconds. ", (int)(100.0 * nFileIdx / lstVideoSpices.Count));
            }

            // ffmpeg -i "concat:0001.ts|0002.ts|0003.ts|0004.ts|0005.ts" -bsf:a aac_adtstoasc -y full.mp4
            string sVideoName = "";
            this.Dispatcher.Invoke(() => { sVideoName = txtBookName.Text; });
            if (string.IsNullOrEmpty(sVideoName))
                sVideoName = "Video_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            else
            {
                sVideoName = sVideoName.Replace(" - YouTube", "").Replace('\\', '￥').Replace('#', '＃')
                    .Replace('$', '＄').Replace('%', '％').Replace('!', '！').Replace('&', '＆').Replace('\'', '’').Replace('{', '｛')
                    .Replace('\"', '”').Replace('}', '｝').Replace(':', '：').Replace('\\', '￥').Replace('@', '＠').Replace('<', '＜').Replace('>', '＞').Replace('+', '＋')
                    .Replace('`', '‘').Replace('*', '＊').Replace('|', '｜').Replace('?', '？').Replace('=', '＝').Replace('/', '／');
                sVideoName += DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            }

            string strCmd = "";// "ffmpeg -i \"concat:0001.ts|0002.ts|0003.ts|0004.ts|0005.ts\" -bsf:a aac_adtstoasc -y full.mp4";
            string strParam = "";// "ffmpeg -i \"concat:0001.ts|0002.ts|0003.ts|0004.ts|0005.ts\" -bsf:a aac_adtstoasc -y full.mp4";
            nFileIdx = 0;
            foreach (string sCmd in lstVideoSpices)
            {
                nFileIdx++;
                string strTsFileName = string.Format("{0:0000}", nFileIdx) + ".ts";
                if (string.IsNullOrEmpty(strCmd))
                {
                    strCmd = "ffmpeg -i \"concat:" + strTsFileName;
                    strParam = "-i \"concat:" + strTsFileName;
                }
                else
                {
                    strCmd += ("|" + strTsFileName);
                    strParam += ("|" + strTsFileName);
                }
            }
            strCmd += "\" -bsf:a aac_adtstoasc -y \"" + sVideoName + ".mp4\"";
            strParam += "\" -bsf:a aac_adtstoasc -y \"" + sVideoName + ".mp4\"";
            Debug.WriteLine(strCmd);

            DateTime startMerge = DateTime.Now;
            Process proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg.exe",
                    Arguments = strParam,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            UpdateStatusMsg(datacontext, "Begin Merge TS files to <" + sVideoName + ".mp4> ... ", 0);
            bool bRet = proc.Start();
            while (!proc.StandardError.EndOfStream)
            {
                string? sOutput = proc.StandardError.ReadLine() ?? "";

                UpdateStatusMsg(datacontext, sOutput, -1);
                // do something with line
            }
            TimeSpan spanMerge = DateTime.Now - startMerge;
            UpdateStatusMsg(datacontext, "Finished Merge TS files to " + sVideoName + ".mp4 in " + string.Format("{0:#.##}", spanMerge.TotalSeconds) + " seconds. ", 100);
            return true;
        }

    }
#pragma warning restore CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。
#pragma warning restore SYSLIB0014 // 型またはメンバーが旧型式です
}