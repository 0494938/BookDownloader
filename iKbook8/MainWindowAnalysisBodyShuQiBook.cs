using HtmlAgilityPack;
using MSHTML;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text;
using System.Web;
using System.Windows.Controls;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace BookDownloader
{
    public class ShuQiBookNovelContent : BaseBookNovelContent, IFetchNovelContent
    {
        public void AnalysisHtmlBookBody(MainWindow? wndMain, WndContextData? datacontext, string strBody, bool bSilenceMode = false, DownloadStatus? status = null, int nMaxRetry = 0)
        {
            Debug.Assert(!bSilenceMode || (bSilenceMode && status != null));
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(strBody);
            HtmlNode body = html.DocumentNode.ChildNodes["BODY"];

#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning disable CS8601 // Null 参照代入の可能性があります。
            if (body == null)
            {
                wndMain.UpdateStatusMsg(datacontext, "*** URL downloaded BODY is empty, skip this Page *** ", 0);
                return;
            }
            HtmlNode? nextLink = null;
            HtmlNode? content = null;
            HtmlNode? header = null;
            HtmlNode? title = null;


            int nRetry = 0;
            while (nRetry <= nMaxRetry && body !=null && FindBookNextLinkAndContents2(body, ref nextLink, ref header, ref title, ref content))
            {
                nRetry++;
                Thread.Sleep(3000);

                wndMain.Dispatcher.Invoke(() =>
                {
                    IHTMLDocument2? hTMLDocument2 = wndMain.webBrowser.Document as IHTMLDocument2;
                    IHTMLElement? iBody = hTMLDocument2?.body as IHTMLElement;
                    strBody = iBody?.outerHTML ?? "";
                    wndMain.txtWebContents.Text = strBody;
                });


                html.LoadHtml(strBody);
                body = html.DocumentNode.ChildNodes["BODY"];
            }

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
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning restore CS8601 // Null 参照代入の可能性があります。
        }

        public void FindBookNextLinkAndContents(HtmlNode? top, ref HtmlNode nextLink, ref HtmlNode header, ref HtmlNode content)
        {

        }

        /// </summary>
        /// <param name="top"></param>
        /// <param name="nextLink"></param>
        /// <param name="header"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <returns> when return True, means need Retry... when False, means No Retry
        /// </returns>
#pragma warning disable CS8601 // Null 参照代入の可能性があります。
        public bool FindBookNextLinkAndContents2(HtmlNode? top, ref HtmlNode nextLink, ref HtmlNode header, ref HtmlNode title, ref HtmlNode content)
        {
            HtmlNodeCollection ?collCont = top?.SelectNodes(".//div[@class='chapter-content']");
            content = collCont?.First();
            if (content?.InnerText.Contains("\t\t正文加载中...")??true)
            {
                return true;
            }

            HtmlNodeCollection? collTitle = top?.SelectNodes(".//i[@class='page-data js-dataBookInfo']");
            title = collTitle?.First();
            
            HtmlNodeCollection? collHeader = top?.SelectNodes(".//h3[@class='chapter-title']");
            header= collHeader?.First();

            HtmlNodeCollection? collNextDiv = top?.SelectNodes(".//i[@class='page-data js-dataChapters']");
            HtmlNode? nextLinkDiv = collNextDiv?.First();

            nextLink = nextLinkDiv;

            return false;
        }

        public string GetBookHeader(HtmlNode? header)
        {
            if (header != null)
            {
                //dynamic data = JsonConvert.DeserializeObject(header.InnerHtml);
                return header.InnerHtml;
            }
            return "";
        }
#pragma warning restore CS8601 // Null 参照代入の可能性があります。

#pragma warning disable CS8629 // Null 許容値型は Null になる場合があります。
#pragma warning disable CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。
        public string GetBookNextLink(HtmlNode? nextLink)
        {
#pragma warning disable IDE0019 // パターン マッチングを使用します
#pragma warning disable CS8604 // Null 参照引数の可能性があります。
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
            if (nextLink != null)
            {
                JObject? data = JsonConvert.DeserializeObject(nextLink.InnerHtml) as JObject;
                Debug.Assert(data != null);
                if (data != null)
                {
                    JArray? chapterList = GetValueByKeyFromJObject(data, "chapterList") as JArray;
                    ;   foreach(JToken chapter in chapterList?.Children())
                    {
                        JArray? volumeList = GetValueByKeyFromJObject(chapter as JObject, "volumeList") as JArray;
                        if (volumeList!=null)
                        {
                            JObject? matchedVolumn = volumeList?.Children().FirstOrDefault(i => GetValueByKeyFromJObject(i as JObject, "chapterId").ToString() == "2589796") as JObject;
                            if (matchedVolumn != null)
                            {
                                string strUri = GetValueByKeyFromJObject(matchedVolumn as JObject, "contUrlSuffix")?.ToString();
                                int nCurIdx = volumeList.IndexOf(matchedVolumn);
                                if (nCurIdx +1 < volumeList.Count())
                                {
                                    JObject ?nextObj = volumeList[nCurIdx + 1] as JObject;
                                    if (nextObj != null)
                                    {
                                        string sQuery = GetValueByKeyFromJObject(nextObj, "contUrlSuffix")?.ToString()?.Replace("&amp;", "&");
                              //Uri myUri = new Uri("https://www.shuqi.com/reader" + GetValueByKeyFromJObject(nextObj, "contUrlSuffix")?.ToString());

                                        String bookId = HttpUtility.ParseQueryString(sQuery).Get("bookId");
                                        String chapterId = HttpUtility.ParseQueryString(sQuery).Get("chapterId");
                                        //"?bookId=8991909&amp;chapterId=2589796&amp;ut=1719899719&amp;ver=1&amp;aut=1720257026&amp;sign=072ac1f766f98a731553579ba714c7e2",
                                        // https://www.shuqi.com/reader?bid=8991909&cid=2589797
                                        return "https://www.shuqi.com/reader?bid=" + bookId + "&cid=" + chapterId;

                                    }
                                }
                            }
                        }
                    }
                }
            }
#pragma warning restore CS8604 // Null 参照引数の可能性があります。
#pragma warning restore IDE0019 // パターン マッチングを使用します
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
            return "";
        }

        public string GetBookContents(HtmlNode? content)
        {
            StringBuilder sbContent = new StringBuilder();
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
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
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
            return sbContent.ToString().Replace("\r\n\r\n", "\r\n");
        }
#pragma warning restore CS8629 // Null 許容値型は Null になる場合があります。
#pragma warning restore CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。

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