using BaseBookDownloader;
using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace BaseBookDownloader
{
#pragma warning disable CS8601 // Null 参照代入の可能性があります。
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning disable CS8604 // Null 参照引数の可能性があります。
#pragma warning disable CS8632 // Null 参照代入の可能性があります。

    public class YQXSBBookNovelContent : BaseBookNovelContent, IFetchNovelContent
    {
        public bool AnalysisHtmlBookBody(IBaseMainWindow wndMain, BaseWndContextData datacontext, string strUrl, string strBody, bool bSilenceMode = false, DownloadStatus? status = null, int nMaxRetry = 0)
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
            HtmlNode? topDiv = body?.SelectNodes(".//div[@id='j_readMainWrap'][@class='read-main-wrap font-family01']")?.FirstOrDefault();
            if (topDiv != null)
            {
                FindBookNextLinkAndContents(wndMain, datacontext, topDiv, ref nextLink, ref header, ref content, ref novelName);
                if (content != null || nextLink != null)
                {
                    string strNextLink = GetBookNextLink(wndMain, datacontext, nextLink);
                    string strChapterHeader = GetBookHeader(wndMain, datacontext, header);
                    string strContents = " \r\n \r\n " + strChapterHeader + " \r\n" + GetBookContents(wndMain, datacontext, content);
                    string strNovelName = GetBookName(wndMain, datacontext, novelName);
                    ParseResultToUI(wndMain, bSilenceMode, strContents, strNextLink, strNovelName);

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
        public void FindBookNextLinkAndContents(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? top, ref HtmlNode? nextLink, ref HtmlNode? header, ref HtmlNode?  content, ref HtmlNode novelName)
        {
            content = top?.SelectNodes(".//div[@class='ywskythunderfont']").FirstOrDefault();
            header = top?.SelectNodes(".//h1[@class='j_chapterName']").FirstOrDefault();
            nextLink = top?.SelectNodes(".//a[@id='j_chapterNext']").FirstOrDefault();
            novelName = top?.SelectNodes("//h2[@class='last-nav act']").FirstOrDefault();
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
                return "https://www.xs8.cn" + nextLink?.Attributes["href"]?.Value ;
            }

            return "";
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
                        string? strLine = ReformLine(element.InnerText);
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
            //throw new NotImplementedException();
            return content?.InnerText??"";
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