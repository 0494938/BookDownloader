using System.Diagnostics;
using System.Windows;

namespace iKbook8
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private void AnalysisHtmlBody(ref WndContextData datacontext,ref string strBody, bool bSilenceMode=false, DownloadStatus? atatus=null)
        {
            Debug.Assert(!bSilenceMode || (bSilenceMode && atatus !=null));

            switch(datacontext.SiteType){
                case BatchQueryNovelContents.IKBOOK8:
                    AnalysisHtmlIkBookBody(ref datacontext, 
                        /*
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
                        */
                        strBody
                        // + "\r\n</html>"
                        , bSilenceMode, atatus);
                    break;
                default:
                    break;
            }
        }

    }
}