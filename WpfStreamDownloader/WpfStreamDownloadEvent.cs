using BaseBookDownloader;
using System.Diagnostics;
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
            if ((datacontext != null))
            {
                datacontext.DictDownloadStatus.Clear();
                datacontext.PageLoaded = false;
                datacontext.NextLinkAnalysized = false;

                btnNextPage.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                txtWebContents.Text = "";

                datacontext.SiteType = (BatchQueryNovelContents)cmbNovelType.SelectedIndex;
                try
                {
                    datacontext.PgmNaviUrl = strUrl;
                    webBrowser.CoreWebView2.Navigate(strUrl);
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
            Debug.WriteLine("btnNextPage_Click invoked...");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if ((datacontext != null))
                datacontext.SiteType = (BatchQueryNovelContents)cmbNovelType.SelectedIndex;

            BtnClickActions(txtCurURL.Text);
        }

        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("btnCurrURL_Click invoked...");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if ((datacontext != null))
                datacontext.SiteType = (BatchQueryNovelContents)cmbNovelType.SelectedIndex;

            BtnClickActions(txtCurURL.Text);
        }

        private void btnAutoURL_Click(object sender, RoutedEventArgs e)
        {
        }

        private void OnYoutubeDownload(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnYoutubeDownload invoked...");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if (datacontext != null)
            {
                //var VedioUrl = "https://www.youtube.com/embed/" + "0pPPXeXKdfg" + ".mp4";
                string VedioUrl = webBrowser.Source.ToString().Trim();
                YouTube youTube = YouTube.Default;
                var video = youTube.GetVideo(VedioUrl);
                string strFileName = AppDomain.CurrentDomain.BaseDirectory + System.IO.Path.DirectorySeparatorChar + video.FullName;

                new Thread(() => {
                    DateTime start = DateTime.Now;
                    UpdateStatusMsg(datacontext, "Download Start: " + VedioUrl.ToString() + " ...", 0);
                    System.IO.File.WriteAllBytes(strFileName, video.GetBytes());
                    TimeSpan span = DateTime.Now - start;
                    UpdateStatusMsg(datacontext, "Download Finished and save to " + strFileName + " in " + string.Format("{0:#.##}", span.TotalSeconds) + " seconds. ", 100);
                }).Start();
            }
        }

        private void OnPornHubDownload(object sender, RoutedEventArgs e)
        {
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if (datacontext != null)
            {
                new Thread(() => {
                    string strUrl="";
                    this.Dispatcher.Invoke(() => { strUrl= webBrowser.Source.ToString(); });
                    
                    string? sHtml = GetWebDocHtmlSource(strUrl);
                    if (!string.IsNullOrEmpty(sHtml))
                    {
                        PornHubStreamPageContent analysizer = new PornHubStreamPageContent();
                        analysizer.AnalysisHtmlStream(this, datacontext, strUrl, sHtml, false, null, 0, true);
                    }else
                        UpdateStatusMsg(datacontext, "Download Failed as Html is empty...", 100);
                }).Start();
                
            }
        }
    }
}