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
        //private void AnalysisHtmlIkBookBody(ref WndContextData datacontext, string strBody, bool bSilenceMode = false, DownloadStatus? status = null)
        //{

        //}
        //void FindIkBookNextLinkAndContents(HtmlNode parent, ref HtmlNode section_opt, ref HtmlNode content) {
        //    foreach (HtmlNode element in parent?.ChildNodes)
        //    {
        //        //hrefTags.Add(element.GetAttribute("href"));
        //        if (string.Equals("div", element.Name))
        //        {
        //            if (string.Equals(element.Attributes["class"]?.Value, "content"))
        //            {
        //                content = element;
        //            }

        //            else if (string.Equals(element.Attributes["class"]?.Value, "section-opt") && section_opt == null)
        //            {
        //                section_opt = element;
        //            }
        //            else
        //            {
        //                FindIkBookNextLinkAndContents(element, ref section_opt, ref content);
        //            }
        //        }
        //    }

        //}

        //string GetIkBookNextLink(HtmlNode section_opt)
        //{
        //    foreach (HtmlNode element in section_opt?.ChildNodes)
        //    {
        //        //hrefTags.Add(element.GetAttribute("href"));
        //        if (string.Equals("a", element.Name))
        //        {
        //            if (string.Equals(element.InnerHtml, "下一页") || string.Equals(element.InnerHtml, "下一章"))
        //            {
        //                return element.Attributes["href"]?.Value;
        //            }
        //        }
        //    }
        //    return "";
        //}

        //string  GetIkBookContents(HtmlNode content){
        //    StringBuilder sbContent = new StringBuilder();
        //    //strContents = "";
        //    foreach (HtmlNode element in content?.ChildNodes)
        //    {
        //        //hrefTags.Add(element.GetAttribute("href"));
        //        if (string.Equals("#text", element.Name))
        //        {
        //            string? strLine = element.InnerText?.Replace("\r", "")?.Replace("\n", "")?.Replace("&nbsp;", " ")?.Replace("&lt;", "<")?.Replace("&gt;", ">")?.Replace("&amp;", "&")?
        //                .Replace("&ensp;", " ")?.Replace("&emsp;", " ")?.Replace("&ndash;", " ")?.Replace("&mdash;", " ")?
        //                .Replace("&sbquo;", "“")?.Replace("&rdquo;", "”")?.Replace("&bdquo;", "„")?
        //                .Replace("&quot;", "\"")?.Replace("&circ;", "ˆ")?.Replace("&tilde;", "˜")?.Replace("&prime;", "′")?.Replace("&Prime;", "″");
        //            if(!string.IsNullOrEmpty(strLine?.Trim()) && !string.Equals(strLine?.Trim(), "\\/阅|读|模|式|内|容|加|载|不|完|整|，退出可阅读完整内容|点|击|屏|幕|中|间可|退|出|阅-读|模|式|."))
        //            {
        //                sbContent.Append(strLine);
        //            }
        //        }
        //        else if (string.Equals("br", element.Name))
        //        {
        //            sbContent.Append("\r\n");
        //        }
        //    }
        //    return sbContent.ToString().Replace("\r\n\r\n", "\r\n");
        //}
    }
    public class IKBook8NovelContent : IFetchNovelContent
    {
        public void AnalysisHtmlBookBody(MainWindow wndMain, ref WndContextData datacontext, string strBody, bool bSilenceMode = false, DownloadStatus? status = null)
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
                        HtmlNode section_opt = null;
                        HtmlNode header = null;
                        HtmlNode content = null;
                        FindBookNextLinkAndContents(element, ref section_opt, ref header, ref content);
                        if (content != null || section_opt != null)
                        {
                            string strNextLink = GetBookNextLink(section_opt);
                            string strChapterHeader = GetBookHeader(header);
                            //string strContents = GetBookContents(content);
                            string strContents = " \r\n \r\n " + strChapterHeader + " \r\n" + GetBookContents(content);

                            wndMain.txtAnalysizedContents.Text = strContents;
                            wndMain.txtNextUrl.Text = strNextLink;
                            wndMain.txtCurURL.Text = strNextLink;
                            if (wndMain.txtAggregatedContents.Text.Length > 1024 * 64)
                                wndMain.txtAggregatedContents.Text = strContents;
                            else
                                wndMain.txtAggregatedContents.Text += strContents;
                            wndMain.txtAggregatedContents.ScrollToEnd();
                            if (bSilenceMode)
                            {
                                Debug.Assert(status != null);
                                status.NextUrl = strNextLink;

                                DownloadStatus.ContentsWriter?.Write(strContents);
                            }
                            datacontext.NextLinkAnalysized = !string.IsNullOrEmpty(strNextLink);
                            wndMain.btnNextPage.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
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

        public void FindBookNextLinkAndContents(HtmlNode parent, ref HtmlNode section_opt, ref HtmlNode header, ref HtmlNode content)
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
                    else if (string.Equals(element.Attributes["class"]?.Value, "section-opt") && section_opt == null)
                    {
                        section_opt = element;
                    }
                    else
                    {
                        FindBookNextLinkAndContents(element, ref section_opt, ref header, ref content);
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

        public string GetBookContents(HtmlNode content)
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

        public string GetBookHeader(HtmlNode header)
        {
            return header.InnerText;
        }

        public string GetBookNextLink(HtmlNode section_opt)
        {
            foreach (HtmlNode element in section_opt?.ChildNodes)
            {
                //hrefTags.Add(element.GetAttribute("href"));
                if (string.Equals("a", element.Name))
                {
                    if (string.Equals(element.InnerHtml, "下一页") || string.Equals(element.InnerHtml, "下一章"))
                    {
                        return element.Attributes["href"]?.Value;
                    }
                }
            }
            return "";
        }
    }
}