using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace BaseBookDownloader
{
#pragma warning disable CS8604 // Null 参照引数の可能性があります。
#pragma warning disable CS8601 // Null 参照代入の可能性があります。
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning disable CS8632 // Null 参照代入の可能性があります。
    public class CangQiongBookNovelContent : BaseBookNovelContent, IFetchNovelContent
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
            HtmlNodeCollection? topDiv = body.SelectNodes(".//div[@class='reader-main']");
            if ((topDiv?.Count ?? 0) > 0)
            {
                FindBookNextLinkAndContents( wndMain, datacontext, topDiv.First(), ref nextLink, ref header, ref content, ref novelName);
                if (content != null || nextLink != null)
                {
                    string strNextLink = GetBookNextLink(wndMain, datacontext, nextLink);
                    string strChapterHeader = GetBookHeader(wndMain, datacontext, header);
                    string strContents = " \r\n \r\n " + GetBookContents(wndMain, datacontext, content);
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
            content = top?.SelectNodes(".//div[@class='content']")?.FirstOrDefault();
            header = top?.SelectNodes("//div[@class='layout-tit xs-hidden']")?.FirstOrDefault();
            HtmlNode? nextLinkDiv = top?.SelectNodes("//div[@class='section-opt']")?.FirstOrDefault();

            //<div onclick="JumpNext();" class="erzitop_"><a title="第002章 抓捕  我的谍战岁月" href="/wxread/94612_43816525.html">下一章</a> </div>
            //IEnumerable<HtmlNode>? collNext= nextLinkDiv?.Descendants().Where(n => n?.Name == "a" && (n.InnerText == "下一页" || n.InnerText == "下一章")) as IEnumerable<HtmlNode>;
            nextLink = nextLinkDiv?.Descendants().Where(n => n?.Name == "a" && (n.InnerText == "下一章" || n.InnerText.Trim() == "下一页" || n.InnerText.Trim() == "下一节"))?.FirstOrDefault();
            novelName = top?.SelectNodes("//div[@class='layout-tit xs-hidden']")?.FirstOrDefault();
        }

        public string GetBookHeader(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? header)
        {
            return header?.Attributes["href"]?.Value??"";
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
            return "http://www.cqhhhs.com" + sUrl;
        }

        public string GetBookContents(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? content, string? key = null)
        {
            StringBuilder sbContent = new StringBuilder();
            foreach (HtmlNode element in content?.ChildNodes)
            {
                //hrefTags.Add(element.GetAttribute("href"));
                if (string.Equals("#text", element.Name))
                {
                    string? strLine = ReformLine(element.InnerText);
                    if (!string.IsNullOrEmpty(strLine?.Trim()) && !string.Equals(strLine?.Trim(), "\\/阅|读|模|式|内|容|加|载|不|完|整|，退出可阅读完整内容|点|击|屏|幕|中|间可|退|出|阅-读|模|式|."))
                    {
                        sbContent.Append(strLine);
                    }
                }
                else if (string.Equals("br", element.Name))
                {
                    sbContent.Append("\r\n");
                }
            }
            return sbContent.ToString().Replace("\r\n\r\n\r\n", "\r\n").Replace("\r\n\r\n", "\r\n");
        }

        public string GetBookName(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? content)
        {
            //throw new NotImplementedException();
            return content?.SelectNodes("//a").Where(n => !string.IsNullOrEmpty(n.Attributes["title"]?.Value))?.FirstOrDefault()?.Attributes["title"]?.Value??"";
        }

        public string GetBookName2(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode content)
        {
            throw new NotImplementedException();
        }

        public bool AnalysisHtmlStream(IBaseMainWindow wndMain, BaseWndContextData datacontext, string strURL, string strBody, bool bSilenceMode = false, DownloadStatus? status = null, int nMaxRetry = 0, bool bForceDownload = false)
        {
            throw new NotImplementedException();
        }
    }
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning restore CS8601 // Null 参照代入の可能性があります。
#pragma warning restore CS8604 // Null 参照引数の可能性があります。
#pragma warning restore CS8632 // Null 参照代入の可能性があります。
}