using HtmlAgilityPack;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection.PortableExecutable;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;

namespace iKbook8
{
    public interface IFetchNovelContent {
        public void AnalysisHtmlBookBody(MainWindow  wndMain, WndContextData datacontext, string strBody, bool bSilenceMode = false, DownloadStatus? status = null);
        public void FindBookNextLinkAndContents(HtmlNode parent, ref HtmlNode nextLink, ref HtmlNode header, ref HtmlNode content);
        public string GetBookHeader(HtmlNode header);
        public string GetBookNextLink(HtmlNode nextLink);
        public string GetBookContents(HtmlNode content);
        public string GetBookName(HtmlNode content);
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public void AnalysisHtmlBody(WndContextData datacontext, string strURL, string strBody, bool bSilenceMode = false, DownloadStatus? status = null)
        {
            Thread thread = new Thread(() => AnalysisHtmlBodyThreadFunc(datacontext, strURL, strBody, bSilenceMode , status));
            thread.Start();

        }
        public void AnalysisHtmlBodyThreadFunc(WndContextData datacontext, string strURL, string strBody, bool bSilenceMode=false, DownloadStatus? status = null)
        {
            Debug.Assert(!bSilenceMode || (bSilenceMode && status != null));
            if (bSilenceMode)
            {
                UpdateStatusMsg(datacontext, strURL + " : Begin to Analysize downloaded Contents Body ...", (int)((100.0 / DownloadStatus.ThreadMax * (status.ThreadNum-1+0.5))));
            }
            else
                UpdateStatusMsg(datacontext, strURL+ " : Begin to Analysize downloaded Contents Body ...", 50);

            switch (datacontext.SiteType)
            {
                case BatchQueryNovelContents.IKBOOK8:
                    (new IKBook8NovelContent()).AnalysisHtmlBookBody(this, datacontext, strBody, bSilenceMode, status);
                    break;
                case BatchQueryNovelContents.QQBOOK:
                    (new OOBookNovelContent()).AnalysisHtmlBookBody(this, datacontext, strBody, bSilenceMode, status);
                    break;
                case BatchQueryNovelContents.BIQUGE:
                case BatchQueryNovelContents.BIQUGE2:
                    (new BiQuGeBookNovelContent()).AnalysisHtmlBookBody(this, datacontext, strBody, bSilenceMode, status);
                    break;
                default:
                    break;
            }
            if (bSilenceMode)
            {
                UpdateStatusMsg(datacontext, strURL + " : Finished Analysing of downloaded Uri Contents Body ...", (int)((100.0 / DownloadStatus.ThreadMax * (status.ThreadNum))));
            }
            else
                UpdateStatusMsg(datacontext, strURL + " : Finished Analysing of downloaded Uri Contents Body ...", 100);
            if (bSilenceMode)
            {
                if (status.ThreadNum < DownloadStatus.ThreadMax && !string.IsNullOrEmpty(status.NextUrl))
                {
                    DownloadOneURLAndGetNext(datacontext, status.NextUrl);
                }
                else
                {
                    DownloadStatus.ContentsWriter = null;
                    UpdateStatusMsg(datacontext, "Finished batch download ...", 100);
                    this.Dispatcher.Invoke(() =>
                    {
                        if (!string.IsNullOrEmpty(status.NextUrl))
                            txtInitURL.Text = status.NextUrl;
                        MessageBox.Show(this, "Batch download finished...", "Web Novel Downloader", MessageBoxButton.OK);
                    });
                }
            }
        }
    }
}