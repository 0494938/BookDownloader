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
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning disable CS8601 // Null 参照代入の可能性があります。
#pragma warning disable CS8632 // Null 参照代入の可能性があります。
    public class XXSBBookNovelContent : BaseBookNovelContent, IFetchNovelContent
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
            HtmlNode? topDiv = body.SelectNodes(".//div[@class='read-main'][@id='read-main']")?.FirstOrDefault();
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
                }
            }
            return true;
        }

        public void FindBookNextLinkAndContents(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? top, ref HtmlNode? nextLink, ref HtmlNode? header, ref HtmlNode? content)
        {
            content = top?.SelectNodes(".//div[@class='read-content'][@id='read-content']")?.FirstOrDefault();
            header = content?.SelectNodes(".//h2").FirstOrDefault();
            nextLink = top?.SelectNodes(".//div[@class='read-page']")?.FirstOrDefault();
        }

        public string GetBookHeader(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? header)
        {
            if (header != null)
                return header.InnerText;
            return "";
        }

        public string GetBookNextLink(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? nextLink)
        {
            if (nextLink != null)
            {
                HtmlNode?  aNext = nextLink?.SelectNodes(".//a[@id='nextChapterBtn']")?.FirstOrDefault();
                return aNext?.Attributes["href"]?.Value??"";
            }
            return "";
        }

        public string GetBookContents(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? content)
        {
            if (content != null)
            {
                IEnumerable<HtmlNode>? collContentP = content?.Descendants().Where(n => n.Name == "p");
                if (collContentP!=null && collContentP?.Count()>0)
                {
                    StringBuilder sbContent= new StringBuilder();
                    foreach (HtmlNode element in collContentP)
                    {
                        string? strLine = ReformLine(element.InnerText);
                        if (!string.IsNullOrEmpty(strLine?.Trim()))
                        {
                            sbContent.Append(strLine).AppendLine();
                        }
                    }
                    return sbContent.ToString().Replace("\r\n\r\n\r\n", "\r\n").Replace("\r\n\r\n", "\r\n");
                }
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
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning restore CS8604 // Null 参照引数の可能性があります。
#pragma warning restore CS8601 // Null 参照代入の可能性があります。
#pragma warning restore CS8632 // Null 参照代入の可能性があります。
}