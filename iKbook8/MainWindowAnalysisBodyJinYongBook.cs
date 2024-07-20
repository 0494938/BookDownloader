using BaseBookDownload;
using HtmlAgilityPack;
using System.Diagnostics;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace BookDownloader
{
#pragma warning disable CS8604 // Null 参照引数の可能性があります。
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning disable CS8601 // Null 参照代入の可能性があります。
    public class JinYongBookNovelContent : BaseBookNovelContent, IFetchNovelContent
    {
        public void AnalysisHtmlBookBody(IBaseMainWindow wndMain, BaseWndContextData datacontext, string strUrl, string strBody, bool bSilenceMode = false, DownloadStatus? status = null, int nMaxRetry = 0)
        {
            this.URL = strUrl;

            Debug.Assert(!bSilenceMode || (bSilenceMode && status != null));
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(strBody);
            HtmlNode body = html.DocumentNode.ChildNodes["BODY"];

            if (body == null)
            {
                wndMain.UpdateStatusMsg(datacontext, "*** URL downloaded BODY is empty, skip this Page *** ", 0);
                return;
            }

            HtmlNode? nextLink = null;
            HtmlNode? content = null;
            HtmlNode? header = null;
            HtmlNodeCollection? topDiv = body.SelectNodes(".//div[@class='read_bg']");
            if ((topDiv?.Count ?? 0) > 0)
            {
                FindBookNextLinkAndContents(topDiv.First(), ref nextLink, ref header, ref content);
                if (content != null || nextLink != null)
                {
                    string strNextLink = GetBookNextLink(nextLink);
                    string strChapterHeader = GetBookHeader(header);
                    string strContents = " \r\n \r\n " + strChapterHeader + " \r\n" + GetBookContents(content);

                    wndMain.GetDispatcher().Invoke(() =>
                    {
                        ParseResultToUI(wndMain, bSilenceMode, strContents, strNextLink);

                    });

                    if (bSilenceMode)
                    {
                        Debug.Assert(status != null);
                        status.NextUrl = strNextLink;

                        DownloadStatus.ContentsWriter?.Write(strContents);
                        DownloadStatus.ContentsWriter?.Flush();
                    }
                    datacontext.NextLinkAnalysized = !string.IsNullOrEmpty(strNextLink);
                    wndMain.GetDispatcher().Invoke(() =>
                    {
                        wndMain.UpdateNextPageButton();
                    });
                    return;
                }
            }
        }


        public void FindBookNextLinkAndContents(HtmlNode? top, ref HtmlNode nextLink, ref HtmlNode header, ref HtmlNode content)
        {
            HtmlNodeCollection ?collCont = top?.SelectNodes("//article[@class='content']");
            content = collCont?.First()??null;

            HtmlNodeCollection? collHeader = top?.SelectNodes("//p[@class='style_h1']");
            header = collHeader?.First();

            HtmlNodeCollection? collNextDiv = top?.SelectNodes(".//div[@class='read_nav']");
            HtmlNode? nextLinkDiv = collNextDiv?.First();

            //<a id="next_url" href="/b/184315/976061_2.html"><i class="fa fa-forward"></i> 下一页</a>
            HtmlNodeCollection? collNext= nextLinkDiv?.SelectNodes(".//a[@id='next_url']");
            //HtmlNodeCollection? collNextARef = collNext.First()?.SelectNodes(".//a");
            nextLink = collNext?.First();
        }

        public string GetBookHeader(HtmlNode? header)
        {
            if (header != null)
                return header.InnerText;
            return "";
        }

        public string GetBookNextLink(HtmlNode? nextLink)
        {
            string sUrl = nextLink?.Attributes["href"]?.Value??"";
            if (sUrl.StartsWith("http"))
                return sUrl;
            if (sUrl.StartsWith("www"))
            {
                return "https://" + sUrl;
            }
            return "http://www.jinhuaja.com" + sUrl;
        }

        public string GetBookContents(HtmlNode? content)
        {
            if (content != null)
                return content?.InnerText?.Replace("\r", "")?.Replace("\n", "\r\n")?.Replace("&nbsp;", " ")?.Replace("&lt;", "<")?.Replace("&gt;", ">")?.Replace("&amp;", "&")?
                        .Replace("&ensp;", " ")?.Replace("&emsp;", " ")?.Replace("&ndash;", " ")?.Replace("&mdash;", " ")?
                        .Replace("&sbquo;", "“")?.Replace("&rdquo;", "”")?.Replace("&bdquo;", "„")?
                        .Replace("&quot;", "\"")?.Replace("&circ;", "ˆ")?.Replace("&tilde;", "˜")?.Replace("&prime;", "′")?.Replace("&Prime;", "″")?.Replace("\r\n\r\n\r\n", "\r\n")?.Replace("\r\n\r\n", "\r\n")??"";
            else
                return "";
        }

        public string GetBookName(HtmlNode? content)
        {
            throw new NotImplementedException();
        }

        public string GetBookName2(HtmlNode content)
        {
            throw new NotImplementedException();
        }
    }
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning restore CS8604 // Null 参照引数の可能性があります。
#pragma warning restore CS8601 // Null 参照代入の可能性があります。
}