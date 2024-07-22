using BaseBookDownload;
using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.Linq;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace BaseBookDownload
{
#pragma warning disable CS8601 // Null 参照代入の可能性があります。
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning disable CS8632 // Null 参照代入の可能性があります。
    public class WxdzsBookNovelContent : BaseBookNovelContent, IFetchNovelContent
    {
        public bool AnalysisHtmlBookBody(IBaseMainWindow wndMain, BaseWndContextData datacontext, string strUrl, string strBody, bool bSilenceMode = false, DownloadStatus? status = null, int nMaxRetry = 0)
        {
            this.URL = strUrl;

            Debug.Assert(!bSilenceMode || (bSilenceMode && status != null));
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(strBody);
            //HtmlNode body = html.DocumentNode.ChildNodes["BODY"];
            HtmlNode? body = GetHtmlBody(html);

            if (body == null)
            {
                wndMain.UpdateStatusMsg(datacontext, "*** URL downloaded BODY is empty, skip this Page *** ", 0);
                return true;
            }

            HtmlNode? nextLink = null;
            HtmlNode? content = null;
            HtmlNode? header = null;
            HtmlNodeCollection? topDiv = body.SelectNodes(".//div[@id='PageBody']");
            if ((topDiv?.Count ?? 0) > 0)
            {
                FindBookNextLinkAndContents(wndMain, datacontext, topDiv?.First(),  ref nextLink, ref header, ref content);
                if (content != null || nextLink != null)
                {
                    string strNextLink = GetBookNextLink(wndMain, datacontext, nextLink);
                    string strChapterHeader = GetBookHeader(wndMain, datacontext, header);
                    string strContents = " \r\n \r\n " + strChapterHeader + " \r\n" + GetBookContents(wndMain, datacontext, content);

                    ParseResultToUI(wndMain, bSilenceMode, strContents, strNextLink);

                    if (bSilenceMode)
                    {
                        Debug.Assert(status != null);
                        status.NextUrl = strNextLink;

                        DownloadStatus.ContentsWriter?.Write(strContents);
                        DownloadStatus.ContentsWriter?.Flush();
                    }
                    datacontext.NextLinkAnalysized = !string.IsNullOrEmpty(strNextLink);
                    wndMain.UpdateNextPageButton();
                }
            }
            return true;
        }

        public void FindBookNextLinkAndContents(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? top, ref HtmlNode? nextLink, ref HtmlNode? header, ref HtmlNode? content)
        {
            HtmlNodeCollection ?collCont = top?.SelectNodes(".//div[@id='Lab_Contents']");
            content = collCont?.First();

            HtmlNodeCollection? collHeader = top?.SelectNodes(".//h1[@id='ChapterTitle']");
            header = collHeader?.First();

            HtmlNodeCollection? collNextDiv = top?.SelectNodes(".//div[@id='Pan_Top']");
            HtmlNode? nextLinkDiv = collNextDiv?.First();

            //<div onclick="JumpNext();" class="erzitop_"><a title="第002章 抓捕  我的谍战岁月" href="/wxread/94612_43816525.html">下一章</a> </div>
            HtmlNodeCollection? collNext= nextLinkDiv?.SelectNodes(".//div[@onclick='JumpNext();']");
            HtmlNodeCollection? collNextARef = collNext?.First()?.SelectNodes(".//a");
            nextLink = collNextARef?.First();
        }

        public string GetBookHeader(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? header)
        {
            return header?.InnerText??"";
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
            return "https://www.wxdzs.net" + sUrl;
        }

        public string GetBookContents(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? content)
        {
            return ReformContent(content?.InnerText??"")??"";
        }

        public string GetBookName(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? content)
        {
            throw new NotImplementedException();
        }

        public string GetBookName2(HtmlNode content)
        {
            throw new NotImplementedException();
        }
    }
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning restore CS8601 // Null 参照代入の可能性があります。
#pragma warning restore CS8632 // Null 参照代入の可能性があります。
}