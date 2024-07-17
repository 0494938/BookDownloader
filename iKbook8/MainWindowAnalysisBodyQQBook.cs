using HtmlAgilityPack;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace iKbook8
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private void AnalysisHtmlQQBookBody(ref WndContextData datacontext, string strBody, bool bSilenceMode = false, DownloadStatus? status = null)
        {
            Debug.Assert(!bSilenceMode || (bSilenceMode && status != null));
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(strBody);
            Debug.Assert(html.DocumentNode.ChildNodes.Count == 1 && string.Equals(html.DocumentNode.ChildNodes[0].Name, "body"));
            HtmlNode body = html.DocumentNode.ChildNodes[0];

            foreach (HtmlNode element in body?.ChildNodes)
            {
                //hrefTags.Add(element.GetAttribute("href"));
                if (string.Equals("#text", element.Name))
                {
                }
                else if (string.Equals("script", element.Name))
                {
                }
                else if (string.Equals("ul", element.Name))
                {
                }
                else if (string.Equals("ins", element.Name))
                {
                }
                else if (string.Equals("br", element.Name))
                {
                }
                else if (string.Equals("b", element.Name))
                {
                }
                else if (string.Equals("a", element.Name))
                {
                }
                else if (string.Equals("link", element.Name))
                {
                }
                else if (string.Equals("meta", element.Name))
                {
                }
                else if (string.Equals("title", element.Name))
                {
                }
                else if (string.Equals("div", element.Name))
                {
                    if (string.Equals(element.Attributes["id"]?.Value, "__nuxt"))
                    {
                        HtmlNode section_opt = null;
                        HtmlNode content = null;
                        HtmlNode header = null;
                        FindQQBookNextLinkAndContents(element, ref section_opt, ref header, ref content);
                        if (content != null || section_opt != null)
                        {
                            string strNextLink = GetQQBookNextLink(section_opt);
                            string strChapterHeader = GetQQBookHeader(header);
                            string strContents = GetQQBookContents(content);

                            txtAnalysizedContents.Text = strContents;
                            txtNextUrl.Text = strNextLink;
                            txtCurURL.Text = strNextLink;
                            if (bSilenceMode) {
                                Debug.Assert(status!=null);
                                status.NextUrl = strNextLink;
                                if(txtAggregatedContents.Text.Length > 1024 * 64)
                                    txtAggregatedContents.Text = (" \r\n " + strChapterHeader + " \r\n" + strContents);
                                else
                                    txtAggregatedContents.Text += (" \r\n" + strChapterHeader + " \r\n" + strContents);
                                txtAggregatedContents.ScrollToEnd();

                                DownloadStatus.ContentsWriter?.Write(" \r\n" + strChapterHeader + " \r\n" + strContents);
                                DownloadStatus.ContentsWriter?.Flush();
                            }
                            datacontext.NextLinkAnalysized = !string.IsNullOrEmpty(strNextLink);
                            btnNextPage.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
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
        void FindQQBookNextLinkAndContents(HtmlNode parent, ref HtmlNode section_opt, ref HtmlNode header, ref HtmlNode content) {
            foreach (HtmlNode element in parent?.ChildNodes)
            {
                //hrefTags.Add(element.GetAttribute("href"));
                if (string.Equals("div", element.Name))
                {
                    if (string.Equals(element.Attributes["class"]?.Value, "chapter-content isTxt"))
                    {
                        content = element;
                    }
                    else if (string.Equals(element.Attributes["class"]?.Value, "footer") && section_opt == null)
                    {
                        section_opt = element;
                    }
                    else if (string.Equals(element.Attributes["class"]?.Value, "read-header") && header == null)
                    {
                        header = element;
                    }
                    else
                    {
                        FindQQBookNextLinkAndContents(element, ref section_opt, ref header, ref content);
                    }
                }
            }

        }

        
        string GetQQBookHeader(HtmlNode header)
        {
            foreach (HtmlNode element in header?.ChildNodes)
            {
                if (string.Equals("h1", element.Name))
                {
                    if (string.Equals(element.Attributes["class"]?.Value, "chapter-title") )
                    {
                        return element.InnerText;
                    }
                }
            }
            return "";
        }

        string GetQQBookNextLink(HtmlNode section_opt)
        {
            foreach (HtmlNode element in section_opt?.ChildNodes)
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

        string  GetQQBookContents(HtmlNode content){
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
                    if(!string.IsNullOrEmpty(strLine?.Trim()) && !string.Equals(strLine?.Trim(), "\\/阅|读|模|式|内|容|加|载|不|完|整|，退出可阅读完整内容|点|击|屏|幕|中|间可|退|出|阅-读|模|式|."))
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
    }
}