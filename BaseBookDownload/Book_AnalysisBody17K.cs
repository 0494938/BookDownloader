using BaseBookDownload;
using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace BaseBookDownload
{
#pragma warning disable CS8601 // Null 参照代入の可能性があります。
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning disable CS8604 // Null 参照引数の可能性があります。
#pragma warning disable CS8632 // Null 参照代入の可能性があります。

    public class _17KBookNovelContent : BaseBookNovelContent, IFetchNovelContent
    {
        public void AnalysisHtmlBookBody(IBaseMainWindow wndMain, BaseWndContextData datacontext, string strUrl, string strBody, bool bSilenceMode = false, DownloadStatus? status = null, int nMaxRetry = 0)
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
                return;
            }

            HtmlNode? nextLink = null;
            HtmlNode? content = null;
            HtmlNode? header = null;
            HtmlNode? topDiv = body?.SelectNodes(".//div[@class='area']")?.FirstOrDefault();
            if (topDiv != null)
            {
                FindBookNextLinkAndContents(wndMain, datacontext, topDiv, ref nextLink, ref header, ref content);
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
                    return;
                }
            }
        }
        public void FindBookNextLinkAndContents(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? top, ref HtmlNode? nextLink, ref HtmlNode? header, ref HtmlNode?  content)
        {
            content = top?.SelectNodes(".//div[@class='readAreaBox content']").FirstOrDefault();

            header = content?.SelectNodes(".//h1").FirstOrDefault();
            content = content?.SelectNodes(".//div[@class='p']").FirstOrDefault();

            nextLink = top?.SelectNodes(".//div[@class='read_tit']").FirstOrDefault();
        }

        public string GetBookHeader(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? header)
        {
            if (header != null)
                return header.InnerText;
            return "";
        }

        public string GetBookNextLink(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? nextLink)
        {

            if(nextLink != null)
            {
                HtmlNode? a = nextLink.SelectNodes(".//a[@class='nextChapter']")?.Where(n=> n.InnerText.Trim() == "下一章" || n.InnerText.Trim() == "下一页" || n.InnerText.Trim() == "下一页")?.FirstOrDefault();
                string ?sLink = a?.Attributes["href"]?.Value;
                if (sLink?.StartsWith("http")??false)
                {
                    return sLink;
                }
                return "https://www.17k.com" + sLink;
            }

            return "";

            //return "https://www.xs8.cn" + sUrl;
        }

        public string GetBookContents(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? content)
        {
            if (content != null)
            {
                StringBuilder sbContent = new StringBuilder();
                foreach (HtmlNode element in content?.ChildNodes)
                {
                    if(element.Name == "p")
                    {
                        string? strLine = element.InnerText?.Replace("\r", "")?.Replace("\n", "")?.Replace("&nbsp;", " ")?.Replace("&lt;", "<")?.Replace("&gt;", ">")?.Replace("&amp;", "&")?
                            .Replace("&ensp;", " ")?.Replace("&emsp;", " ")?.Replace("&ndash;", " ")?.Replace("&mdash;", " ")?
                            .Replace("&sbquo;", "“")?.Replace("&rdquo;", "”")?.Replace("&bdquo;", "„")?
                            .Replace("&quot;", "\"")?.Replace("&circ;", "ˆ")?.Replace("&tilde;", "˜")?.Replace("&prime;", "′")?.Replace("&Prime;", "″");
                        if (!string.IsNullOrEmpty(strLine?.Trim()))
                        {
                            sbContent.Append(strLine).AppendLine();
                        }
                    }
                }
                return sbContent.ToString().Replace("\r\n\r\n\r\n", "\r\n").Replace("\r\n\r\n", "\r\n");
            }

            return "";
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
#pragma warning restore CS8601 // Null 参照代入の可能性があります。
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning restore CS8604 // Null 参照引数の可能性があります。
#pragma warning restore CS8632 // Null 参照代入の可能性があります。
}