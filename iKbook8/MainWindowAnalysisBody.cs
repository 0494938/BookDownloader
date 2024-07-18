using HtmlAgilityPack;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace iKbook8
{
    public interface IFetchNovelContent {
        public void AnalysisHtmlBookBody(MainWindow  wndMain, ref WndContextData datacontext, string strBody, bool bSilenceMode = false, DownloadStatus? status = null);
        public void FindBookNextLinkAndContents(HtmlNode parent, ref HtmlNode section_opt, ref HtmlNode header, ref HtmlNode content);
        public string GetBookHeader(HtmlNode header);
        public string GetBookNextLink(HtmlNode section_opt);
        public string GetBookContents(HtmlNode content);
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private void AnalysisHtmlBody(ref WndContextData datacontext,ref string strBody, bool bSilenceMode=false, DownloadStatus? atatus=null)
        {
            Debug.Assert(!bSilenceMode || (bSilenceMode && atatus !=null));

            UpdateStatusMsg(datacontext, "Begin to Analysize downloaded Uri Contents Body...");
            switch (datacontext.SiteType){
                case BatchQueryNovelContents.IKBOOK8:
                    /*
                    AnalysisHtmlIkBookBody(ref datacontext,
                    "<!DOCTYPE html>\r\n<html>\r\n" +
                    "<head>\r\n<meta charset=\"gbk\"/>\r\n" +
                    "<meta name=\"applicable-device\" content=\"pc,mobile\">\r\n" +
                    "<meta name=\"MobileOptimized\" content=\"width\">\r\n" +
                    "<meta name=\"HandheldFriendly\" content=\"true\">\r\n" +
                    "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0\"/>\r\n" +
                    "<meta name=\"renderer\" content=\"webkit|ie-comp|ie-stand\"/>\r\n" +
                    "<meta http-equiv=\"X-UA-Compatible\" content=\"ie=edge\"/>\r\n" +
                    "<title>Current Chapter</title>\r\n" +
                    "<meta http-equiv=\"Cache-Control\" content=\"no-transform\" />\r\n" +
                    "<meta http-equiv=\"Cache-Control\" content=\"no-siteapp\" />\r\n" +
                    "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=gbk\"/>\r\n" +
                    //"<meta name=\"keywords\" content=\"满唐华彩TXT免费,正文 第1章 长安雪\"/>\r\n" +
                    //"<meta name=\"description\" content=\"爱看书吧提供了怪诞的柄早创作的玄幻小说《满唐华彩TXT免费》干净清爽无代笾的文字章节：正文 第1章 长安雪在线阅读。\"/>\r\n" +
                    "<link rel=\"stylesheet\" href=\"/themes/qu/sweet-alert.css\"/>\r\n" +
                    "<link rel=\"stylesheet\" href=\"/themes/qu/style.css?v=1.118\"/>\r\n" +
                    "<script type=\"text/javascript\" src=\"/scripts/jquery.min.js\"></script>\r\n" +
                    "<script src=\"/themes/qu/bqg.js?v=1.34\"></script>\r\n" +
                    "</head>" + 
                    strBody
                    + "\r\n</html>"
                    , bSilenceMode, atatus);
                    */
                    (new IKBook8NovelContent()).AnalysisHtmlBookBody(this, ref datacontext, strBody, bSilenceMode, atatus);
                    break;
                case BatchQueryNovelContents.QQBOOK:
                    //AnalysisHtmlQQBookBody(ref datacontext, strBody, bSilenceMode, atatus);
                    (new OOBookNovelContent()).AnalysisHtmlBookBody(this, ref datacontext, strBody, bSilenceMode, atatus);
                    break;
                case BatchQueryNovelContents.BIQUGE:
                    //AnalysisHtmlQQBookBody(ref datacontext, strBody, bSilenceMode, atatus);
                    (new BiQuGeBookNovelContent()).AnalysisHtmlBookBody(this, ref datacontext, strBody, bSilenceMode, atatus);
                    break;
                default:
                    break;
            }
            UpdateStatusMsg(datacontext, "Finished Analysing of  downloaded Uri Contents Body...");
        }

    }
}