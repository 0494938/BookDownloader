using HtmlAgilityPack;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;

namespace iKbook8
{
    public interface IFetchNovelContent {
        public void AnalysisHtmlBookBody(MainWindow  wndMain, ref WndContextData datacontext, string strBody, bool bSilenceMode = false, DownloadStatus? status = null);
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
        public void AnalysisHtmlBody(ref WndContextData datacontext,ref string strBody, bool bSilenceMode=false, DownloadStatus? atatus=null)
        {
            Debug.Assert(!bSilenceMode || (bSilenceMode && atatus !=null));
            UpdateStatusMsg(datacontext, "Begin to Analysize downloaded Uri Contents Body...");

            switch (datacontext.SiteType)
            {
                case BatchQueryNovelContents.IKBOOK8:
                    (new IKBook8NovelContent()).AnalysisHtmlBookBody(this, ref datacontext, strBody, bSilenceMode, atatus);
                    break;
                case BatchQueryNovelContents.QQBOOK:
                    (new OOBookNovelContent()).AnalysisHtmlBookBody(this, ref datacontext, strBody, bSilenceMode, atatus);
                    break;
                case BatchQueryNovelContents.BIQUGE:
                    (new BiQuGeBookNovelContent()).AnalysisHtmlBookBody(this, ref datacontext, strBody, bSilenceMode, atatus);
                    break;
                default:
                    break;
            }
            UpdateStatusMsg(datacontext, "Finished Analysing of  downloaded Uri Contents Body...");
        }
    }
}