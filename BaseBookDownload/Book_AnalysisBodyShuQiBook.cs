using BaseBookDownloader;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace BaseBookDownloader
{
#pragma warning disable IDE0019 // パターン マッチングを使用します
#pragma warning disable IDE0090 // 'new(...)' を使用する
#pragma warning disable CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。
#pragma warning disable CS8601 // Null 参照代入の可能性があります。
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning disable CS8604 // Null 参照引数の可能性があります。
#pragma warning disable CS8629 // Null 許容値型は Null になる場合があります。
#pragma warning disable CS8632 // Null 参照代入の可能性があります。
    public class ShuQiBookNovelContent : BaseBookNovelContent, IFetchNovelContent
    {
        public bool AnalysisHtmlBookBody(IBaseMainWindow wndMain, BaseWndContextData datacontext, string strUrl, string strBody, bool bSilenceMode = false, DownloadStatus? status = null, int nMaxRetry = 0)
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


            int nRetry = 0;
            while (nRetry <= nMaxRetry && body !=null && FindBookNextLinkAndContents2(wndMain, datacontext, body, ref nextLink, ref header, ref novelName, ref content) && !datacontext.UnloadPgm)
            {
                //Debug.Assert(false);
                nRetry++;
                Thread.Sleep(1000);

                strBody = wndMain.GetWebDocHtmlBody(strUrl, true);
                wndMain.UpdateWebBodyOuterHtml(strBody);

                html.LoadHtml(strBody);
                //body = html.DocumentNode.ChildNodes["BODY"];
                body = GetHtmlBody(html);
            }

            if (content != null || nextLink != null)
            {
                string strNextLink = GetBookNextLink(wndMain, datacontext, nextLink);
                string strChapterHeader = GetBookHeader(wndMain, datacontext, header);
                string strContents = " \r\n \r\n " + strChapterHeader + " \r\n" + GetBookContents(wndMain, datacontext, content);
                string strNovelName = GetBookName(wndMain, datacontext, novelName);
                ParseResultToUI(wndMain, bSilenceMode, strContents, strNextLink, strNovelName);

                if (bSilenceMode)
                {
                    Debug.Assert(status != null);
                    status.NextUrl = strNextLink;

                    WriteToFile(status, strChapterHeader, strContents, strNextLink, strNovelName);
                }
                datacontext.NextLinkAnalysized = !string.IsNullOrEmpty(strNextLink);
                wndMain.UpdateNextPageButton();
            }
            return true;
        }

        public void FindBookNextLinkAndContents(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? top, ref HtmlNode? nextLink, ref HtmlNode? header, ref HtmlNode? content, ref HtmlNode novelName)
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
        public bool FindBookNextLinkAndContents2(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? top, ref HtmlNode nextLink, ref HtmlNode header, ref HtmlNode novelName, ref HtmlNode content)
        {
            content = top?.SelectNodes(".//div[@class='chapter-content']")?.FirstOrDefault();
            if (content?.InnerText.Contains("\t\t正文加载中...")??true)
            {
                return true;
            }

            header= top?.SelectNodes(".//h3[@class='chapter-title']")?.FirstOrDefault();
            nextLink = top?.SelectNodes(".//i[@class='page-data js-dataChapters']")?.FirstOrDefault();
            novelName = top?.SelectNodes("//head/title")?.FirstOrDefault();

            return false;
        }

        public string GetBookHeader(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? header)
        {
            if (header != null)
            {
                //dynamic data = JsonConvert.DeserializeObject(header.InnerHtml);
                return header.InnerHtml;
            }
            return "";
        }

        public string GetBookNextLink(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? nextLink)
        {
            if (nextLink != null)
            {
                Uri uri = new Uri(URL);
                string sCurChapterId = UrlUtil.ParseQueryString(uri.Query)["cid"];
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
                            JObject? matchedVolumn = volumeList?.Children().FirstOrDefault(i => GetValueByKeyFromJObject(i as JObject, "chapterId").ToString() == sCurChapterId) as JObject;
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

                                        String bookId = UrlUtil.ParseQueryString(sQuery)["bookId"];
                                        String chapterId = UrlUtil.ParseQueryString(sQuery)["chapterId"];
                                        return "https://www.shuqi.com/reader?bid=" + bookId + "&cid=" + chapterId;

                                    }
                                }
                            }
                        }
                    }
                }
            }
            return "";
        }

        public string GetBookContents(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? content)
        {
            StringBuilder sbContent = new StringBuilder();
            foreach (HtmlNode element in content?.ChildNodes)
            {
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
            return sbContent.ToString().Replace("\r\n\r\n", "\r\n");
        }

        public string GetBookName(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? content)
        {
            //throw new NotImplementedException();
            return content?.InnerText??"" ;
        }

        public string GetBookName2(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode content)
        {
            throw new NotImplementedException();
        }
    }
#pragma warning restore CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。
#pragma warning restore CS8601 // Null 参照代入の可能性があります。
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning restore CS8604 // Null 参照引数の可能性があります。
#pragma warning restore CS8629 // Null 許容値型は Null になる場合があります。
#pragma warning restore CS8632 // Null 参照代入の可能性があります。
#pragma warning restore IDE0019 // パターン マッチングを使用します
#pragma warning restore IDE0090 // 'new(...)' を使用する
}