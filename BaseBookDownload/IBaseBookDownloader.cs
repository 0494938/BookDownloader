using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace BaseBookDownloader
{
#pragma warning disable CS8632 // Null 参照代入の可能性があります。
    public interface IFetchNovelContent
    {
        public bool AnalysisHtmlStream(IBaseMainWindow wndMain, BaseWndContextData datacontext, string strURL, string strBody, bool bSilenceMode = false, DownloadStatus? status = null, int nMaxRetry = 0);
        public bool AnalysisHtmlBook(IBaseMainWindow wndMain, BaseWndContextData datacontext, string strURL, string strBody, bool bSilenceMode = false, DownloadStatus? status = null, int nMaxRetry = 0);
        public void FindBookNextLinkAndContents(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? parent, ref HtmlNode? nextLink, ref HtmlNode? header, ref HtmlNode? content, ref HtmlNode novelName);
        public string GetBookHeader(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? header);
        public string GetBookNextLink(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? nextLink);
        public string GetBookContents(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? content);
        public string GetBookName(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? content);
    }

    public static class PrettyPrintUtil
    {
        public static string PrettyPrintXml(this string serialisedInput)
        {
            if (string.IsNullOrEmpty(serialisedInput))
            {
                return serialisedInput;
            }

            try
            {
                return XDocument.Parse(serialisedInput).ToString();
            }
            catch (Exception) { }


            return serialisedInput;
        }
        private static bool IsTagIsOneLineTag(HtmlNode node)
        {
            if (!node.HasChildNodes)
                return true;

            if (node.HasChildNodes && node.ChildNodes.Count > 1)
                return false;
            if (node.HasChildNodes)
                return IsTagIsOneLineTag(node.ChildNodes[0]);
            else
                return true;
        }

        private static bool isEmptyHtmlText(string strText)
        {
            if (strText.Replace(" ", "").Replace("\r", "").Replace("\t", "").Replace("\n", "") == "")
                return true;
            return false;
        }

        private static StringBuilder PrettyPrintHtmlTagInOneLine(StringBuilder sb, HtmlNode node, int nLevel, bool bIgnoreScript, bool bIgnoreStyle, bool bIgnoreHead) {
            if (node.Name == "head" && bIgnoreHead)
            {
            }
            else if (node.Name == "script")
            {
                if (!bIgnoreScript)
                {
                    sb.Append(node.OuterHtml.Trim().Replace("\r", "").Replace("\n", ""));
                }
            }
            else if (node.Name == "style")
            {
                if (!bIgnoreStyle)
                {
                    sb.Append(node.OuterHtml.Trim().Replace("\r", "").Replace("\n", ""));
                }
            }
            //else if (node.HasChildNodes && node.ChildNodes.Count == 1 && node.ChildNodes[0].Name == "#text")
            //{
            //    sb.Append("<" + node.Name);
            //    foreach (var attr in node.Attributes)
            //    {
            //        sb.Append(" " + attr.Name + "=\"" + attr.Value + "\"");
            //    }
            //    sb.Append(">");
            //    if (!isEmptyHtmlText(node.InnerText))
            //        sb.Append(node.ChildNodes[0].InnerText);
            //    sb.Append("</" + node.Name + ">");
            //}
            else if (node.Name == "#comment")
            {
                sb.Append(node.OuterHtml.Trim());
            }
            else if (node.Name == "#text")
            {
                if (!isEmptyHtmlText(node.InnerText)) sb.Append(node.InnerText.Trim().Replace("\r\n\r\n\r\n", "\r\n").Replace("\r\n\r\n", "\r\n"));
            }
            else if (node.Name == "br")
                sb.Append(node.OuterHtml.Trim());
            else if (node.Name == "p")
                sb.Append("<p>" + node.InnerHtml + "</p>");
            else if (!node.HasChildNodes)
                sb.Append(node.OuterHtml);
            else
            {
                sb.Append("<" + node.Name);
                foreach (var attr in node.Attributes)
                {
                    sb.Append(" " + attr.Name + "=\"" + attr.Value + "\"");
                }
                sb.Append(">");
                foreach (var child in node.ChildNodes)
                {
                    PrettyPrintHtmlTagInOneLine(sb, child, nLevel + 1, bIgnoreScript, bIgnoreStyle,  bIgnoreHead);
                }
                sb.Append("</" + node.Name + ">");
            }
            return sb;
        }

        private static StringBuilder PrettyPrintHtmlTag(StringBuilder sb, HtmlNode node, int nLevel, bool bIgnoreScript, bool bIgnoreHead, bool bIgnoreStyle, bool bOneLineTextCascadeInOneLine=false) {
            if (node.Name == "li" && node.InnerText == "散文诗词")
            {
                Debug.Assert(true);
            }
            if (node.Name == "head")
            {
                Debug.Assert(true);
            }
            if (node.Name == "div" && node.Attributes["class"]?.Value == "p")
            {
                Debug.Assert(true);
            }

            if (bOneLineTextCascadeInOneLine && IsTagIsOneLineTag(node)) {
                StringBuilder sbLine = new StringBuilder();
                PrettyPrintHtmlTagInOneLine(sbLine, node, nLevel, bIgnoreScript, bIgnoreStyle, bIgnoreHead);
                if(!isEmptyHtmlText(sbLine.ToString())) sb.Append("".PadLeft(nLevel * 4)).Append(sbLine).AppendLine();
            }
            else
            {
                if (node.Name == "head" && bIgnoreHead)
                {
                }
                else if (node.Name == "script")
                {
                    if (!bIgnoreScript)
                    {
                        sb.Append("".PadLeft(nLevel * 4) + "<" + node.Name);
                        foreach (var attr in node.Attributes)
                        {
                            sb.Append(" " + attr.Name + "=\"" + attr.Value + "\"");
                        }
                        sb.AppendLine(">");
                        if (!isEmptyHtmlText(node.InnerText)) sb.AppendLine("".PadLeft(nLevel * 4) + node.InnerText.Trim());
                        sb.AppendLine("".PadLeft(nLevel * 4) + "</" + node.Name + ">");
                    }
                }
                else if (node.Name == "style")
                {
                    if (!bIgnoreStyle)
                    {
                        sb.Append("".PadLeft(nLevel * 4) + "<" + node.Name);
                        foreach (var attr in node.Attributes)
                        {
                            sb.Append(" " + attr.Name + "=\"" + attr.Value + "\"");
                        }
                        sb.AppendLine(">");
                        if (!isEmptyHtmlText(node.InnerText)) sb.AppendLine("".PadLeft(nLevel * 4) + node.InnerText.Trim());
                        sb.AppendLine("".PadLeft(nLevel * 4) + "</" + node.Name + ">");
                    }
                }
                //else if (node.HasChildNodes && node.ChildNodes.Count == 1 && node.ChildNodes[0].Name == "#text"){
                //    sb.Append("".PadLeft(nLevel * 4) + "<" + node.Name);
                //    foreach (var attr in node.Attributes)
                //    {
                //        sb.Append(" " + attr.Name + "=\"" + attr.Value + "\"");
                //    }
                //    sb.Append(">");
                //    if (!isEmptyHtmlText(node.InnerText))
                //        sb.Append(node.ChildNodes[0].InnerText.Replace("\r\n\r\n\r\n", "\r\n").Replace("\r\n\r\n", "\r\n"));
                //    sb.AppendLine("</" + node.Name + ">");
                //}
                else if (node.Name == "#comment")
                {
                    sb.AppendLine("".PadLeft(nLevel * 4) + node.OuterHtml.Trim());
                }
                else if (node.Name == "#text")
                {
                    if (!isEmptyHtmlText(node.InnerText)) sb.AppendLine("".PadLeft(nLevel * 4) + node.InnerText.Trim());
                }
                else if (node.Name == "br")
                    sb.AppendLine("".PadLeft(nLevel * 4) + node.OuterHtml.Trim());
                else if (node.Name == "p")
                    sb.AppendLine("".PadLeft(nLevel * 4) + "<p>" + node.InnerHtml + "</p>");
                else if (!node.HasChildNodes)
                    sb.AppendLine("".PadLeft(nLevel * 4) + node.OuterHtml);
                else
                {
                    sb.Append("".PadLeft(nLevel * 4) + "<" + node.Name);
                    foreach (var attr in node.Attributes)
                    {
                        sb.Append(" " + attr.Name + "=\"" + attr.Value + "\"");
                    }
                    sb.AppendLine(">");
                    foreach (var child in node.ChildNodes)
                    {
                        PrettyPrintHtmlTag(sb, child, nLevel + 1, bIgnoreScript, bIgnoreHead, bIgnoreStyle, bOneLineTextCascadeInOneLine);
                    }
                    sb.AppendLine("".PadLeft(nLevel * 4) + "</" + node.Name + ">");
                }
            }
            return sb;
        }

        public static string PrettyPrintHtml(this string serialisedInput, bool bIgnoreScript, bool bIgnoreHead, bool bIgnoreStyle=false)
        {
            if (string.IsNullOrEmpty(serialisedInput))
            {
                return serialisedInput;
            }

            try
            {
                HtmlDocument html = new HtmlDocument();
                html.LoadHtml(serialisedInput);

                StringBuilder sb = new StringBuilder();
                foreach (HtmlNode child in html.DocumentNode.ChildNodes)
                {
                    PrettyPrintHtmlTag(sb, child, 0, bIgnoreScript, bIgnoreHead, true);
                }

                return sb.ToString();
            }
            catch (Exception) { }


            return serialisedInput;
        }

        public static string PrettyPrintJson(this string serialisedInput)
        {
            if (string.IsNullOrEmpty(serialisedInput))
            {
                return serialisedInput;
            }

            try
            {
                var t = JsonConvert.DeserializeObject<object>(serialisedInput);
                return JsonConvert.SerializeObject(t, Formatting.Indented);
            }
            catch (Exception) { }

            return serialisedInput;
        }

    }

#pragma warning restore CS8632 // Null 参照代入の可能性があります。
}
