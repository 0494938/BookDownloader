using HtmlAgilityPack;
using System.Diagnostics;
using System.Windows.Controls;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace BookDownloader
{
    public class JinYongBookNovelContent : IFetchNovelContent
    {
        public void AnalysisHtmlBookBody(MainWindow wndMain, WndContextData datacontext, string strBody, bool bSilenceMode = false, DownloadStatus? status = null)
        {
            Debug.Assert(!bSilenceMode || (bSilenceMode && status != null));
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(strBody);
            HtmlNode body = html.DocumentNode.ChildNodes["BODY"];

            HtmlNode nextLink = null;
            HtmlNode content = null;
            HtmlNode header = null;
            HtmlNodeCollection topDiv = body.SelectNodes(".//div[@class='read_bg']");
            if ((topDiv?.Count ?? 0) > 0)
            {
                FindBookNextLinkAndContents(topDiv.First(), ref nextLink, ref header, ref content);
                if (content != null || nextLink != null)
                {
                    string strNextLink = GetBookNextLink(nextLink);
                    string strChapterHeader = GetBookHeader(header);
                    string strContents = " \r\n \r\n " + strChapterHeader + " \r\n" + GetBookContents(content);

                    wndMain.Dispatcher.Invoke(() =>
                    {
                        wndMain.txtAnalysizedContents.Text = strContents;
                        wndMain.txtNextUrl.Text = strNextLink;
                        wndMain.txtCurURL.Text = strNextLink;
                        if (wndMain.txtAggregatedContents.Text.Length > 1024 * 64)
                            wndMain.txtAggregatedContents.Text = strContents;
                        else
                            wndMain.txtAggregatedContents.Text += strContents;
                        wndMain.txtAggregatedContents.ScrollToEnd();
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

        public void FindBookNextLinkAndContents(HtmlNode top, ref HtmlNode nextLink, ref HtmlNode header, ref HtmlNode content)
        {
            HtmlNodeCollection ?collCont = top.SelectNodes("//article[@class='content']");
            content = collCont?.First();

            HtmlNodeCollection? collHeader = top.SelectNodes("//p[@class='style_h1']");
            header = collHeader?.First();

            HtmlNodeCollection? collNextDiv = top.SelectNodes(".//div[@class='read_nav']");
            HtmlNode nextLinkDiv = collNextDiv?.First();

            //<a id="next_url" href="/b/184315/976061_2.html"><i class="fa fa-forward"></i> 下一页</a>
            HtmlNodeCollection? collNext= nextLinkDiv.SelectNodes(".//a[@id='next_url']");
            //HtmlNodeCollection? collNextARef = collNext.First()?.SelectNodes(".//a");
            nextLink = collNext?.First();
        }

        public string GetBookHeader(HtmlNode header)
        {
            return header.InnerText;
        }

        public string GetBookNextLink(HtmlNode nextLink)
        {
            string sUrl = nextLink.Attributes["href"]?.Value??"";
            if (sUrl.StartsWith("http"))
                return sUrl;
            if (sUrl.StartsWith("www"))
            {
                return "https://" + sUrl;
            }
            return "http://www.jinhuaja.com" + sUrl;
        }

        public string GetBookContents(HtmlNode content)
        {
            return content?.InnerText?.Replace("\r", "")?.Replace("\n", "\r\n")?.Replace("&nbsp;", " ")?.Replace("&lt;", "<")?.Replace("&gt;", ">")?.Replace("&amp;", "&")?
                        .Replace("&ensp;", " ")?.Replace("&emsp;", " ")?.Replace("&ndash;", " ")?.Replace("&mdash;", " ")?
                        .Replace("&sbquo;", "“")?.Replace("&rdquo;", "”")?.Replace("&bdquo;", "„")?
                        .Replace("&quot;", "\"")?.Replace("&circ;", "ˆ")?.Replace("&tilde;", "˜")?.Replace("&prime;", "′")?.Replace("&Prime;", "″").Replace("\r\n\r\n\r\n", "\r\n").Replace("\r\n\r\n", "\r\n");
        }

        public string GetBookName(HtmlNode content)
        {
            throw new NotImplementedException();
        }

        public string GetBookName2(HtmlNode content)
        {
            throw new NotImplementedException();
        }
    }
}