using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace iKbook8
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private void AnalysisHtmlIkBookBody(ref WndContextData datacontext, string strBody)
        {
            /*
            var doc = new HTMLDocument();
            doc.load(htmlContent);
            var nodes = doc.DocumentNode.SelectNodes("//ul[@class='list']/li");
            var result = new List<string>();
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    result.Add(node.InnerText);
                }
            }
            */

            /*
            HtmlDocument htmlSnippet = new HtmlDocument();
            htmlSnippet.LoadHtml(Html);

            List<string> hrefTags = new List<string>();

            HtmlParser parser = new HtmlParser();
            var doc = parser.Parse(strBody) as IHTMLDocument;
            //IHTMLDocument2? docbody = doc.;
            if (doc != null)
            {
                doc.doc;
            }
            */

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
                else if (string.Equals("div", element.Name))
                {
                    if (string.Equals(element.Attributes["class"]?.Value, "container"))
                    {
                        HtmlNode section_opt = null;
                        HtmlNode content = null;
                        FindNextLinkAndContents(element, ref section_opt, ref content);
                        if (content != null || section_opt != null)
                        {
                            string strNextLink = GetIkBookNextLink(section_opt);
                            string strContents = GetIkBookContents(content);

                            txtAnalysizedContents.Text = strContents;
                            txtNextUrl.Text = strNextLink;
                            txtCurURL.Text = strContents;


                        }
                    }
                }
                else
                {
                    Debug.Assert(false);
                }
            }

            /*
            // yields: [<p class="content">Fizzler</p>]
            document.QuerySelectorAll(".content");

            // yields: [<p class="content">Fizzler</p>,<p>CSS Selector Engine</p>]
            document.QuerySelectorAll("p");

            // yields empty sequence
            document.QuerySelectorAll("body>p");

            // yields [<p class="content">Fizzler</p>,<p>CSS Selector Engine</p>]
            document.QuerySelectorAll("body p");

            // yields [<p class="content">Fizzler</p>]
            document.QuerySelectorAll("p:first-child");
            */

            /*
            foreach (IElement element in document?.body)
            {
                hrefTags.Add(element.GetAttribute("href"));
            }
            */
        }
        void FindNextLinkAndContents(HtmlNode parent, ref HtmlNode section_opt, ref HtmlNode content) {
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
                        FindNextLinkAndContents(element, ref section_opt, ref content);
                    }
                }
            }

        }

        string GetIkBookNextLink(HtmlNode section_opt)
        {
            foreach (HtmlNode element in section_opt?.ChildNodes)
            {
                //hrefTags.Add(element.GetAttribute("href"));
                if (string.Equals("a", element.Name))
                {
                    if (string.Equals(element.InnerHtml, "下一页"))
                    {
                        return element.Attributes["href"]?.Value;
                    }
                }
            }
            return "";
        }

        string  GetIkBookContents(HtmlNode content){
            StringBuilder sbContent = new StringBuilder();
            //strContents = "";
            foreach (HtmlNode element in content?.ChildNodes)
            {
                //hrefTags.Add(element.GetAttribute("href"));
                if (string.Equals("#text", element.Name))
                {
                    sbContent.Append(element.InnerText?.Replace("\r","")?.Replace("\n","")?.Replace("&nbsp;"," ")?.Replace("&lt;", "<")?.Replace("&gt;", ">")?.Replace("&amp;", "&")?
                        .Replace("&ensp;", " ")?.Replace("&emsp;", " ")?.Replace("&ndash;", " ")?.Replace("&mdash;", " ")?
                        .Replace("&sbquo;", "“")?.Replace("&rdquo;", "”")?.Replace("&bdquo;", "„"))?
                        .Replace("&quot;", "\"")?.Replace("&circ;", "ˆ")?.Replace("&tilde;", "˜")?.Replace("&prime;", "′")?.Replace("&Prime;", "″")
                        ;
                }
                else if (string.Equals("br", element.Name))
                {
                    sbContent.Append("\r\n");
                }
            }
            return sbContent.ToString();
        }
    }
}