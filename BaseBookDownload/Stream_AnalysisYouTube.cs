using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.Linq;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace BaseBookDownloader
{
#pragma warning disable CS8601 // Null 参照代入の可能性があります。
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning disable CS8604 // Null 参照引数の可能性があります。
#pragma warning disable CS8632 // Null 参照代入の可能性があります。

    public class YouTubeStreamPageContent : BaseBookNovelContent, IFetchNovelContent
    {
        public bool AnalysisHtmlBook(IBaseMainWindow wndMain, BaseWndContextData datacontext, string strUrl, string strBody, bool bSilenceMode = false, DownloadStatus? status = null, int nMaxRetry = 0)
        {
            return true;
        }
        
        private void FinishDocAnalyis(IBaseMainWindow wndMain, BaseWndContextData datacontext, string strURL, HtmlNode? nextLink, HtmlNode? header, HtmlNode? content, HtmlNode? novelName, bool bSilenceMode, DownloadStatus? status = null)
        {
            string strCurMp4FileLink = GetBookNextLink(wndMain, datacontext, nextLink);
            string strChapterHeader = GetBookHeader(wndMain, datacontext, header);
            string strContents = GetBookContents(wndMain, datacontext, content);
            string strNovelName = strChapterHeader;
            ParseResultToUI(wndMain, datacontext, bSilenceMode, strContents, strCurMp4FileLink, strChapterHeader, strNovelName);
            wndMain.UpdateStatusMsg(datacontext, strURL + " : Finished Analysing of downloaded Uri Contents Body ...", 100);
            wndMain.DownloadFile(datacontext, strCurMp4FileLink, strChapterHeader);

            return;
        }

        public void FindBookNextLinkAndContents(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? top, ref HtmlNode? curIdentifier, ref HtmlNode? header, ref HtmlNode?  content, ref HtmlNode novelName)
        {
            content = top?.SelectNodes(".//script")?.Descendants().Where(n => n.InnerText.StartsWith("var ytInitialData =")).FirstOrDefault();

            curIdentifier = top?.SelectNodes(".//div[@class='watch-main-col']")?.Descendants().Where(n => n.Name == "meta" && n.Attributes["itemprop"]?.Value == "identifier").FirstOrDefault();

            novelName = top?.SelectNodes("//head").Descendants().Where(n => n.Name == "title")?.FirstOrDefault();
            header = novelName;
        }

        public string GetBookHeader(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? header)
        {
            if (header != null)
                return header.InnerText.Trim().Replace(" - YouTube", "").Replace('\\', '￥').Replace('#', '＃')
                    .Replace('$', '＄').Replace('%', '％').Replace('!', '！').Replace('&', '＆').Replace('\'', '’').Replace('{', '｛')
                    .Replace('\"', '”').Replace('}', '｝').Replace(':', '：').Replace('\\', '￥').Replace('@', '＠').Replace('<', '＜').Replace('>', '＞').Replace('+', '＋')
                    .Replace('`', '‘').Replace('*', '＊').Replace('|', '｜').Replace('?', '？').Replace('=', '＝').Replace('/', '／');
            return "";
        }

        public string GetBookNextLink(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? currIdentifer)
        {

            if(currIdentifer != null)
            {
                string ?sLink = currIdentifer?.Attributes["content"]?.Value;
                if (!string.IsNullOrEmpty(sLink))
                    return "https://www.youtube.com/embed/" + sLink + ".mp4";
                else
                    return "";
            }

            return "";
        }

        public string GetBookContents(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? content, string? key = null)
        {
            string? strJsonScript = content?.InnerText?.Substring("var ytInitialData =".Length);
            strJsonScript = strJsonScript?.TrimEnd();
            strJsonScript = strJsonScript?.TrimEnd(';');
            return strJsonScript??"";
        }

        public string GetBookName(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? bookName)
        {
            //throw new NotImplementedException();
            return bookName?.InnerText??"" ;
        }

        public string GetBookName2(HtmlNode content)
        {
            throw new NotImplementedException();
        }

        public bool AnalysisHtmlStream(IBaseMainWindow wndMain, BaseWndContextData datacontext, string strURL, string strBody, bool bSilenceMode = false, DownloadStatus? status = null, int nMaxRetry = 0, bool bForceDownload = false)
        {
            this.URL = strURL;

            Debug.Assert(!bSilenceMode || (bSilenceMode && status != null));
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(strBody);
            HtmlNode? body = GetHtmlBody(html);

            if (body == null)
            {
                wndMain.UpdateStatusMsg(datacontext, "*** URL downloaded BODY is empty, skip this Page *** ", 0);
                return true;
            }

            HtmlNode? currIdentifier = null;
            HtmlNode? content = null;
            HtmlNode? header = null;
            HtmlNode? novelName = null;
            if (body != null)
            {
                FindBookNextLinkAndContents(wndMain, datacontext, body, ref currIdentifier, ref header, ref content, ref novelName);
                if (currIdentifier != null)
                {
                    FinishDocAnalyis(wndMain, datacontext, strURL, currIdentifier, header, content, novelName, bSilenceMode, status);
                    return true;
                }
                
            }
            return true;
        }
    }
#pragma warning restore CS8601 // Null 参照代入の可能性があります。
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning restore CS8604 // Null 参照引数の可能性があります。
#pragma warning restore CS8632 // Null 参照代入の可能性があります。
}