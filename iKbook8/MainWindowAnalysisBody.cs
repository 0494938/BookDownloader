using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Windows;

namespace BookDownloader
{
    public interface IFetchNovelContent
    {
        public void AnalysisHtmlBookBody(WPFMainWindow? wndMain, WndContextData? datacontext, string strURL, string strBody, bool bSilenceMode = false, DownloadStatus? status = null, int nMaxRetry = 0);
        protected void FindBookNextLinkAndContents(HtmlNode? parent, ref HtmlNode nextLink, ref HtmlNode header, ref HtmlNode content);
        protected string GetBookHeader(HtmlNode? header);
        protected string GetBookNextLink(HtmlNode? nextLink);
        protected string GetBookContents(HtmlNode? content);
        public string GetBookName(HtmlNode? content);
    }
    public class BaseBookNovelContent
    {
        protected string? URL { get; set; } = null;
        public static JToken? GetValueByKeyFromJObject(JObject jObj, string sKey)
        {
            JToken? value;
            if (jObj.TryGetValue(sKey, out value))
            {
                return value;
            }
            return null;
        }


        protected void ParseResultToUI(WPFMainWindow wndMain, bool bSilenceMode, string strContents, string strNextLink)
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
    public partial class WPFMainWindow : BaseDownloadWnd
    {
        public void AnalysisHtmlBody(WndContextData? datacontext, bool bWaitOption, string strURL, string strBody, bool bSilenceMode = false, DownloadStatus? status = null)
        {
            Debug.Assert(datacontext != null);
            try
            {
                Thread thread = new Thread(() => AnalysisHtmlBodyThreadFunc(datacontext, this, strURL, strBody, bSilenceMode, status));
                thread.Start();
            }
            catch (TaskCanceledException)
            {
                //ignore TaskCanceledException
            }
            //AnalysisHtmlBodyThreadFunc(datacontext, this, strURL, strBody, bSilenceMode, status);
        }
    }
}