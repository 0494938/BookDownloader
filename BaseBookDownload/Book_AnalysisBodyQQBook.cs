using BaseBookDownload;
using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Xml.Linq;
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
            //HtmlNode body = html.DocumentNode.ChildNodes["BODY"];
            HtmlNode? body = GetHtmlBody(html);

            if (body == null)
            {
                wndMain.UpdateStatusMsg(datacontext, "*** URL downloaded BODY is empty, skip this Page *** ", 0);
                return;
            }

            HtmlNode? nextLink = null;
            HtmlNode? content = null;
            HtmlNode? header = null;
            FindBookNextLinkAndContents(wndMain, datacontext, body, ref nextLink, ref header, ref content);
            if (content != null || nextLink != null)
            {
                string strNextLink = GetBookNextLink(wndMain, datacontext, nextLink);
                string strChapterHeader = GetBookHeader(wndMain, datacontext, header);
                string strContents = " \r\n \r\n " + strChapterHeader + " \r\n" + GetBookContents(wndMain, datacontext, content);

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

        public void FindBookNextLinkAndContents(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? parent, ref HtmlNode? nextLink, ref HtmlNode? header, ref HtmlNode? content)
        {
            if(parent != null)
            {

                HtmlNodeCollection? collCont = parent?.SelectNodes(".//div[@id='Lab_Contents'][@class='kong']");
                content = (collCont?.Count ?? 0) > 0 ? (collCont[0]) : null;

                HtmlNodeCollection? collHeader = parent?.SelectNodes(".//h1[@id='ChapterTitle']");
                header = (collHeader?.Count ?? 0) > 0 ? (collHeader[0]) : null;

                var collNext = parent?.SelectNodes(".//div[@onclick='JumpNext();']")?.ToArray()?.Where(n => n.ChildNodes.Where(sub => sub.Name == "a").Count() > 0);
                nextLink = collNext?.FirstOrDefault();

                if (nextLink==null && header==null && content==null)
                {
                    collCont = parent?.SelectNodes(".//div[@class='chapter-content isTxt']");
                    //content = (collCont?.Count ?? 0) > 0 ? (collCont[0]) : null;
                    content = collCont?.FirstOrDefault();

                    collHeader = parent?.SelectNodes(".//h1[@id='chapter-title']");
                    //header = (collHeader?.Count ?? 0) > 0 ? (collHeader[0]) : null;
                    header = collHeader?.FirstOrDefault();

                    collNext = parent?.SelectNodes(".//div[@class='footer']")?.ToArray()?.Where(n => n.ChildNodes.Where(sub => sub.Name == "a").Count() > 0);
                    nextLink = collNext?.FirstOrDefault();

                }
            }
        }

        public string GetBookHeader(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? header)
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

        public string GetBookNextLink(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? nextLink)
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

        public string GetBookContents(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? content)
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

        public string GetBookName(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? content)
        {
            throw new NotImplementedException();
        }
    }
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning restore CS8601 // Null 参照代入の可能性があります。
#pragma warning restore CS8632 // Null 参照代入の可能性があります。
}