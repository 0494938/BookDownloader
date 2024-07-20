using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text;
using System.Windows.Controls;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace BookDownloader
{
    public class YQXSBBookNovelContent : BaseBookNovelContent, IFetchNovelContent
    {
#pragma warning disable CS8604 // Null 参照引数の可能性があります。
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning disable CS8601 // Null 参照代入の可能性があります。
        public void AnalysisHtmlBookBody(MainWindow? wndMain, WndContextData? datacontext, string strBody, bool bSilenceMode = false, DownloadStatus? status = null, int nMaxRetry = 0)
        {
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
            HtmlNode? topDiv = body?.SelectNodes(".//div[@id='j_readMainWrap'][@class='read-main-wrap font-family01']").FirstOrDefault();
            if (topDiv != null)
            {
                FindBookNextLinkAndContents(topDiv, ref nextLink, ref header, ref content);
                if (content != null || nextLink != null)
                {
                    string strNextLink = GetBookNextLink(nextLink);
                    string strChapterHeader = GetBookHeader(header);
                    string strContents = " \r\n \r\n " + strChapterHeader + " \r\n" + GetBookContents(content);

                    wndMain.Dispatcher.Invoke(() =>
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
                    wndMain.Dispatcher.Invoke(() =>
                    {
                        wndMain.btnNextPage.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                    });
                    return;
                }
            }
        }
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning restore CS8604 // Null 参照引数の可能性があります。
        public void FindBookNextLinkAndContents(HtmlNode? top, ref HtmlNode nextLink, ref HtmlNode header, ref HtmlNode content)
        {
            content = top?.SelectNodes(".//div[@class='ywskythunderfont']").FirstOrDefault();

            header = top?.SelectNodes(".//h1[@class='j_chapterName']").FirstOrDefault();

            nextLink = top?.SelectNodes(".//a[@id='j_chapterNext']").FirstOrDefault();
        }
#pragma warning restore CS8601 // Null 参照代入の可能性があります。

        public string GetBookHeader(HtmlNode? header)
        {
            if (header != null)
                return header.InnerText;
            return "";
        }

        public string GetBookNextLink(HtmlNode? nextLink)
        {

            if(nextLink != null)
            {
                return "https://www.xs8.cn" + nextLink?.Attributes["href"]?.Value ;
            }

            return "";

            //return "https://www.xs8.cn" + sUrl;
        }

#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
        public string GetBookContents(HtmlNode? content)
        {
            if (content != null)
            {
                StringBuilder sbContent = new StringBuilder();
                foreach (HtmlNode element in content?.ChildNodes)
                {
                    if(element.Name == "p")
                    {
                        string? strLine = element.InnerText?.Replace("\r", "")?.Replace("\n", "")?.Replace("&nbsp;", " ")?.Replace("&lt;", "<")?.Replace("&gt;", ">")?.Replace("&amp;", "&")?
                            .Replace("&ensp;", " ")?.Replace("&emsp;", " ")?.Replace("&ndash;", " ")?.Replace("&mdash;", " ")?
                            .Replace("&sbquo;", "“")?.Replace("&rdquo;", "”")?.Replace("&bdquo;", "„")?
                            .Replace("&quot;", "\"")?.Replace("&circ;", "ˆ")?.Replace("&tilde;", "˜")?.Replace("&prime;", "′")?.Replace("&Prime;", "″");
                        if (!string.IsNullOrEmpty(strLine?.Trim()))
                        {
                            sbContent.Append(strLine).AppendLine();
                        }
                    }
                }
                return sbContent.ToString().Replace("\r\n\r\n\r\n", "\r\n").Replace("\r\n\r\n", "\r\n");
            }

            return "";
        }
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。

        public string GetBookName(HtmlNode? content)
        {
            throw new NotImplementedException();
        }

        public string GetBookName2(HtmlNode content)
        {
            throw new NotImplementedException();
        }
    }
}