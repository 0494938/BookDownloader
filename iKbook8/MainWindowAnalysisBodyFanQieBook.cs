using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text;
using System.Windows.Controls;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace BookDownloader
{
    public class FanQieBookNovelContent : BaseBookNovelContent, IFetchNovelContent
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
            //HtmlNodeCollection? topDiv = body.SelectNodes(".//div[@class='read_bg']");
            if (body!=null)
            {
                FindBookNextLinkAndContents(body, ref nextLink, ref header, ref content);
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
#pragma warning restore CS8601 // Null 参照代入の可能性があります。

#pragma warning disable CS8601 // Null 参照代入の可能性があります。
        public void FindBookNextLinkAndContents(HtmlNode? top, ref HtmlNode nextLink, ref HtmlNode header, ref HtmlNode content)
        {
            HtmlNodeCollection ?collCont = top?.SelectNodes("//div[@class='muye-reader-content noselect']");
            content = collCont?.First()??null;

            HtmlNodeCollection? collHeader = top?.SelectNodes("//h1[@class='muye-reader-title']");
            header = collHeader?.First();

            IEnumerable<HtmlNode>? collNextScript = top?.SelectNodes(".//script")?.Where(n => n.InnerHtml.Contains("window.__INITIAL_STATE__="));
            nextLink = collNextScript?.First();
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
            string sScript = nextLink?.InnerText?.Trim()??"";
            sScript = sScript.Substring(1, sScript.Length-4).Trim();
            sScript = sScript.Substring("function()".Length).Trim();
            sScript = sScript.Substring(1,sScript.Length-2).Trim();
            sScript = sScript.Substring("window.__INITIAL_STATE__=".Length);
            sScript = sScript.Substring(0, sScript.Length-1).Trim();

            JObject? data = JsonConvert.DeserializeObject(sScript) as JObject;
            JObject? reader = data?["reader"] as JObject;
            JObject? chapterData = reader?["chapterData"] as JObject;
            var nextItemId = chapterData?["nextItemId"];

            if (nextItemId == null)
                return "";
            else 
                return "https://fanqienovel.com/reader/" + nextItemId.ToString() + "?enter_from=page";


            //if (sUrl.StartsWith("http"))
            //    return sUrl;
            //if (sUrl.StartsWith("www"))
            //{
            //    return "https://" + sUrl;
            //}
            //return "http://www.jinhuaja.com" + sUrl;
        }

        public string GetBookContents(HtmlNode? content)
        {
            if (content != null)
            {
                IEnumerable<HtmlNode>? divContents = content.Descendants().Where(n => n.Name == "div" && string.IsNullOrEmpty(n.Attributes["id"]?.Value ?? "") && string.IsNullOrEmpty(n.Attributes["name"]?.Value ?? ""));
                if(divContents!=null && divContents?.Count() > 0)
                {
                    content = divContents.FirstOrDefault();
                    StringBuilder sbContent = new StringBuilder();
                    //strContents = "";
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
                    foreach (HtmlNode element in content?.ChildNodes)
                    {
                        //hrefTags.Add(element.GetAttribute("href"));
                        if (string.Equals("p", element.Name))
                        {
                            string? strLine = DotDecodingUtil.DecodeDitStr(element.InnerText)?.Replace("\r", "")?.Replace("\n", "")?.Replace("&nbsp;", " ")?.Replace("&lt;", "<")?.Replace("&gt;", ">")?.Replace("&amp;", "&") ?
                                .Replace("&ensp;", " ")?.Replace("&emsp;", " ")?.Replace("&ndash;", " ")?.Replace("&mdash;", " ")?
                                .Replace("&sbquo;", "“")?.Replace("&rdquo;", "”")?.Replace("&bdquo;", "„")?
                                .Replace("&quot;", "\"")?.Replace("&circ;", "ˆ")?.Replace("&tilde;", "˜")?.Replace("&prime;", "′")?.Replace("&Prime;", "″");
                            if (!string.IsNullOrEmpty(strLine?.Trim()))
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
                    return sbContent.ToString().Replace("\r\n\r\n\r\n", "\r\n").Replace("\r\n\r\n", "\r\n");
                }
            }

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
}