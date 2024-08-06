using BaseBookDownloader;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using VideoLibrary;

namespace WpfStreamDownloader
{
#pragma warning disable SYSLIB0014 // 型またはメンバーが旧型式です
#pragma warning disable CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。
    public class DownloadTask
    {
        public string FileName { get; set; } = "";
        public string FullPathName { get; set; } = "";
        public string Uri { get; set; } = "";
        public string Progress { get; set; } = "";
        public DateTime StartTime { get; set; }
        public int ProgressChangeCnt = 0;
    }

    public class WndContextData : BaseWndContextData
    {
        public Visibility EnabledDbgButtons { get; set; } =
#if DEBUG
#if false
            Visibility.Visible;
#else
            Visibility.Hidden;
#endif
#else
            Visibility.Hidden;
#endif
        public List<DownloadTask> TaskList { get; set; } = new List<DownloadTask>();

    }

    public partial class WpfStreamMainWindow : Window, IBaseMainWindow
    {

        private void WebClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            DownloadTask task = (DownloadTask)e.UserState;
            Debug.Assert(task != null);
            WndContextData? datacontext = null;
            this.Dispatcher.Invoke(() => { datacontext = App.Current.MainWindow.DataContext as WndContextData; });
            TimeSpan span = DateTime.Now - task.StartTime;
            UpdateStatusMsg(datacontext, "Download Finished and save to " + task.FileName + " in " + string.Format("{0:#.##}", span.TotalSeconds) + " seconds. Cancelled:" + e.Cancelled + ", Result" + e.Result, 100);

            task.Progress = "100%";
            this.Dispatcher.Invoke(() => { lstTasks.Items.Refresh(); });
        }

        public bool DownloadFile(BaseWndContextData? datacontext0, string sVedioUrl, string sTitle, bool bForceDownload = false)
        {
            bool bAutoDownload = false;
            WndContextData? datacontext = datacontext0 as WndContextData;
            this.Dispatcher.Invoke(() => { return bAutoDownload = chkAutoDownload.IsChecked ?? false; });
            if (datacontext != null && bAutoDownload)
            {
                try
                {
                    if (IsYouTubeSite(sVedioUrl))
                    {
                        YouTube youTube = YouTube.Default;
                        var video = youTube.GetVideo(sVedioUrl);
                        string strFileName = /*AppDomain.CurrentDomain.BaseDirectory*/ datacontext.FileSavePath + video.FullName;

                        UpdateStatusMsg(datacontext, "Download Start: " + sVedioUrl.ToString() + " ...", 0);

                        DownloadTask task = new DownloadTask() { Uri = sVedioUrl, Progress= "0%", FullPathName= strFileName, StartTime=DateTime.Now, FileName=""};
                        datacontext.TaskList.Add(task);
                        this.Dispatcher.Invoke(() => { lstTasks.Items.Refresh(); });

                        System.IO.File.WriteAllBytes(strFileName, video.GetBytes());

                        TimeSpan span = DateTime.Now - task.StartTime;
                        UpdateStatusMsg(datacontext, "Download Finished and save to " + strFileName + " in " + string.Format("{0:#.##}", span.TotalSeconds) + " seconds. ", 100);
                        task.Progress = "100%";
                        this.Dispatcher.Invoke(() => { lstTasks.Items.Refresh(); });
                        return true;
                    }
                }
                catch (Exception e)
                {
                    UpdateStatusMsg(datacontext, "Download Finished with Error " + e.Message, 100);
                    return false;
                }
            }

            return false;
        }

