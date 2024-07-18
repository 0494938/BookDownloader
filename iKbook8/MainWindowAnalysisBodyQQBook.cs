using HtmlAgilityPack;
using System.Diagnostics;
using System.Text;
using System.Windows.Controls;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace BookDownloader
{
    public class OOBookNovelContent : IFetchNovelContent
    {
        public void AnalysisHtmlBookBody(MainWindow wndMain, WndContextData datacontext, string strBody, bool bSilenceMode = false, DownloadStatus? status = null)
        {
            Debug.Assert(!bSilenceMode || (bSilenceMode && status != null));
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(strBody);
            Debug.Assert(html.DocumentNode.ChildNodes.Count == 1 && string.Equals(html.DocumentNode.ChildNodes[0].Name, "body"));
            HtmlNode body = html.DocumentNode.ChildNodes[0];

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
                        HtmlNode nextLink = null;
                        HtmlNode content = null;
                        HtmlNode header = null;
                        FindBookNextLinkAndContents(element, ref nextLink, ref header, ref content);
                        if (content != null || nextLink != null)
                        {
                            string strNextLink = GetBookNextLink(nextLink);
                            string strChapterHeader = GetBookHeader(header);
                            string strContents = " \r\n \r\n " + strChapterHeader + " \r\n" + GetBookContents(content);

                            wndMain.Dispatcher.Invoke(() =>
                            {
                                wndMain.txtAnalysizedContents.Text = strContents;
                                wndMain.txtNextUrl.Text = strNextLink;
                                wndMain.txtCurURL.Text = strNextLink;


                                if (wndMain.txtAggregatedContents.Text.Length > 1024 * 64)
                                    wndMain.txtAggregatedContents.Text = strContents;
                                else
                                    wndMain.txtAggregatedContents.Text += strContents;
                                wndMain.txtAggregatedContents.ScrollToEnd();
                            });
                            if (bSilenceMode)
                            {
                                Debug.Assert(status != null);
                                status.NextUrl = strNextLink;

                                DownloadStatus.ContentsWriter?.Write(strContents);
                                DownloadStatus.ContentsWriter?.Flush();
                            }
                            datacontext.NextLinkAnalysized = !string.IsNullOrEmpty(strNextLink);
                            wndMain.Dispatcher.Invoke(() =>
                            {
                                wndMain.btnNextPage.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
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

        public void FindBookNextLinkAndContents(HtmlNode parent, ref HtmlNode nextLink, ref HtmlNode header, ref HtmlNode content)
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

        public string GetBookHeader(HtmlNode header)
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
            return "";
        }

        public string GetBookNextLink(HtmlNode nextLink)
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
            return "";
        }

        public string GetBookContents(HtmlNode content)
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
            return sbContent.ToString().Replace("\r\n\r\n", "\r\n");
        }

        public string GetBookName(HtmlNode content)
        {
            throw new NotImplementedException();
        }
    }
}