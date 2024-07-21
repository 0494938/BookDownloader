using BaseBookDownload;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace BaseBookDownload
{
#pragma warning disable CS8604 // Null 参照引数の可能性があります。
#pragma warning disable CS8601 // Null 参照代入の可能性があります。
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning disable CS8632 // Null 参照代入の可能性があります。
    public class CangQiongBookNovelContent : BaseBookNovelContent, IFetchNovelContent
    {
        
        public void AnalysisHtmlBookBody(IBaseMainWindow wndMain, BaseWndContextData datacontext, string strUrl, string strBody, bool bSilenceMode = false, DownloadStatus? status = null, int nMaxRetry = 0)
        {
            this.URL = strUrl;

            Debug.Assert(!bSilenceMode || (bSilenceMode && status != null));
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(strBody);
            HtmlNode body = html.DocumentNode.ChildNodes["BODY"];

            if (body == null)
            {
                wndMain.UpdateStatusMsg(datacontext, "*** URL downloaded BODY is empty, skip this Page *** ", 0);
                return;
            }

            HtmlNode? nextLink = null;
            HtmlNode? content = null;
            HtmlNode? header = null;
            HtmlNodeCollection? topDiv = body.SelectNodes(".//div[@class='reader-main']");
            if ((topDiv?.Count ?? 0) > 0)
            {
                FindBookNextLinkAndContents(topDiv.First(), ref nextLink, ref header, ref content);
                if (content != null || nextLink != null)
                {
                    string strNextLink = GetBookNextLink(nextLink);
                    //string strChapterHeader = GetBookHeader(header);
                    string strContents = " \r\n \r\n " + GetBookContents(content);

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
                    return;
                }
            }
        }

        public void FindBookNextLinkAndContents(HtmlNode? top, ref HtmlNode nextLink, ref HtmlNode header, ref HtmlNode content)
        {
            HtmlNodeCollection? collCont = top?.SelectNodes(".//div[@class='content']");
            content = collCont?.First();

            HtmlNodeCollection? collHeader = top?.SelectNodes("//div[@class='layout-tit xs-hidden']");
            header = collHeader?.First();

            HtmlNodeCollection? collNextDiv = top?.SelectNodes("//div[@class='section-opt']");
            HtmlNode? nextLinkDiv = collNextDiv?.First();

            //<div onclick="JumpNext();" class="erzitop_"><a title="第002章 抓捕  我的谍战岁月" href="/wxread/94612_43816525.html">下一章</a> </div>
            IEnumerable<HtmlNode>? collNext= nextLinkDiv?.Descendants().Where(n => n?.Name == "a" && (n.InnerText == "下一页" || n.InnerText == "下一章")) as IEnumerable<HtmlNode>;
            nextLink = collNext?.First();
        }

        public string GetBookHeader(HtmlNode? header)
        {
            return header?.Attributes["href"]?.Value??"";
        }

        public string GetBookNextLink(HtmlNode? nextLink)
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

        public string GetBookContents(HtmlNode? content)
        {
            StringBuilder sbContent = new StringBuilder();
            foreach (HtmlNode element in content?.ChildNodes)
            {
                //hrefTags.Add(element.GetAttribute("href"));
                if (string.Equals("#text", element.Name))
                {
                    string? strLine = element.InnerText?.Replace("\r", "")?.Replace("\n", "")?.Replace("&nbsp;", " ")?.Replace("&lt;", "<")?.Replace("&gt;", ">")?.Replace("&amp;", "&")?
                        .Replace("&ensp;", " ")?.Replace("&emsp;", " ")?.Replace("&ndash;", " ")?.Replace("&mdash;", " ")?
                        .Replace("&sbquo;", "“")?.Replace("&rdquo;", "”")?.Replace("&bdquo;", "„")?
                        .Replace("&quot;", "\"")?.Replace("&circ;", "ˆ")?.Replace("&tilde;", "˜")?.Replace("&prime;", "′")?.Replace("&Prime;", "″");
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
            return sbContent.ToString().Replace("\r\n\r\n", "\r\n");
        }

        public string GetBookName(HtmlNode? content)
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
#pragma warning restore CS8604 // Null 参照引数の可能性があります。
#pragma warning restore CS8632 // Null 参照代入の可能性があります。
}