        bool DownloadTsFileAndMergeByCmdLine(IBaseMainWindow wndMain, BaseWndContextData? datacontext0, string strOrgDownloadUrl, string sTitle, System.Collections.Generic.List<string> lstVideoSpices, string sVideoName)
        {
            WndContextData? datacontext = datacontext0 as WndContextData;
            string sVideoPath = "";
            if (string.IsNullOrEmpty(sVideoName))
                sVideoPath = datacontext?.FileSavePath + "Video_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            else
            {
                sVideoPath = datacontext?.FileSavePath + sVideoName.Replace(" - YouTube", "").Replace('\\', '￥').Replace('#', '＃')
                    .Replace('$', '＄').Replace('%', '％').Replace('!', '！').Replace('&', '＆').Replace('\'', '’').Replace('{', '｛')
                    .Replace('\"', '”').Replace('}', '｝').Replace(':', '：').Replace('\\', '￥').Replace('@', '＠').Replace('<', '＜').Replace('>', '＞').Replace('+', '＋')
                    .Replace('`', '‘').Replace('*', '＊').Replace('|', '｜').Replace('?', '？').Replace('=', '＝').Replace('/', '／');
                sVideoPath += DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            }

            WebClient webClient = new WebClient();
            int nFileIdx = 0;
            string sLeadId = string.Format("{0:#}", Thread.CurrentThread.ManagedThreadId) + "_";
            var items = lstTasks.Items;
            
            DownloadTask task = new DownloadTask() { Uri = strOrgDownloadUrl, Progress = "0%", FileName= sVideoName, FullPathName = datacontext.FileSavePath + sVideoName + ".mp4", StartTime = DateTime.Now, };
            datacontext.TaskList.Add(task);
            this.Dispatcher.Invoke(() => { lstTasks.Items.Refresh(); });

            foreach (string sFileUrl in lstVideoSpices)
            {
                DateTime start = DateTime.Now;
                nFileIdx++;
                string strTsFileName = datacontext?.FileTempPath + sLeadId + string.Format("{0:0000}", nFileIdx) + ".ts";
                byte[] fileContent = webClient.DownloadData(sFileUrl);
                System.IO.File.WriteAllBytes(strTsFileName, fileContent);
                TimeSpan span = DateTime.Now - start;
                wndMain.UpdateStatusMsg(datacontext, "Download Finished and save to " + strTsFileName + " in " + string.Format("{0:#.##}", span.TotalSeconds) + " seconds. ", (int)(100.0 * nFileIdx / lstVideoSpices.Count));
                task.Progress = string.Format("{0:###.00}", (int)(50.0 * nFileIdx / lstVideoSpices.Count)) + "%";
            }

            string strCmd = "";
            string strParam = "";
            nFileIdx = 0;
            foreach (string sCmd in lstVideoSpices)
            {
                nFileIdx++;
                string strTsFileName = datacontext?.FileTempPath + sLeadId + string.Format("{0:0000}", nFileIdx) + ".ts";
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
            strCmd += "\" -bsf:a aac_adtstoasc -y \"" + sVideoPath + ".mp4\"";
            strParam += "\" -bsf:a aac_adtstoasc -y \"" + sVideoPath + ".mp4\"";
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

            wndMain.UpdateStatusMsg(datacontext, "Begin Merge TS files to <" + sVideoPath + ".mp4> ... ", 0);
            bool bRet = proc.Start();
            int n = 0;
            while (!proc.StandardError.EndOfStream)
            {
                n++;
                string? sOutput = proc.StandardError.ReadLine() ?? "";

                wndMain.UpdateStatusMsg(datacontext, sOutput, -1);
                task.Progress = string.Format("{0:###.00}", 50 + 49 * (1 - 1.0/Math.Pow(2, n/50))) + "%";
                this.Dispatcher.Invoke(() => { lstTasks.Items.Refresh(); });

            }

            nFileIdx = 0;
            foreach (string sCmd in lstVideoSpices)
            {
                nFileIdx++;
                string strTsFileName = datacontext?.FileTempPath + sLeadId + string.Format("{0:0000}", nFileIdx) + ".ts";
                File.Delete(strTsFileName);
            }

            TimeSpan spanMerge = DateTime.Now - startMerge;
            task.Progress = "100%";
            wndMain.UpdateStatusMsg(datacontext, "Finished Merge TS files to " + sVideoName + ".mp4 in " + string.Format("{0:#.##}", spanMerge.TotalSeconds) + " seconds. ", 100);
            return true;
        }

        public bool DownloadFile(BaseWndContextData? datacontext0, System.Collections.Generic.Dictionary<string, string> dictUrls, string sTitle, bool bForceDownload = false)
        {
            bool bAutoDownload = false;
            try
            {
                WndContextData? datacontext = datacontext0 as WndContextData;
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
                        byte[]? fileContent = null;
                        if (IsPornHubSite(strDownloadUrl))
                        {
                            string sVideoName = "";
                            this.Dispatcher.Invoke(() => { sVideoName = txtBookName.Text; });

                            fileContent = webClient.DownloadData(strDownloadUrl);
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
                            DownloadTsFileAndMergeByCmdLine(this, datacontext, strDownloadUrl, sTitle, lstVideoSpices, sVideoName);
                        }
                        else if (IsRedPornSite(strDownloadUrl))
                        {
                            string sVideoName = "";
                            this.Dispatcher.Invoke(() => { sVideoName = datacontext.FileSavePath + txtBookName.Text; });

                            UpdateStatusMsg(datacontext, "Begin download " + strDownloadUrl + " ... ", 0);
                            DownloadTask taskPara = new DownloadTask() { StartTime = DateTime.Now, FileName = Path.GetFileName(sVideoName), Progress = "", Uri = strDownloadUrl };
                            datacontext.TaskList.Add(taskPara);
                            this.Dispatcher.Invoke(() => { lstTasks.Items.Refresh(); });

                            webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;
                            webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                            webClient.DownloadFileAsync(new Uri(strDownloadUrl), sVideoName + ".mp4", taskPara);
                        }
                    }
                }
            }
            catch(TaskCanceledException)
            {

            }

            return true;
        }

        private void WebClient_DownloadFileCompleted(object? sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            DownloadTask task = (DownloadTask)e.UserState;
            Debug.Assert(task != null);
            WndContextData? datacontext = null;
            this.Dispatcher.Invoke(() => { datacontext = App.Current.MainWindow.DataContext as WndContextData; });
            TimeSpan span = DateTime.Now - task.StartTime;
            UpdateStatusMsg(datacontext, "Download Finished and save to " + task.FileName + " in " + string.Format("{0:#.##}", span.TotalSeconds) + " seconds. Cancelled:" + e.Cancelled + ", Error:" + e.Error + ", Result:" + e.ToString(), 100);
            task.Progress = "100%";
            this.Dispatcher.Invoke(() => { lstTasks.Items.Refresh(); });
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadTask task = (DownloadTask)e.UserState;
            Debug.Assert(task != null);
            task.ProgressChangeCnt++;
            if (task.ProgressChangeCnt % 59 == 0)
            {
                try
                {
                    WndContextData? datacontext = null;
                    this.Dispatcher.Invoke(() => { datacontext = App.Current.MainWindow.DataContext as WndContextData; });
                    UpdateStatusMsg(datacontext, "Downloading " + task.FileName + "... " + (e.BytesReceived / 1024 / 1024) + " MB/" + (e.TotalBytesToReceive / 1024 / 1024) + " MB Received. ", e.ProgressPercentage);
                    task.Progress = e.ProgressPercentage + "%";
                    this.Dispatcher.Invoke(() => { lstTasks.Items.Refresh(); });
                }
                catch (Exception) { }
            }
        }
    }
#pragma warning restore CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。
#pragma warning restore SYSLIB0014 // 型またはメンバーが旧型式です
}