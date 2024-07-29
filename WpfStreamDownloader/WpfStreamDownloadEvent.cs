using BaseBookDownloader;
using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

            BtnClickActions(txtNextUrl.Text);
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
                string VedioUrl = txtCurURL.Text.Trim();
                YouTube youTube = YouTube.Default;
                var video = youTube.GetVideo(VedioUrl);
                string strFileName = AppDomain.CurrentDomain.BaseDirectory + System.IO.Path.DirectorySeparatorChar + video.FullName + ".mp4";

                new Thread(() => {
                    DateTime start = DateTime.Now;
                    UpdateStatusMsg(datacontext, "Download Start: " + VedioUrl.ToString() + " ...", 100);
                    System.IO.File.WriteAllBytes(strFileName, video.GetBytes());
                    TimeSpan span = DateTime.Now - start;
                    UpdateStatusMsg(datacontext, "Download Finished and save to " + strFileName + " in " + span.TotalSeconds + "seconds. ", 100);
                }).Start();
            }
        }

        private void OnPornHubDownload(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnYoutubeDownload invoked...");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if (datacontext != null)
            {
                //string VedioUrl = "https://cv-h.phncdn.com/hls/videos/202404/11/450925391/720P_4000K_450925391.mp4";
                //string VedioUrl = "https://cv-h.phncdn.com/hls/videos/202404/11/450925391/720P_4000K_450925391.mp4/master.m3u8?JWnOXHnl4LLaxvkxY6IDKriCh7qK4s-MT9XcIlGBVTU7FUj6aP9jg7ebjUDRxseVwt6C2Zvki0vMDsSLc1OiVODReZL3o9xaNSDSlXJJaxayh8MKURnsFhGfYbFsdk7GW903Xmu-9kgr16xZ4eGCm6KgQ8C-JmEgf6cl50wWe3f4cIBbds_4Bo5fnb_ZRvLUzAEBwgC8CBw";

                Debug.Assert(webBrowser.CoreWebView2 != null);
                CoreWebView2Frame? frame0 = frame;
                //webBrowser.CoreWebView2.

                //new Thread(() => {
                //    DateTime start = DateTime.Now;
                //    UpdateStatusMsg(datacontext, "Download Start: " + VedioUrl.ToString() + " ...", 100);
                //    System.IO.File.WriteAllBytes(strFileName, video.GetBytes());
                //    TimeSpan span = DateTime.Now - start;
                //    UpdateStatusMsg(datacontext, "Download Finished and save to " + strFileName + " in " + span.TotalSeconds + "seconds. ", 100);
                //}).Start();
            }

        }
    }
}