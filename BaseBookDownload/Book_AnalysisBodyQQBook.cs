using BaseBookDownload;
using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.Text;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace BaseBookDownload
{
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning disable CS8601 // Null 参照代入の可能性があります。
#pragma warning disable CS8632 // Null 参照代入の可能性があります。
    public class OOBookNovelContent : BaseBookNovelContent, IFetchNovelContent
    {
        public void AnalysisHtmlBookBody(IBaseMainWindow wndMain, BaseWndContextData datacontext, string strUrl, string strBody, bool bSilenceMode = false, DownloadStatus? status = null, int nMaxRetry = 0)
        {
            this.URL = strUrl;

            Debug.Assert(!bSilenceMode || (bSilenceMode && status != null));
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(strBody);
            HtmlNode? body = html.DocumentNode.ChildNodes["BODY"];

            if (body == null)
            {
                wndMain.UpdateStatusMsg(datacontext, "*** URL downloaded BODY is empty, skip this Page *** ", 0);
                return;
            }
            else
            {
                foreach (HtmlNode element in body?.ChildNodes)
                {
                    //hrefTags.Add(element.GetAttribute("href"));
                    if (string.Equals("#text", element.Name) || string.Equals("script", element.Name) || string.Equals("ul", element.Name))
                    {
                    }
                    else if (string.Equals("ins", element.Name) || string.Equals("br", element.Name) || string.Equals("b", element.Name) || string.Equals("#comment", element.Name))
                    {
                    }
                    else if (string.Equals("a", element.Name) || string.Equals("link", element.Name) || string.Equals("meta", element.Name) || string.Equals("title", element.Name))
                    {
                    }
                    else if (string.Equals("div", element.Name))
                    {
                        if (string.Equals(element.Attributes["id"]?.Value, "__nuxt"))
                        {
                            HtmlNode? nextLink = null;
                            HtmlNode? content = null;
                            HtmlNode? header = null;
                            FindBookNextLinkAndContents(element, ref nextLink, ref header, ref content);
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
                    else
                    {
                        Debug.Assert(false);
                    }
                }
            }
        }

        public void FindBookNextLinkAndContents(HtmlNode? parent, ref HtmlNode nextLink, ref HtmlNode header, ref HtmlNode content)
        {
            if(parent != null)
            {
                foreach (HtmlNode element in parent?.ChildNodes)
                {
                    //hrefTags.Add(element.GetAttribute("href"));
                    if (string.Equals("div", element.Name))
                    {
                        if (string.Equals(element.Attributes["class"]?.Value, "chapter-content isTxt"))
                        {
                            content = element;
                        }
                        else if (string.Equals(element.Attributes["class"]?.Value, "footer") && nextLink == null)
                        {
                            nextLink = element;
                        }
                        else if (string.Equals(element.Attributes["class"]?.Value, "read-header") && header == null)
                        {
                            header = element;
                        }
                        else
                        {
                            FindBookNextLinkAndContents(element, ref nextLink, ref header, ref content);
                        }
                    }
                }
            }
        }

        public string GetBookHeader(HtmlNode? header)
        {
            if (header != null)
            {
                foreach (HtmlNode element in header?.ChildNodes)
                {
                    if (string.Equals("h1", element.Name))
                    {
                        if (string.Equals(element.Attributes["class"]?.Value, "chapter-title"))
                        {
                            return element.InnerText;
                        }
                    }
                }
            }
            return "";
        }

        public string GetBookNextLink(HtmlNode? nextLink)
        {
            if(nextLink != null)
            {
                foreach (HtmlNode element in nextLink?.ChildNodes)
                {
                    //hrefTags.Add(element.GetAttribute("href"));
                    if (string.Equals("a", element.Name))
                    {
                        if (string.Equals(element.InnerText, "下一页") || string.Equals(element.InnerText, "下一章"))
                        {
                            return "https:" + element.Attributes["href"]?.Value;
                        }
                    }
                }
            }
            
            return "";
        }

        public string GetBookContents(HtmlNode? content)
        {
            if (content != null)
            {
                StringBuilder sbContent = new StringBuilder();
                //strContents = "";
                foreach (HtmlNode element in content?.ChildNodes)
                {
                    //hrefTags.Add(element.GetAttribute("href"));
                    if (string.Equals("p", element.Name))
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
                return sbContent.ToString().Replace("\r\n\r\n\r\n", "\r\n").Replace("\r\n\r\n", "\r\n");
            }
            return "";
        }

        public string GetBookName(HtmlNode? content)
        {
            throw new NotImplementedException();
        }
    }
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning restore CS8601 // Null 参照代入の可能性があります。
#pragma warning restore CS8632 // Null 参照代入の可能性があります。
}