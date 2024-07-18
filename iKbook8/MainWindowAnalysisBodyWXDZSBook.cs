using HtmlAgilityPack;
using System.Diagnostics;
using System.Windows.Controls;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace iKbook8
{
    public class WxdzsBookNovelContent : IFetchNovelContent
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
            HtmlNodeCollection topDiv = body.SelectNodes(".//div[@id='PageBody']");
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
            HtmlNodeCollection ?collCont = top.SelectNodes(".//div[@id='Lab_Contents']");
            content = collCont?.First();

            HtmlNodeCollection? collHeader = top.SelectNodes(".//h1[@id='ChapterTitle']");
            header = collHeader?.First();

            HtmlNodeCollection? collNextDiv = top.SelectNodes(".//div[@id='Pan_Top']");
            HtmlNode nextLinkDiv = collNextDiv?.First();

            //<div onclick="JumpNext();" class="erzitop_"><a title="第002章 抓捕  我的谍战岁月" href="/wxread/94612_43816525.html">下一章</a> </div>
            HtmlNodeCollection? collNext= nextLinkDiv.SelectNodes(".//div[@onclick='JumpNext();']");
            HtmlNodeCollection? collNextARef = collNext.First()?.SelectNodes(".//a");
            nextLink = collNextARef?.First();
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
            return "https://www.wxdzs.net" + sUrl;
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