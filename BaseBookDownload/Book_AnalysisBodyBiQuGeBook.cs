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
#pragma warning disable CS8601 // Null 参照代入の可能性があります。
#pragma warning disable CS8632 // Null 参照代入の可能性があります。
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
    public class BiQuGeBookNovelContent : BaseBookNovelContent, IFetchNovelContent
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
            HtmlNodeCollection? topDiv = body.SelectNodes("//div[@class='content_read']");
            if ((topDiv?.Count??0) > 0)
            {
                Debug.Assert(topDiv.Count ==1);
                FindBookNextLinkAndContents2(body, ref nextLink, ref header, ref content);
                if (content != null || nextLink != null)
                {
                    string strNextLink = GetBookNextLink2(nextLink);
                    string strChapterHeader = GetBookHeader2(header);
                    string strContents = " \r\n \r\n " + strChapterHeader + " \r\n" + GetBookContents2(content);

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
            else
            {
                FindBookNextLinkAndContents(body, ref nextLink, ref header, ref content);
                if (content != null || nextLink != null)
                {
                    string strNextLink = GetBookNextLink(nextLink);
                    string strChapterHeader = GetBookHeader(header);
                    string strContents = " \r\n \r\n " + strChapterHeader + " \r\n" + GetBookContents(content);

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

        public void FindBookNextLinkAndContents(HtmlNode? parent, ref HtmlNode nextLink, ref HtmlNode header, ref HtmlNode content)
        {
            foreach (HtmlNode element in parent?.ChildNodes)
            {
                //hrefTags.Add(element.GetAttribute("href"));
                if (string.Equals("div", element.Name))
                {
                    if (string.Equals(element.Attributes["class"]?.Value, "nr_nr") && string.Equals(element.Attributes["id"]?.Value, "nr"))
                    {
                        content = element;
                    }
                    else if (string.Equals(element.Attributes["class"]?.Value, "nr_page") )
                    {
                        nextLink = element;
                    }
                    else
                    {
                        FindBookNextLinkAndContents(element, ref nextLink, ref header, ref content);
                    }
                }
                else if (string.Equals("h1", element.Name))
                {
                    if (string.Equals(element.Attributes["class"]?.Value, "nr_title") && string.Equals(element.Attributes["id"]?.Value, "nr_title"))
                    {
                        header = element;
                    }
                }
            }
        }

        public void FindBookNextLinkAndContents2(HtmlNode? parent, ref HtmlNode nextLink, ref HtmlNode header, ref HtmlNode content)
        {
            foreach (HtmlNode element in parent?.ChildNodes)
            {
                //hrefTags.Add(element.GetAttribute("href"));
                if (string.Equals("div", element.Name))
                {
                    if (string.Equals(element.Attributes["name"]?.Value, "content") && string.Equals(element.Attributes["id"]?.Value, "content"))
                    {
                        content = element;
                    }
                    else if (string.Equals(element.Attributes["class"]?.Value, "bottem"))
                    {
                        nextLink = element;
                    }
                    else if (string.Equals(element.Attributes["class"]?.Value, "con_top"))
                    {
                        header = element;
                    }
                    else
                    {
                        FindBookNextLinkAndContents2(element, ref nextLink, ref header, ref content);
                    }
                }
            }
        }

        public string GetBookHeader(HtmlNode? header)
        {
            return header?.InnerText??"";
        }

        public string GetBookHeader2(HtmlNode? header)
        {
            string sHeader="";
            foreach (HtmlNode element in header?.ChildNodes)
            {
                if (string.Equals("#text", element.Name))
                {
                    sHeader += element.InnerText?.Replace("\r", "")?.Replace("\n", "")?.Replace("&nbsp;", " ")?.Replace("&lt;", "<")?.Replace("&gt;", ">")?.Replace("&amp;", "&")?
                        .Replace("&ensp;", " ")?.Replace("&emsp;", " ")?.Replace("&ndash;", " ")?.Replace("&mdash;", " ")?
                        .Replace("&sbquo;", "“")?.Replace("&rdquo;", "”")?.Replace("&bdquo;", "„")?
                        .Replace("&quot;", "\"")?.Replace("&circ;", "ˆ")?.Replace("&tilde;", "˜")?.Replace("&prime;", "′")?.Replace("&Prime;", "″") ;
                }
            }
            return sHeader;
        }

        public string GetBookNextLink(HtmlNode? nextLink)
        {
            IEnumerable<HtmlNode> ?lstNodes = nextLink?.Descendants().Where(n => n?.Name == "td" && n.Attributes["class"]?.Value == "mulu") as IEnumerable<HtmlNode>;
            IEnumerable<HtmlNode> ?nxtNodes = nextLink?.Descendants().Where(n => n?.Name == "td" && n.Attributes["class"]?.Value == "next") as IEnumerable<HtmlNode>;
            Debug.Assert(lstNodes?.Count()==1 && nxtNodes?.Count() ==1);

            IEnumerable< HtmlNode > aLstNode = lstNodes.First().Descendants().Where(n => n?.Name == "a" && n.Attributes["id"]?.Value == "pb_mulu") as IEnumerable<HtmlNode>;
            IEnumerable<HtmlNode> aNxtNode = nxtNodes.First().Descendants().Where(n => n?.Name == "a" && n.Attributes["id"]?.Value == "pb_next") as IEnumerable<HtmlNode>;

            Debug.Assert(aLstNode?.Count() == 1 && aNxtNode?.Count() == 1);
            string? sLst = aLstNode?.First()?.Attributes["href"]?.Value;
            string? sNxt = aNxtNode?.First()?.Attributes["href"]?.Value;
            return "https://m.xbiqugew.com/book" + sLst?.Replace("chapters_","") + sNxt;
        }

        public string GetBookNextLink2(HtmlNode? nextLink)
        {
            //IEnumerable<HtmlNode>? idxNodes = nextLink?.Descendants().Where(n => n?.Name == "a" && n.Attributes["id"]?.Value == "link-index") as IEnumerable<HtmlNode>;
            IEnumerable<HtmlNode>? nxtNodes = nextLink?.Descendants().Where(n => n?.Name == "a" && n.Attributes["id"]?.Value == "link-next") as IEnumerable<HtmlNode>;
            //Debug.Assert(idxNodes?.Count() == 1);
            Debug.Assert(nxtNodes?.Count() == 1);

            //string? sIdx = idxNodes?.First()?.Attributes["href"]?.Value;
            string? sNxt = nxtNodes?.First()?.Attributes["href"]?.Value;
            return sNxt??"";
        }

        public string GetBookContents(HtmlNode? content)
        {
            StringBuilder sbContent = new StringBuilder();
            IEnumerable<HtmlNode>? contentNodes = content?.Descendants().Where(n => n?.Name == "div" && n.Attributes["id"]?.Value == "nr1") as IEnumerable<HtmlNode>;
            foreach (HtmlNode element in contentNodes?.First().ChildNodes)
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
                        sbContent.Append(strLine).AppendLine();
                    }
                }
                else if (string.Equals("br", element.Name))
                {
                    sbContent.Append("\r\n");
                }
            }
            return sbContent.ToString().Replace("\r\n\r\n", "\r\n");
        }

        public string GetBookContents2(HtmlNode? content)
        {
            StringBuilder sbContent = new StringBuilder();
            //IEnumerable<HtmlNode>? contentNodes = content.Descendants().Where(n => n?.Name == "div" && n.Attributes["id"]?.Value == "nr1") as IEnumerable<HtmlNode>;
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
                        sbContent.Append(strLine).AppendLine();
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
#pragma warning restore CS8601 // Null 参照代入の可能性があります。
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning restore CS8632 // Null 参照代入の可能性があります。
}