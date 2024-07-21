using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace BaseBookDownload
{
#pragma warning disable CS8601 // Null 参照代入の可能性があります。
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning disable CS8632 // Null 参照代入の可能性があります。
    public class IKBook8NovelContent : BaseBookNovelContent, IFetchNovelContent
    {
        public void AnalysisHtmlBookBody(IBaseMainWindow wndMain, BaseWndContextData datacontext, string strUrl, string strBody, bool bSilenceMode = false, DownloadStatus? status = null, int nMaxRetry = 0)
        {
            this.URL = strUrl;

            Debug.Assert(!bSilenceMode || (bSilenceMode && status != null));
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(strBody);
            HtmlNode? body = GetHtmlBody(html);//.DocumentNode.ChildNodes["BODY"];

            if (body == null)
            {
                //.Print("URL downloaded BODY is empty ...");
                wndMain.UpdateStatusMsg(datacontext, "*** URL downloaded BODY is empty, skip this Page *** ", 0);
                return;
            }

            HtmlNode? nextLink = null;
            HtmlNode? header = null;
            HtmlNode? content = null;
            FindBookNextLinkAndContents(wndMain, datacontext, body, ref nextLink, ref header, ref content);
            if (content != null || nextLink != null)
            {
                string strNextLink = GetBookNextLink(wndMain, datacontext, nextLink);
                string strChapterHeader = GetBookHeader(wndMain, datacontext, header);
                //string strContents = GetBookContents(wndMain, datacontext, content);
                string strContents = " \r\n \r\n " + strChapterHeader + " \r\n" + GetBookContents(wndMain, datacontext, content);

                ParseResultToUI(wndMain, bSilenceMode, strContents, strNextLink);

                if (bSilenceMode)
                {
                    Debug.Assert(status != null);
                    status.NextUrl = strNextLink;

                    DownloadStatus.ContentsWriter?.Write(strContents);
                }
                datacontext.NextLinkAnalysized = !string.IsNullOrEmpty(strNextLink);
                wndMain.UpdateNextPageButton();

                return;
            }
        }

        public void FindBookNextLinkAndContents(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? parent, ref HtmlNode? nextLink, ref HtmlNode? header, ref HtmlNode? content)
        {
            //Treat as chrome browser
            HtmlNodeCollection ?collCont=  parent?.SelectNodes(".//div[@id='Lab_Contents'][@class='kong']");
            content = (collCont?.Count?? 0) > 0 ? (collCont[0]):null;
            
            HtmlNodeCollection? collHeader = parent?.SelectNodes(".//h1[@id='ChapterTitle']");
            header = (collHeader?.Count ?? 0) > 0 ? (collHeader[0]) : null;

            var collNext = parent?.SelectNodes(".//div[@onclick='JumpNext();']")?.ToArray()?.Where(n => n.ChildNodes.Where(sub => sub.Name == "a").Count() > 0);
            nextLink = collNext?.FirstOrDefault();

            if(collCont == null && header== null && nextLink == null)
            {
                //Treat as IE core browser
                collCont = parent?.SelectNodes(".//div[@class='content']");
                content = collCont?.Where(n => n.ChildNodes.Count() >= 6).FirstOrDefault();


                collHeader = parent?.SelectNodes(".//h1[@class='title']");
                header = (collHeader?.Count ?? 0) > 0 ? (collHeader[0]) : null;

                collNext = parent?.SelectNodes(".//div[@class='section-opt']")?.ToArray()?.Where(n => n.ChildNodes.Where(sub => sub.Name == "a").Count() > 0);
                nextLink = collNext?.FirstOrDefault();
            }

        }

        public string GetBookContents(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? content)
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
                else if (string.Equals("p", element.Name))
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
            }
            return sbContent.ToString().Replace("\r\n\r\n", "\r\n");
        }

        public string GetBookHeader(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? header)
        {
            return header?.InnerText??"";
        }

        public string GetBookName(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? content)
        {
            throw new NotImplementedException();
        }

        public string GetBookNextLink(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? nextLink)
        {
            foreach (HtmlNode element in nextLink?.ChildNodes)
            {
                //hrefTags.Add(element.GetAttribute("href"));
                if (string.Equals("a", element.Name))
                {
                    if (string.Equals(element.InnerHtml, "下一页") || string.Equals(element.InnerHtml, "下一章"))
                    {
                        string? sUrl = element?.Attributes["href"]?.Value ?? "";
                        if (sUrl.StartsWith("http"))
                            return sUrl;
                        else
                            return "https://www.wxdzs.net" + sUrl;
                    }
                }
            }
            return "";
        }
    }
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning restore CS8601 // Null 参照代入の可能性があります。
#pragma warning restore CS8632 // Null 参照代入の可能性があります。
}