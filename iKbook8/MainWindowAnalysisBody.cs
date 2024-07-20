using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Windows;

namespace BookDownloader
{
    public interface IFetchNovelContent {
        public void AnalysisHtmlBookBody(MainWindow? wndMain, WndContextData? datacontext, string strBody, bool bSilenceMode = false, DownloadStatus? status = null, int nMaxRetry = 0);
        public void FindBookNextLinkAndContents(HtmlNode? parent, ref HtmlNode nextLink, ref HtmlNode header, ref HtmlNode content);
        public string GetBookHeader(HtmlNode? header);
        public string GetBookNextLink(HtmlNode? nextLink);
        public string GetBookContents(HtmlNode? content);
        public string GetBookName(HtmlNode? content);
    }

    public class BaseBookNovelContent
    {
        public static JToken? GetValueByKeyFromJObject(JObject jObj, string sKey)
        {
            JToken? value;
            if (jObj.TryGetValue(sKey, out value))
            {
                return value;
            }
            return null;
        }


        protected void ParseResultToUI(MainWindow wndMain, bool bSilenceMode, string strContents, string strNextLink)
        {
            wndMain.txtAnalysizedContents.Text = strContents;
            wndMain.txtNextUrl.Text = strNextLink;
            wndMain.txtCurURL.Text = strNextLink;
            if (bSilenceMode)
            {
                if (wndMain.txtAggregatedContents.Text.Length > 1024 * 64)
                    wndMain.txtAggregatedContents.Text = strContents;
                else
                    wndMain.txtAggregatedContents.Text += strContents;
            }
            else
            {
                wndMain.txtAggregatedContents.Text += strContents;
            }
            wndMain.txtAggregatedContents.ScrollToEnd();
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public void AnalysisHtmlBody(WndContextData? datacontext, bool bWaitOption, string strURL, string strBody, bool bSilenceMode = false, DownloadStatus? status = null)
        {
            Debug.Assert(datacontext != null);
            Thread thread = new Thread(() => AnalysisHtmlBodyThreadFunc(datacontext, strURL, strBody, bSilenceMode, status));
            thread.Start();
        }
    }
}