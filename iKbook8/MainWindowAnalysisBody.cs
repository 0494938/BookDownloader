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

        public void AnalysisHtmlBodyThreadFunc(WndContextData? datacontext, string strURL, string strBody, bool bSilenceMode=false, DownloadStatus? status = null)
        {
            Debug.Assert(!bSilenceMode || (bSilenceMode && status != null));
            /*
            if (nDelayMiliSeconds > 0) { 
                Thread.Sleep(nDelayMiliSeconds);
            }
            */

            IFetchNovelContent? fetchNovelContent = null;
            int nMaxRetry = 60; //span is 3s.
            switch (datacontext?.SiteType)
            {
                case BatchQueryNovelContents.IKBOOK8:
                    fetchNovelContent = new IKBook8NovelContent();
                    break;
                case BatchQueryNovelContents.QQBOOK:
                    fetchNovelContent = new OOBookNovelContent();
                    break;
                case BatchQueryNovelContents.WXDZH:
                    fetchNovelContent = new WxdzsBookNovelContent();
                    break;
                case BatchQueryNovelContents.CANGQIONG:
                    fetchNovelContent = new CangQiongBookNovelContent();
                    break;
                case BatchQueryNovelContents.JINYONG:
                    fetchNovelContent = new JinYongBookNovelContent();
                    break;
                case BatchQueryNovelContents.SHUQI:
                    nMaxRetry = 60;
                    fetchNovelContent = new ShuQiBookNovelContent();
                    break;
                case BatchQueryNovelContents.FANQIE:
                case BatchQueryNovelContents.FANQIE2:
                    nMaxRetry = 60;
                    fetchNovelContent = new FanQieBookNovelContent();
                    break;
                case BatchQueryNovelContents.BIQUGE:
                case BatchQueryNovelContents.BIQUGE2:
                    fetchNovelContent = new BiQuGeBookNovelContent();
                    break;
                default:
                    break;
            }

            if (bSilenceMode)
            {
                UpdateStatusMsg(datacontext, strURL + " : Begin to Analysize downloaded Contents Body using " + datacontext?.SiteType.ToString() + "<" + fetchNovelContent?.GetType()?.Name + "> ...", (int)((100.0 / DownloadStatus.ThreadMax * (status?.ThreadNum??1 -1+0.5))));
            }
            else
                UpdateStatusMsg(datacontext, strURL+ " : Begin to Analysize downloaded Contents Body using " + datacontext?.SiteType.ToString() + "<" + fetchNovelContent?.GetType()?.Name + "> ...", 50);

            if(fetchNovelContent!=null)
            {
                    fetchNovelContent.AnalysisHtmlBookBody(this, datacontext, strBody, bSilenceMode, status, nMaxRetry);
            }
            if (bSilenceMode)
            {
                UpdateStatusMsg(datacontext, strURL + " : Finished Analysing of downloaded Uri Contents Body of No." + status?.ThreadNum ?? 1 + " ...", (int)((100.0 / DownloadStatus.ThreadMax * (status?.ThreadNum??1))));
            }
            else
                UpdateStatusMsg(datacontext, strURL + " : Finished Analysing of downloaded Uri Contents Body of No. " + status?.ThreadNum ?? 1 + "...", 100);

            if (bSilenceMode)
            {
                if (status?.ThreadNum < DownloadStatus.ThreadMax && !string.IsNullOrEmpty(status.NextUrl))
                {
                    DownloadOneURLAndGetNext(datacontext, status.NextUrl);
                }else
                {
                    DownloadStatus.ContentsWriter = null;
                    UpdateStatusMsg(datacontext, "Finished batch download(Total " + status?.ThreadNum + " Downloaded) ...", 100);
                    this.Dispatcher.Invoke(() =>
                    {
                        if (!string.IsNullOrEmpty(status?.NextUrl))
                            txtInitURL.Text = status.NextUrl;
                        MessageBox.Show(this, "Batch download finished...", "Web Novel Downloader", MessageBoxButton.OK);
                    });
                }
            }
        }
    }
}