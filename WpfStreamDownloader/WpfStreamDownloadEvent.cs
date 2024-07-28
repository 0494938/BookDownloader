using BaseBookDownloader;
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
        private void ClickBtntnInitURL(string strUrl)
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
            ClickBtntnInitURL(txtInitURL.Text);
        }

        private void btnCurrURL_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("btnNextPage_Click invoked...");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if ((datacontext != null))
                datacontext.SiteType = (BatchQueryNovelContents)cmbNovelType.SelectedIndex;

            ClickBtntnInitURL(txtNextUrl.Text);
        }

        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("btnCurrURL_Click invoked...");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if ((datacontext != null))
                datacontext.SiteType = (BatchQueryNovelContents)cmbNovelType.SelectedIndex;

            ClickBtntnInitURL(txtCurURL.Text);
        }

        private void btnAutoURL_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OnYoutubeDownload(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnYoutubeDownload invoked...");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            //var VedioUrl = "https://www.youtube.com/embed/" + "0pPPXeXKdfg" + ".mp4";
            var VedioUrl = txtCurURL.Text.Trim();
            var youTube = YouTube.Default;

            UpdateStatusMsg(datacontext, "Download Start: " + VedioUrl.ToString() + " ...", 100);
            var video = youTube.GetVideo(VedioUrl);
            System.IO.File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + System.IO.Path.DirectorySeparatorChar + video.FullName + ".mp4", video.GetBytes());
            UpdateStatusMsg(datacontext, "Download Finished: " + VedioUrl.ToString() + " ...", 100);
        }

        private void OnPornHubDownload(object sender, RoutedEventArgs e)
        {

        }
    }
}