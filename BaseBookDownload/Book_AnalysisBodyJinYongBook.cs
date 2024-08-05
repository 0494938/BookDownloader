using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.Linq;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace BaseBookDownloader
{
#pragma warning disable CS8604 // Null 参照引数の可能性があります。
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning disable CS8601 // Null 参照代入の可能性があります。
#pragma warning disable CS8632 // Null 参照代入の可能性があります。
    public class JinYongBookNovelContent : BaseBookNovelContent, IFetchNovelContent
    {
        public bool AnalysisHtmlBook(IBaseMainWindow wndMain, BaseWndContextData datacontext, string strUrl, string strBody, bool bSilenceMode = false, DownloadStatus? status = null, int nMaxRetry = 0)
        {
            this.URL = strUrl;

            Debug.Assert(!bSilenceMode || (bSilenceMode && status != null));
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(strBody);
            HtmlNode? body = GetHtmlBody(html);

            if (body == null)
            {
                wndMain.UpdateStatusMsg(datacontext, "*** URL downloaded BODY is empty, skip this Page *** ", 0);
                return true;
            }

            HtmlNode? nextLink = null;
            HtmlNode? content = null;
            HtmlNode? header = null;
            HtmlNode? novelName = null;
            HtmlNode? topDiv = body.SelectNodes(".//div[@class='read_bg']")?.FirstOrDefault();
            if (topDiv != null)
            {
                FindBookNextLinkAndContents(wndMain, datacontext, topDiv, ref nextLink, ref header, ref content, ref novelName);
                if (content != null || nextLink != null)
                {
                    string strNextLink = GetBookNextLink(wndMain, datacontext, nextLink);
                    string strChapterHeader = GetBookHeader(wndMain, datacontext, header);
                    string strContents = " \r\n \r\n " + strChapterHeader + " \r\n" + GetBookContents(wndMain, datacontext, content);
                    string strNovelName = GetBookName(wndMain, datacontext, novelName);
                    ParseResultToUI(wndMain, datacontext, bSilenceMode, strContents, strNextLink, strChapterHeader, strNovelName);

                    if (bSilenceMode)
                    {
                        Debug.Assert(status != null);
                        status.NextUrl = strNextLink;

                        WriteToFile(status, strChapterHeader, strContents, strNextLink, strNovelName);
                    }
                    datacontext.NextLinkAnalysized = !string.IsNullOrEmpty(strNextLink);
                    wndMain.UpdateNextPageButton();
                }
            }
            return true;
        }


        public void FindBookNextLinkAndContents(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? top, ref HtmlNode? nextLink, ref HtmlNode? header, ref HtmlNode? content, ref HtmlNode novelName)
        {
            content = top?.SelectNodes("//article[@class='content']")?.FirstOrDefault();

            header = top?.SelectNodes("//p[@class='style_h1']")?.FirstOrDefault();

            HtmlNode? nextLinkDiv = top?.SelectNodes(".//div[@class='read_nav']")?.FirstOrDefault();

            //<a id="next_url" href="/b/184315/976061_2.html"><i class="fa fa-forward"></i> 下一页</a>
            //HtmlNodeCollection? collNextARef = collNext.First()?.SelectNodes(".//a");
            nextLink = nextLinkDiv?.SelectNodes(".//a[@id='next_url']")?.FirstOrDefault();
            novelName = top?.SelectNodes("//head/meta[@property='og:title']")?.FirstOrDefault();
        }

        public string GetBookHeader(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? header)
        {
            if (header != null)
                return header.InnerText;
            return "";
        }

        public string GetBookNextLink(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? nextLink)
        {
            string sUrl = nextLink?.Attributes["href"]?.Value??"";
            if (sUrl.StartsWith("http"))
                return sUrl;
            if (sUrl.StartsWith("www"))
            {
                return "https://" + sUrl;
            }
            return "http://www.jinhuaja.com" + sUrl;
        }

        public string GetBookContents(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? content, string? key = null)
        {
            if (content != null)
                return ReformContent(content?.InnerText??"")??""; 
            else
                return "";
        }

        public string GetBookName(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? content)
        {
            //throw new NotImplementedException();
            return content?.Attributes["content"]?.Value??"";
        }

        public string GetBookName2(HtmlNode content)
        {
            throw new NotImplementedException();
        }

        public bool AnalysisHtmlStream(IBaseMainWindow wndMain, BaseWndContextData datacontext, string strURL, string strBody, bool bSilenceMode = false, DownloadStatus? status = null, int nMaxRetry = 0, bool bForceDownload = false)
        {
            throw new NotImplementedException();
        }

    }
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning restore CS8604 // Null 参照引数の可能性があります。
#pragma warning restore CS8601 // Null 参照代入の可能性があります。
#pragma warning restore CS8632 // Null 参照代入の可能性があります。
}