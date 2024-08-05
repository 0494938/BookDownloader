using BaseBookDownloader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using VideoLibrary;

namespace WpfStreamDownloader
{
#pragma warning disable SYSLIB0014 // 型またはメンバーが旧型式です
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
            throw new NotImplementedException();
        }

        public void UpdateNextPageButton(BaseWndContextData? datacontext = null)
        {
            //throw new NotImplementedException();
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
            this.Dispatcher.Invoke(() =>
            {
                txtCurURL.Text = url;
            });
        }
        public void UpdateInitPageButton(BaseWndContextData? datacontext = null)
        {
            throw new NotImplementedException();
        }

        public void UpdateAutoDownloadPageButton(BaseWndContextData? datacontext = null)
        {
            throw new NotImplementedException();
        }

        public void UpdateAnalysisPageButton(BaseWndContextData? datacontext = null)
        {
            throw new NotImplementedException();
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
                                foreach (JObject jLink in results)
                                {
                                    JObject? compactVideoRenderer = jLink["compactVideoRenderer"] as JObject;
                                    string strTitle = ((compactVideoRenderer?["title"] as JObject)?["simpleText"])?.ToString() ?? "";
                                    string strLen = ((compactVideoRenderer?["lengthText"] as JObject)?["simpleText"])?.ToString() ?? "";
                                    string strIdentify = compactVideoRenderer?["videoId"]?.ToString() ?? "";

                                    //para.Inlines.Add(new Bold(new Run(strTitle + "\r\n")));
                                    //para.Inlines.Add(new Bold(new Run("Length : ")));
                                    //para.Inlines.Add(new Run(strLen + "\r\n"));
                                    Hyperlink link = new Hyperlink();
                                    link.IsEnabled = true;
                                    link.Inlines.Add(strTitle);
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
            throw new NotImplementedException();
        }

        public bool isWebBrowserEmpty(BaseWndContextData? datacontext = null)
        {
            throw new NotImplementedException();
        }

        public bool isWebPageLoadComplete(string strURL, BaseWndContextData? datacontext = null)
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
            if ((datacontext != null))
            {
                if (cmbNovelType.SelectedIndex == (int)BatchQueryNovelContents.YOUTUBE)
                    datacontext.IsYouTube = true;
                else
                    datacontext.IsYouTube = false;
                if (cmbNovelType.SelectedIndex == (int)BatchQueryNovelContents.PORNHUB)
                    datacontext.IsPornTube = true;
                else
                    datacontext.IsPornTube = false;

                btnDownloadYouTube.GetBindingExpression(Button.VisibilityProperty).UpdateTarget();
                btnDownloadPornHub.GetBindingExpression(Button.VisibilityProperty).UpdateTarget();
            }
        }

        public string BatchDownloadNotified(BaseWndContextData? datacontext, DownloadStatus status, string sDownloadFileName)
        {
            throw new NotImplementedException();
        }

        public string GetNovelName(BaseWndContextData? datacontext = null)
        {
            throw new NotImplementedException();
        }

        public bool DownloadFile(BaseWndContextData? datacontext, string sVedioUrl, bool bForceDownload = false)
        {
            //WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            bool bAutoDownload = false;
            this.Dispatcher.Invoke(() => { return bAutoDownload = chkAutoDownload.IsChecked ?? false; });
            if (datacontext != null && bAutoDownload)
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
                    UpdateStatusMsg(datacontext, "Download Finished and save to " + strFileName + " in " + string.Format("{0:#.##}", span.TotalSeconds) + " seconds. ", 100);
                    return true;
                }
                catch (Exception e) {
                    UpdateStatusMsg(datacontext, "Download Finished with Error " + e.Message, 100);
                    return false;
                }
            }

            return false;
        }

        bool DownloadTsFileAndMergeByCmdLine(BaseWndContextData? datacontext, System.Collections.Generic.List<string> lstVideoSpices)
        {
            WebClient webClient = new WebClient();
            int nFileIdx = 0;
            string sLeadId = string.Format("{0:#}", Thread.CurrentThread.ManagedThreadId) + "_";
            foreach (string sFileUrl in lstVideoSpices)
            {
                DateTime start = DateTime.Now;
                //UpdateStatusMsg(datacontext, "Download Start: " + sFileUrl.ToString() + " ...", 0);
                nFileIdx++;
                string strTsFileName = sLeadId + string.Format("{0:0000}", nFileIdx) + ".ts";
                byte[] fileContent = webClient.DownloadData(sFileUrl);
                System.IO.File.WriteAllBytes(strTsFileName, fileContent);
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
                string strTsFileName = sLeadId + string.Format("{0:0000}", nFileIdx) + ".ts";
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

            nFileIdx = 0;
            foreach (string sCmd in lstVideoSpices)
            {
                nFileIdx++;
                string strTsFileName = sLeadId + string.Format("{0:0000}", nFileIdx) + ".ts";
                File.Delete(strTsFileName);
            }

            TimeSpan spanMerge = DateTime.Now - startMerge;
            UpdateStatusMsg(datacontext, "Finished Merge TS files to " + sVideoName + ".mp4 in " + string.Format("{0:#.##}", spanMerge.TotalSeconds) + " seconds. ", 100);
            return true;
        }

        private void Hyperlink_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            var hyperlink = (Hyperlink)sender;
            Process.Start(hyperlink.NavigateUri.ToString());
        }

        //private void ConfigureHWDecoder(out AVHWDeviceType HWtype)
        //{
        //    HWtype = AVHWDeviceType.AV_HWDEVICE_TYPE_NONE;
        //    //Console.WriteLine("Use hardware acceleration for decoding?[n]");
        //    //var key = Console.ReadLine();
        //    var availableHWDecoders = new Dictionary<int, AVHWDeviceType>();

        //    //if (key == "y")
        //    {
        //        //Console.WriteLine("Select hardware decoder:");
        //        var type = AVHWDeviceType.AV_HWDEVICE_TYPE_NONE;
        //        var number = 0;

        //        while ((type = ffmpeg.av_hwdevice_iterate_types(type)) != AVHWDeviceType.AV_HWDEVICE_TYPE_NONE)
        //        {
        //            //Console.WriteLine($"{++number}. {type}");
        //            availableHWDecoders.Add(number, type);
        //        }

        //        if (availableHWDecoders.Count == 0)
        //        {
        //            //Console.WriteLine("Your system have no hardware decoders.");
        //            HWtype = AVHWDeviceType.AV_HWDEVICE_TYPE_NONE;
        //            return;
        //        }

        //        var decoderNumber = availableHWDecoders
        //            .SingleOrDefault(t => t.Value == AVHWDeviceType.AV_HWDEVICE_TYPE_DXVA2).Key;
        //        if (decoderNumber == 0)
        //            decoderNumber = availableHWDecoders.First().Key;
        //        Debug.WriteLine($"Selected [{decoderNumber}]");
        //        //Console.WriteLine($"Selected [{decoderNumber}]");
        //        int.TryParse(Console.ReadLine(), out var inputDecoderNumber);
        //        availableHWDecoders.TryGetValue(inputDecoderNumber == 0 ? decoderNumber : inputDecoderNumber,
        //            out HWtype);
        //    }
        //}
        
        public bool DownloadFile(BaseWndContextData? datacontext, System.Collections.Generic.Dictionary<string, string> dictUrls, bool bForceDownload = false)
        {
            bool bAutoDownload = false;
            this.Dispatcher.Invoke(() => { return bAutoDownload = chkAutoDownload.IsChecked ?? false; });
            if (datacontext != null && (bAutoDownload || bForceDownload))
            {
                string strDownloadUrl = "";
                if (dictUrls.ContainsKey("1080"))
                    strDownloadUrl = dictUrls["1080"];
                else if (dictUrls.ContainsKey("720"))
                    strDownloadUrl = dictUrls["720"];
                else if (dictUrls.ContainsKey("480"))
                    strDownloadUrl = dictUrls["480"];
                else if (dictUrls.ContainsKey("240"))
                    strDownloadUrl = dictUrls["240"];
                else if (dictUrls.Count > 0)
                    strDownloadUrl = dictUrls.FirstOrDefault().Value ?? "";

                if (!string.IsNullOrEmpty(strDownloadUrl))
                {
                    WebClient webClient = new WebClient();
                    byte[] fileContent = webClient.DownloadData(strDownloadUrl);
                    string sContent = Encoding.UTF8.GetString(fileContent);
                    string sUrl = strDownloadUrl.Substring(0, strDownloadUrl.IndexOf(".mp4/") + ".mp4/".Length);

                    StringReader sr = new StringReader(sContent);
                    string strAlt = "";
                    string? line = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        line = line.Trim();
                        if (!string.IsNullOrEmpty(line) && !line.StartsWith("#"))
                        {
                            strAlt = line;
                            break;
                        }
                    }
                    fileContent = webClient.DownloadData(sUrl + strAlt);
                    sContent = Encoding.UTF8.GetString(fileContent);

                    System.Collections.Generic.List<string> lstVideoSpices = new System.Collections.Generic.List<string>();
                    sr = new StringReader(sContent);
                    line = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        line = line.Trim();
                        if (!string.IsNullOrEmpty(line) && !line.StartsWith("#"))
                        {
                            lstVideoSpices.Add(sUrl + line);
                        }
                    }

                    //DownloadTsFileAndMergeInAir(datacontext, lstVideoSpices);
                    DownloadTsFileAndMergeByCmdLine(datacontext, lstVideoSpices);
                }
            }
            return true;
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
#pragma warning restore SYSLIB0014 // 型またはメンバーが旧型式です
}