using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace BaseBookDownloader
{
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning disable CS8601 // Null 参照代入の可能性があります。
#pragma warning disable CS8632 // Null 参照代入の可能性があります。
    public class OOBookNovelContent : BaseBookNovelContent, IFetchNovelContent
    {
        public bool AnalysisHtmlBook(IBaseMainWindow wndMain, BaseWndContextData datacontext, string strUrl, string strBody, bool bSilenceMode = false, DownloadStatus? status = null, int nMaxRetry = 0)
        {
            this.URL = strUrl;

            Debug.Assert(!bSilenceMode || (bSilenceMode && status != null));
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(strBody);
            HtmlNode? body = GetHtmlBody(html);

            if (body == null)
            {
                wndMain.UpdateStatusMsg(datacontext, "*** URL downloaded BODY is empty, skip this Page *** ", 0);
                return true;
            }

            HtmlNode? nextLink = null;
            HtmlNode? content = null;
            HtmlNode? header = null;
            HtmlNode? novelName = null;
            FindBookNextLinkAndContents(wndMain, datacontext, body, ref nextLink, ref header, ref content, ref novelName);
            if (content != null || nextLink != null)
            {
                string strNextLink = GetBookNextLink(wndMain, datacontext, nextLink);
                string strChapterHeader = GetBookHeader(wndMain, datacontext, header);
                string strContents = " \r\n \r\n " + strChapterHeader + " \r\n" + GetBookContents(wndMain, datacontext, content);
                string strNovelName = GetBookName(wndMain, datacontext, novelName);
                ParseResultToUI(wndMain, datacontext, bSilenceMode, strContents, strNextLink, strChapterHeader, strNovelName);
                if (bSilenceMode)
                {
                    Debug.Assert(status != null);
                    status.NextUrl = strNextLink;

                    WriteToFile(datacontext, status, strUrl, strChapterHeader, strContents, strNextLink, strNovelName);
                }
                datacontext.NextLinkAnalysized = !string.IsNullOrEmpty(strNextLink);
                wndMain.UpdateNextPageButton();
            }
            return true;
        }

        public void FindBookNextLinkAndContents(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? parent, ref HtmlNode? nextLink, ref HtmlNode? header, ref HtmlNode? content, ref HtmlNode novelName)
        {
            if(parent != null)
            {

                //HtmlNodeCollection? collCont = parent?.SelectNodes(".//div[@id='Lab_Contents'][@class='kong']");
                content = parent?.SelectNodes(".//div[@id='Lab_Contents'][@class='kong']")?.FirstOrDefault();

                //HtmlNodeCollection? collHeader = parent?.SelectNodes(".//h1[@id='ChapterTitle']");
                header = parent?.SelectNodes(".//h1[@id='ChapterTitle']")?.FirstOrDefault();

                //var collNext = parent?.SelectNodes(".//div[@onclick='JumpNext();']")?.ToArray()?.Where(n => n.ChildNodes.Where(sub => sub.Name == "a").Count() > 0);
                nextLink = parent?.SelectNodes(".//div[@onclick='JumpNext();']")?.ToArray()?.Where(n => n.ChildNodes.Where(sub => sub.Name == "a").Count() > 0)?.FirstOrDefault();

                if (nextLink==null && header==null && content==null)
                {
                    //collCont = parent?.SelectNodes(".//div[@class='chapter-content isTxt']");
                    content = parent?.SelectNodes(".//div[@class='chapter-content isTxt']")?.FirstOrDefault();

                    //collHeader = parent?.SelectNodes(".//h1[@id='chapter-title']");
                    //header = (collHeader?.Count ?? 0) > 0 ? (collHeader[0]) : null;
                    header = parent?.SelectNodes(".//h1[@id='chapter-title']")?.FirstOrDefault();

                    //collNext = parent?.SelectNodes(".//div[@class='footer']")?.ToArray()?.Where(n => n.ChildNodes.Where(sub => sub.Name == "a").Count() > 0);
                    nextLink = parent?.SelectNodes(".//div[@class='footer']")?.ToArray()?.Where(n => n.ChildNodes.Where(sub => sub.Name == "a").Count() > 0)?.FirstOrDefault();

                }

                novelName = parent?.SelectNodes(".//h2[@class='page-breadcrumb-item']")?.FirstOrDefault();
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

        public string GetBookContents(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? content, string? key = null)
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
                        string? strLine = ReformLine(element.InnerText);
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
            //throw new NotImplementedException();
            return content?.InnerText??"";
        }

        public bool AnalysisHtmlStream(IBaseMainWindow wndMain, BaseWndContextData datacontext, string strURL, string strBody, bool bSilenceMode = false, DownloadStatus? status = null, int nMaxRetry = 0, bool bForceDownload = false)
        {
            throw new NotImplementedException();
        }
    }
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning restore CS8601 // Null 参照代入の可能性があります。
#pragma warning restore CS8632 // Null 参照代入の可能性があります。
}