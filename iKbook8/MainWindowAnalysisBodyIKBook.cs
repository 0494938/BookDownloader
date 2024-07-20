using HtmlAgilityPack;
using System.Diagnostics;
using System.Text;
using System.Windows.Controls;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace BookDownloader
{
#pragma warning disable CS8601 // Null 参照代入の可能性があります。
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
    public class IKBook8NovelContent : BaseBookNovelContent, IFetchNovelContent
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
                //.Print("URL downloaded BODY is empty ...");
                wndMain.UpdateStatusMsg(datacontext, "*** URL downloaded BODY is empty, skip this Page *** ", 0);
                return;
            }

            foreach (HtmlNode element in body?.ChildNodes)
            {
                //hrefTags.Add(element.GetAttribute("href"));
                if (string.Equals("#text", element.Name))
                {
                }
                else if (string.Equals("script", element.Name) || string.Equals("ul", element.Name) || string.Equals("ins", element.Name))
                {
                }
                else if (string.Equals("br", element.Name) || string.Equals("b", element.Name) || string.Equals("a", element.Name))
                {
                }
                else if (string.Equals("link", element.Name) || string.Equals("meta", element.Name) || string.Equals("title", element.Name))
                {
                }
                else if (string.Equals("#comment", element.Name) || string.Equals("xxxx", element.Name) || string.Equals("xxxx", element.Name))
                {
                }
                else if (string.Equals("div", element.Name))
                {
                    if (string.Equals(element.Attributes["class"]?.Value, "container"))
                    {
                        HtmlNode? nextLink = null;
                        HtmlNode? header = null;
                        HtmlNode? content = null;
                        FindBookNextLinkAndContents(element, ref nextLink, ref header, ref content);
                        if (content != null || nextLink != null)
                        {
                            string strNextLink = GetBookNextLink(nextLink);
                            string strChapterHeader = GetBookHeader(header);
                            //string strContents = GetBookContents(content);
                            string strContents = " \r\n \r\n " + strChapterHeader + " \r\n" + GetBookContents(content);

                            wndMain.GetDispatcher().Invoke(() =>
                            {
                                ParseResultToUI(wndMain, bSilenceMode, strContents, strNextLink);
                            });

                            if (bSilenceMode)
                            {
                                Debug.Assert(status != null);
                                status.NextUrl = strNextLink;

                                DownloadStatus.ContentsWriter?.Write(strContents);
                            }
                            datacontext.NextLinkAnalysized = !string.IsNullOrEmpty(strNextLink);
                            wndMain.GetDispatcher().Invoke(() =>
                            {
                                wndMain.UpdateNextPageButton();
                            });

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

        public void FindBookNextLinkAndContents(HtmlNode? parent, ref HtmlNode nextLink, ref HtmlNode header, ref HtmlNode content)
        {
            foreach (HtmlNode element in parent?.ChildNodes)
            {
                //hrefTags.Add(element.GetAttribute("href"));
                if (string.Equals("div", element.Name))
                {
                    if (string.Equals(element.Attributes["class"]?.Value, "content"))
                    {
                        content = element;
                    }
                    else if (string.Equals(element.Attributes["class"]?.Value, "section-opt") && nextLink == null)
                    {
                        nextLink = element;
                    }
                    else
                    {
                        FindBookNextLinkAndContents(element, ref nextLink, ref header, ref content);
                    }
                }else if (string.Equals("h1", element.Name))
                {
                    if (string.Equals(element.Attributes["class"]?.Value, "title"))
                    {
                        header = element;
                    }
                }
            }
        }

        public string GetBookContents(HtmlNode? content)
        {
            StringBuilder sbContent = new StringBuilder();
            //strContents = "";
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

        public string GetBookHeader(HtmlNode? header)
        {
            return header?.InnerText??"";
        }

        public string GetBookName(HtmlNode? content)
        {
            throw new NotImplementedException();
        }

        public string GetBookNextLink(HtmlNode? nextLink)
        {
            foreach (HtmlNode element in nextLink?.ChildNodes)
            {
                //hrefTags.Add(element.GetAttribute("href"));
                if (string.Equals("a", element.Name))
                {
                    if (string.Equals(element.InnerHtml, "下一页") || string.Equals(element.InnerHtml, "下一章"))
                    {
                        return element?.Attributes["href"]?.Value??"";
                    }
                }
            }
            return "";
        }
    }
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning restore CS8601 // Null 参照代入の可能性があります。
}