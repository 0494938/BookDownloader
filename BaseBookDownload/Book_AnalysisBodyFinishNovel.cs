using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace BaseBookDownloader
{
#pragma warning disable CS8601 // Null 参照代入の可能性があります。
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning disable CS8604 // Null 参照引数の可能性があります。
#pragma warning disable CS8632 // Null 参照代入の可能性があります。

    public class NovelFinishBookNovelContent : BaseBookNovelContent, IFetchNovelContent
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
            if (body != null)
            {
                FindBookNextLinkAndContents(wndMain, datacontext, body, ref nextLink, ref header, ref content, ref novelName);
                if (content != null || nextLink != null)
                {
                    //novelName = body;
                    FinishDocAnalyis(wndMain, datacontext, nextLink, header, content, novelName, bSilenceMode, status);
                    return true;
                }
                else
                {
                    //retry when returns Error 1015: The owner of this website (www.keleshuba.net) has banned you temporarily from accessing this website.
                    HtmlNode? bErrorBlock =  body?.SelectNodes(".//h1[@class='inline-block md:block mr-2 md:mb-2 font-light text-60 md:text-3xl text-black-dark leading-tight']")?.FirstOrDefault(); 
                    if (bErrorBlock != null)
                    {
                        if(bErrorBlock?.InnerText?.Replace(" ","")?.Replace("\t","")?.Replace("\r", "")?.Replace("\n", "") == "Error1015")
                        {
                            if (datacontext.RefreshCount == 0)
                                datacontext.RefreshCount = datacontext.MAX_REFRESH_CNT;
                            else
                                datacontext.RefreshCount= datacontext.RefreshCount - 1;
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private void FinishDocAnalyis(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? nextLink, HtmlNode? header, HtmlNode? content, HtmlNode? novelName, bool bSilenceMode, DownloadStatus? status = null)
        {
            string strNextLink = GetBookNextLink(wndMain, datacontext, nextLink);
            string strChapterHeader = GetBookHeader(wndMain, datacontext, header);
            string strContents = " \r\n \r\n " + strChapterHeader + " \r\n" + GetBookContents(wndMain, datacontext, content);
            string strNovelName = GetBookName(wndMain, datacontext, novelName);
            if (strNovelName.StartsWith(strChapterHeader))
                strNovelName = strNovelName.Substring(strChapterHeader.Length);
            strNovelName = strNovelName.Trim(new char[] { ' ','_'});
            strNovelName = strNovelName.Replace("小说无防盗章节_", "").Replace("小说无防盗章节", "");
            strNovelName = strNovelName.Replace("(www.qbxsw.com)", "");
            ParseResultToUI(wndMain, datacontext, bSilenceMode, strContents, strNextLink, strChapterHeader, strNovelName);

            if (bSilenceMode)
            {
                Debug.Assert(status != null);
                status.NextUrl = strNextLink;

                WriteToFile(status, strChapterHeader, strContents, strNextLink, strNovelName);
            }
            datacontext.NextLinkAnalysized = !string.IsNullOrEmpty(strNextLink);
            wndMain.UpdateNextPageButton();
            return;
        }

        public void FindBookNextLinkAndContents(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? top, ref HtmlNode? nextLink, ref HtmlNode? header, ref HtmlNode?  content, ref HtmlNode novelName)
        {
            content = top?.SelectNodes(".//div[@id='content']")?.FirstOrDefault();

            header = top?.SelectNodes(".//h1")?.FirstOrDefault();

            nextLink = top?.SelectNodes(".//div[@class='bottem2']")?.Descendants().Where(n => n.Name =="a" && (n.InnerText == "下一章" || n.InnerText.Trim() == "下一页" || n.InnerText.Trim() == "下一节"))?.FirstOrDefault();
            novelName = top?.SelectNodes("//head")?.Descendants().Where(n => n.Name == "title").FirstOrDefault();
        }

        public string GetBookHeader(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? header)
        {
            if (header != null)
                return header.InnerText.Trim();
            return "";
        }

        public string GetBookNextLink(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? nextLink)
        {

            if(nextLink != null)
            {
                string? sLink = nextLink?.Attributes["href"]?.Value;
                if (sLink.StartsWith("http"))
                    return sLink;
                else
                {
                    return "https://www.qbxsw.com" + nextLink?.Attributes["href"]?.Value;
                }
            }

            return "";
        }

        public string GetBookContents(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? content, string? key = null)
        {
            if (content != null)
            {
                StringBuilder sb = new StringBuilder();
                sb = CascadeGetTagP_TagBr_TagText(sb, content);
                return (ReformContent(sb)??"").Replace("a_336();","");
            }

            return "";
        }

        public string GetBookName(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? novelName)
        {
            string sNovelName = novelName?.InnerText??"";
            return sNovelName;
        }

        public string GetBookName2(HtmlNode content)
        {
            throw new NotImplementedException();
        }

        public bool AnalysisHtmlStream(IBaseMainWindow wndMain, BaseWndContextData datacontext, string strURL, string strBody, bool bSilenceMode = false, DownloadStatus? status = null, int nMaxRetry = 0, bool bForceDownload = false)
        {
            throw new NotImplementedException();
        }
    }
#pragma warning restore CS8601 // Null 参照代入の可能性があります。
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning restore CS8604 // Null 参照引数の可能性があります。
#pragma warning restore CS8632 // Null 参照代入の可能性があります。
}