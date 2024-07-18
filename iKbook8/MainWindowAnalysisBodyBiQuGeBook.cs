using HtmlAgilityPack;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace iKbook8
{
    public class BiQuGeBookNovelContent : IFetchNovelContent
    {
        public void AnalysisHtmlBookBody(MainWindow wndMain, ref WndContextData datacontext, string strBody, bool bSilenceMode = false, DownloadStatus? status = null)
        {
            Debug.Assert(!bSilenceMode || (bSilenceMode && status != null));
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(strBody);
            //Debug.Assert(html.DocumentNode.ChildNodes.Count == 1 && string.Equals(html.DocumentNode.ChildNodes[0].Name, "body"));
            HtmlNode body = html.DocumentNode.ChildNodes["BODY"];

            HtmlNode section_opt = null;
            HtmlNode content = null;
            HtmlNode header = null;
            FindBookNextLinkAndContents(body, ref section_opt, ref header, ref content);
            if (content != null || section_opt != null)
            {
                string strNextLink = GetBookNextLink(section_opt);
                string strChapterHeader = GetBookHeader(header);
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
                    DownloadStatus.ContentsWriter?.Flush();
                }
                datacontext.NextLinkAnalysized = !string.IsNullOrEmpty(strNextLink);
                wndMain.btnNextPage.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                return;
            }

            //foreach (HtmlNode element in body?.ChildNodes)
            //{
            //    //hrefTags.Add(element.GetAttribute("href"));
            //    if (string.Equals("#text", element.Name) || string.Equals("script", element.Name) || string.Equals("ul", element.Name))
            //    {
            //    }
            //    else if (string.Equals("ins", element.Name) || string.Equals("br", element.Name) || string.Equals("b", element.Name) || string.Equals("#comment", element.Name))
            //    {
            //    }
            //    else if (string.Equals("a", element.Name) || string.Equals("link", element.Name) || string.Equals("meta", element.Name) || string.Equals("title", element.Name))
            //    {
            //    }
            //    else if (string.Equals("div", element.Name))
            //    {
            //    }
            //    else
            //    {
            //        Debug.Assert(false);
            //    }
            //}
        }

        public void FindBookNextLinkAndContents(HtmlNode parent, ref HtmlNode section_opt, ref HtmlNode header, ref HtmlNode content)
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
                        section_opt = element;
                    }
                    else
                    {
                        FindBookNextLinkAndContents(element, ref section_opt, ref header, ref content);
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

        public string GetBookHeader(HtmlNode header)
        {
            return header.InnerText;

        }

        public string GetBookNextLink(HtmlNode section_opt)
        {
            IEnumerable<HtmlNode> ?lstNodes = section_opt.Descendants().Where(n => n?.Name == "td" && n.Attributes["class"]?.Value == "mulu") as IEnumerable<HtmlNode>;
            IEnumerable<HtmlNode> ?nxtNodes = section_opt.Descendants().Where(n => n?.Name == "td" && n.Attributes["class"]?.Value == "next") as IEnumerable<HtmlNode>;
            Debug.Assert(lstNodes.Count()==1 && nxtNodes?.Count() ==1);

            IEnumerable< HtmlNode > aLstNode = lstNodes.First().Descendants().Where(n => n?.Name == "a" && n.Attributes["id"]?.Value == "pb_mulu") as IEnumerable<HtmlNode>;
            IEnumerable<HtmlNode> aNxtNode = nxtNodes.First().Descendants().Where(n => n?.Name == "a" && n.Attributes["id"]?.Value == "pb_next") as IEnumerable<HtmlNode>;

            Debug.Assert(aLstNode.Count() == 1 && aNxtNode?.Count() == 1);
            string sLst = aLstNode.First().Attributes["href"]?.Value;
            string sNxt = aNxtNode.First().Attributes["href"]?.Value;
            return "https://m.xbiqugew.com/book" + sLst.Replace("chapters_","") + sNxt;

        }

        public string GetBookContents(HtmlNode content)
        {
            StringBuilder sbContent = new StringBuilder();
            IEnumerable<HtmlNode>? contentNodes = content.Descendants().Where(n => n?.Name == "div" && n.Attributes["id"]?.Value == "nr1") as IEnumerable<HtmlNode>;
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
    }
}