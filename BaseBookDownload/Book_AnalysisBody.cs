using BaseBookDownload;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System.Text;

namespace BaseBookDownload
{
#pragma warning disable CS8632 // Null 参照代入の可能性があります。
    public class BaseBookNovelContent
    {
        protected string? URL { get; set; } = null;
        public static JToken? GetValueByKeyFromJObject(JObject jObj, string sKey)
        {
            JToken? value;
            if (jObj.TryGetValue(sKey, out value))
            {
                return value;
            }
            return null;
        }
        
        public static HtmlNode? GetHtmlBody(HtmlDocument html)
        {
            HtmlNode? parent = null;
            foreach (HtmlNode child in html.DocumentNode.ChildNodes) { 
                if(child.Name == "html")
                {
                    parent = child;
                    break;
                }
            }
            if (parent != null) {
                return parent.ChildNodes["BODY"];
            }
            //HtmlNode? ret = html.DocumentNode.ChildNodes["BODY"];
            return html.DocumentNode.ChildNodes["BODY"];
        }

        protected void ParseResultToUI(IBaseMainWindow wndMain, bool bSilenceMode, string strContents, string strNextLink)
        {
            //WPFMainWindow wndMain = (WPFMainWindow)IWndMain;
            wndMain.UpdateAnalysizedContents(strContents);
            wndMain.UpdateNextUrl(strNextLink);
            wndMain.UpdateCurUrl(strNextLink);
            if (bSilenceMode)
            {
                wndMain.UpdateAggragatedContentsWithLimit(strContents);
            }
            else
            {
                wndMain.UpdateAggragatedContents(strContents);
            }
        }

        public string CascadeGetTagP_TagBr_TagText(HtmlNode? node)
        {
            if (node == null)
                return "";

            if (node.Name == "#text")
                return node.InnerText.Trim();
            else if (node.Name == "p")
                return node.InnerText.Trim() + "\r\n";
            else if (node.Name == "br")
                return "\r\n";
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach(HtmlNode node2 in node.ChildNodes) { 
                    sb.Append(CascadeGetTagP_TagBr_TagText(node2)); 
                }
                return sb.ToString();
            }
        }

        public StringBuilder CascadeGetTagP_TagBr_TagText(StringBuilder sb, HtmlNode? node)
        {
            if (node == null)
                return sb;

            if (node.Name == "#text")
                return sb.Append(node.InnerText.Trim());
            else if (node.Name == "p")
                return sb.Append(node.InnerText.Trim()).AppendLine();
            else if (node.Name == "br")
                return sb.AppendLine();
            else
            {
                foreach (HtmlNode node2 in node.ChildNodes)
                {
                    sb.Append(CascadeGetTagP_TagBr_TagText(node2));
                }
                return sb;
            }
        }
        public string? ReformContent(string s)
        {
            if (s == null)
                return s;

            return s.Replace("\r", null).Replace("\n", "\r\n").Replace("&nbsp;", " ").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&")
                .Replace("&ensp;", " ").Replace("&emsp;", " ").Replace("&ndash;", " ").Replace("&mdash;", " ")
                .Replace("&sbquo;", "“").Replace("&rdquo;", "”").Replace("&bdquo;", "„")
                .Replace("&quot;", "\"").Replace("&circ;", "ˆ").Replace("&tilde;", "˜").Replace("&prime;", "′").Replace("&Prime;", "″").Replace("\r\n\r\n\r\n", "\r\n").Replace("\r\n\r\n", "\r\n");
        }

        public string? ReformLine(string? s)
        {
            if (s == null)
                return null;

            return s.Replace("\r", null).Replace("\n", "\r\n").Replace("&nbsp;", " ").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&")
                .Replace("&ensp;", " ").Replace("&emsp;", " ").Replace("&ndash;", " ").Replace("&mdash;", " ")
                .Replace("&sbquo;", "“").Replace("&rdquo;", "”").Replace("&bdquo;", "„")
                .Replace("&quot;", "\"").Replace("&circ;", "ˆ").Replace("&tilde;", "˜").Replace("&prime;", "′").Replace("&Prime;", "″").Replace("\r\n\r\n\r\n", "\r\n").Replace("\r\n\r\n", "\r\n");
        }

        public string? ReformContent(StringBuilder ? sb)
        {
            if (sb == null)
                return null;

            return sb.Replace("\r", null).Replace("\n", "\r\n").Replace("&nbsp;", " ").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&")
                .Replace("&ensp;", " ").Replace("&emsp;", " ").Replace("&ndash;", " ").Replace("&mdash;", " ")
                .Replace("&sbquo;", "“").Replace("&rdquo;", "”").Replace("&bdquo;", "„")
                .Replace("&quot;", "\"").Replace("&circ;", "ˆ").Replace("&tilde;", "˜").Replace("&prime;", "′").Replace("&Prime;", "″").Replace("\r\n\r\n\r\n", "\r\n").Replace("\r\n\r\n", "\r\n").ToString();
        }
    }
#pragma warning restore CS8632 // Null 参照代入の可能性があります。
}