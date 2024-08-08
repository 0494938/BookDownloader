using BaseBookDownloader;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using VideoLibrary;

namespace WpfStreamDownloader
{
    public partial class WpfStreamMainWindow : Window
    {
        private void BtnClickActions(string strUrl)
        {
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if (datacontext != null && !string.IsNullOrEmpty(strUrl.Trim()))
            {
                datacontext.DictDownloadStatus.Clear();
                datacontext.DictHandledContentsForDupCheck.Clear();
                datacontext.PageLoaded = false;
                datacontext.NextLinkAnalysized = false;

                UpdateAutoDownloadPageButton();
                UpdateNextPageButton();
                UpdateInitPageButton();
                txtWebContents.Text = "";

                datacontext.SiteType = (BatchQueryNovelContents)cmbNovelType.SelectedIndex;
                try
                {
                    datacontext.PgmNaviUrl = strUrl;
                    //webBrowser.CoreWebView2.Navigate(strUrl);
                    LoadUiUrl(datacontext, strUrl);
                    UpdateStatusMsg(datacontext, strUrl + " : Begin to download ...", 0);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    if (webBrowser == null || webBrowser.CoreWebView2 == null || webBrowser.IsLoaded == false)
                        return;
                }
            }
        }
        private void btnInitURL_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("btnInitURL_Click invoked...");
            BtnClickActions(txtInitURL.Text);
        }

        private void btnCurrURL_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("btnCurrURL_Click invoked...");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if ((datacontext != null))
                datacontext.SiteType = (BatchQueryNovelContents)cmbNovelType.SelectedIndex;

            BtnClickActions(txtCurURL.Text);
        }

        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("btnNextPage_Click invoked...");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if ((datacontext != null))
                datacontext.SiteType = (BatchQueryNovelContents)cmbNovelType.SelectedIndex;

            BtnClickActions(txtNextUrl.Text);
        }

        private void btnAutoURL_Click(object sender, RoutedEventArgs e)
        {
            txtLog.Clear();
            Debug.WriteLine("btnAutoURL_Click invoked...");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if (datacontext != null)
            {
                datacontext.DictDownloadStatus.Clear();
                datacontext.DictHandledContentsForDupCheck.Clear();
                datacontext.BackGroundNotRunning = false;
            
                UpdateAutoDownloadPageButton();
                UpdateNextPageButton();
                UpdateInitPageButton();

                datacontext.SiteType = (BatchQueryNovelContents)cmbNovelType.SelectedIndex;
                int nMaxPage = string.IsNullOrEmpty(txtPages.Text.Trim()) ? 20 : int.Parse(txtPages.Text.Trim());
                DownloadStatus.MaxPageToDownload = nMaxPage;

                DownloadStatus.ContentsWriter = new StreamWriter(
                    File.Open(datacontext.FileSavePath + (string.IsNullOrEmpty(txtOutputFileName.Text.Trim()) ? @"Content" : txtOutputFileName.Text.Trim()) + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss") + ".txt",
                    FileMode.CreateNew,
                    FileAccess.ReadWrite,
                    FileShare.Read),
                    Encoding.UTF8
                );

                datacontext.DownloadOneURLAndGetNext(this, txtInitURL.Text, false);
            }
        }

        private void OnYoutubeDownload(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnYoutubeDownload invoked...");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            string sTitle = txtBookName.Text.Trim(); 
            if (datacontext != null)
            {
                string VedioUrl = webBrowser.Source.ToString().Trim();
                YouTube youTube = YouTube.Default;
                try
                {
                    var video = youTube.GetVideo(VedioUrl);
                    string strFileName = datacontext.FileSavePath + video.FullName;

                    new Thread(() => {
                        DateTime start = DateTime.Now;
                        try
                        {
                            UpdateStatusMsg(datacontext, "Download Start: " + VedioUrl.ToString() + " ...", 0);

                            DownloadTask task = new DownloadTask() { Uri = VedioUrl, Progress = "0%", FullPathName = strFileName, FileName = sTitle, StartTime = DateTime.Now, };
                            datacontext.TaskList.Add(task);

                            this.Dispatcher.Invoke(() => { lstTasks.Items.Refresh(); });

                            System.IO.File.WriteAllBytes(strFileName, video.GetBytes());

                            TimeSpan span = DateTime.Now - start;
                            UpdateStatusMsg(datacontext, "Download Finished and save to " + strFileName + " in " + string.Format("{0:#.##}", span.TotalSeconds) + " seconds. ", 100);
                            task.Progress = "100%";
                            this.Dispatcher.Invoke(() => { lstTasks.Items.Refresh(); });
                        }
                        catch (TaskCanceledException) { }
                    }).Start();
                }
                catch (Exception ex) {
                    UpdateStatusMsg(datacontext, "Failed to download " + VedioUrl + " . " + ex.Message, -1);
                }
            }
        }

        private void OnPornHubDownload(object sender, RoutedEventArgs e)
        {
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if (datacontext != null)
            {
                new Thread(() => {
                    string strUrl="";
                    try
                    {
                        this.Dispatcher.Invoke(() => { strUrl = webBrowser.Source.ToString(); });

                        string? sHtml = GetWebDocHtmlSource(strUrl);
                        if (!string.IsNullOrEmpty(sHtml) && IsPornHubSite(strUrl))
                        {
                            PornHubStreamPageContent analysizer = new PornHubStreamPageContent();
                            analysizer.AnalysisHtmlStream(this, datacontext, strUrl, sHtml, false, null, 0, true);
                        }
                        else if (!string.IsNullOrEmpty(sHtml) && IsRedPornSite(strUrl))
                        {
                            RedPornStreamPageContent analysizer = new RedPornStreamPageContent();
                            analysizer.AnalysisHtmlStream(this, datacontext, strUrl, sHtml, false, null, 0, true);
                        }
                        else
                            UpdateStatusMsg(datacontext, "Download Failed as Html is empty...", 100);
                    }
                    catch (TaskCanceledException) { }
                }).Start();
                
            }
        }
    }
}