using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;


namespace BaseBookDownloader
{
#pragma warning disable CS8601 // Null 参照代入の可能性があります。
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning disable CS8604 // Null 参照引数の可能性があります。
#pragma warning disable CS8632 // Null 参照代入の可能性があります。

    public class PornHubStreamPageContent : BaseBookNovelContent, IFetchNovelContent
    {
        public bool AnalysisHtmlBook(IBaseMainWindow wndMain, BaseWndContextData datacontext, string strUrl, string strBody, bool bSilenceMode = false, DownloadStatus? status = null, int nMaxRetry = 0)
        {
            //throw new NotImplementedException();
            return true;
        }

        private static readonly Regex _regexHttpGet = new Regex(@"[?&](\w[\w.]*)=([^?&]+)");
        public static System.Collections.Generic.Dictionary<string, string> ParseUriQueryString(Uri uri)
        {
            return ParseQueryString(uri.PathAndQuery);
        }

        public static System.Collections.Generic.Dictionary<string, string> ParseQueryString(string sUrl)
        {
            var match = _regexHttpGet.Match(sUrl);
            System.Collections.Generic.Dictionary<string, string> paramaters = new System.Collections.Generic.Dictionary<string, string>();
            while (match.Success)
            {
                paramaters.Add(match.Groups[1].Value, match.Groups[2].Value);
                match = match.NextMatch();
            }
            return paramaters;
        }


        private void FinishDocAnalyis(IBaseMainWindow wndMain, BaseWndContextData datacontext, string strUrl, HtmlNode? nextLink, HtmlNode? header, HtmlNode? content, HtmlNode? novelName, bool bSilenceMode, DownloadStatus? status = null, bool bForceDownload=true)
        {
            //System.Collections.Generic.Dictionary<string, string> paras = ParseQueryString(strUrl);
            //string sVideoKey = paras.ContainsKey("viewkey") ? paras["viewkey"] : "";

            //if (!string.IsNullOrEmpty(sVideoKey))
            {
                string strNextLink = GetBookNextLink(wndMain, datacontext, nextLink);
                string strChapterHeader = GetBookHeader(wndMain, datacontext, header);
                string strContents = GetBookContents(wndMain, datacontext, content/*, sVideoKey*/);
                string strNovelName = strChapterHeader;
                ParseResultToUI(wndMain, datacontext, bSilenceMode, strContents, strNextLink, strChapterHeader, strNovelName);

                if (bSilenceMode)
                {
                    Debug.Assert(status != null);
                    status.NextUrl = strNextLink;

                    WriteToFile(status, strChapterHeader, strContents, strNextLink, strNovelName);
                }

                JObject? data = JsonConvert.DeserializeObject(strContents) as JObject;
                JArray? reader = data?["mediaDefinitions"] as JArray;
                System.Collections.Generic.Dictionary<string, string> dictQuality = new System.Collections.Generic.Dictionary<string, string>();
                if (reader != null)
                {
                    foreach (var slice in reader)
                    {
                        JObject? oOutputQuality = slice as JObject;
                        dictQuality[oOutputQuality["quality"].ToString()] = oOutputQuality["videoUrl"].ToString();
                    }
                    wndMain.DownloadFile(datacontext, dictQuality, strNovelName, bForceDownload);
                }

                datacontext.NextLinkAnalysized = true;
                wndMain.UpdateNextPageButton();
            }
            
            return;
        }

        public void FindBookNextLinkAndContents(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? top, ref HtmlNode? nextLink, ref HtmlNode? header, ref HtmlNode?  content, ref HtmlNode novelName)
        {
            //https://iv-h.phncdn.com/-horv25lqCXRADR_pJ0pniNk_j8=,1722842704/hls/videos/202404/11/450925391/1080P_4000K_450925391.mp4/index-v1-a1.m3u8
            content = top?.SelectNodes(".//div[@id='player'][@class='original mainPlayerDiv']")?.FirstOrDefault();
            string strVideoId = content?.Attributes["data-video-id"]?.Value ?? "";
            if ((!string.IsNullOrEmpty(strVideoId)))
            {
                header = top?.SelectNodes(".//h1[@class='title translate']")?.FirstOrDefault();
                content = content?.Descendants()?.Where(n => n.Name == "script")?.FirstOrDefault();
            }
            else
            {
                content = null;
                header = null;
            }

            nextLink = top?.SelectNodes(".//ul[@id='recommMenuSection'][@class='dropdownReccomendedVideos videos']")?.FirstOrDefault();
            //nextLink = nextLink?.Descendants()?.Where(n => n.Name == "script" && n.InnerText.Contains("var VIDEO_SHOW ="))?.FirstOrDefault();

            novelName = header; // top?.SelectNodes(".//p[@id='bookname']")?.FirstOrDefault();
        }

        public string GetBookHeader(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? header)
        {
            if (header != null)
                return header.InnerText.Trim(new char[] { ' ','\t','\r','\n'});
            return "";
        }

        public string GetBookNextLink(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? nextLink)
        {
            if (false && nextLink != null)
            {
                string strScript = nextLink?.InnerText?.TrimStart(new char[] { ' ', '\t', '\r', '\n' }) ?? "";
                StringReader sr = new StringReader(strScript);
                string? strPureScript = "";
                bool bFound = false;
                int nLine = 0;
                string ?sLine = null;
                while (nLine < 10 && (sLine = sr.ReadLine())!=null )
                {
                    nLine++;
                    sLine = sLine.Trim();
                    if (string.IsNullOrEmpty(sLine))
                        continue;
                    if (sLine.StartsWith("//"))
                        continue;
                    if (sLine.StartsWith("var VIDEO_SHOW ="))
                    {
                        strPureScript = sLine.Substring("var VIDEO_SHOW =".Length);
                        strPureScript = strPureScript.TrimEnd(';');
                    }
                    return strPureScript ?? "";
                }
            }
            return "";
        }

        public string GetBookContents(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? content, string? key = null)
        {
            if (content != null)
            {
                key = content?.ParentNode?.Attributes["data-video-id"]?.Value??"";
                Debug.Assert(!string.IsNullOrEmpty(key));
                string strScript = content?.InnerText?.TrimStart(new char[] {' ','\t','\r','\n'})??"";
                StringReader sr = new StringReader(strScript);
                string? strPureScript = sr.ReadLine();
                Debug.Assert(strPureScript.StartsWith("var flashvars_" + key + " ="));
                if (strPureScript.StartsWith("var flashvars_" + key + " ="))
                {
                    strPureScript = strPureScript.Substring(("var flashvars_"+ key+" =").Length);
                    strPureScript = strPureScript.TrimEnd(new char[] { ';', ' ', '\t', '\r', '\n' });
                    return strPureScript ?? "";
                }
            }

            return "";
        }

        public string GetBookName(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? bookName)
        {
            return "" ;
        }

        public string GetBookName2(HtmlNode content)
        {
            throw new NotImplementedException();
        }

        public bool AnalysisHtmlStream(IBaseMainWindow wndMain, BaseWndContextData datacontext, string strURL, string strBody, bool bSilenceMode = false, DownloadStatus? status = null, int nMaxRetry = 0, bool bForceDownload = false)
        {
            this.URL = strURL;

            Debug.Assert(!bSilenceMode || (bSilenceMode && status != null));
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(strBody);
            HtmlNode? body = GetHtmlBody(html);

            if (body == null)
            {
                wndMain.UpdateStatusMsg(datacontext, "*** URL downloaded BODY is empty, skip this Page *** ", 0);
                return true;
            }

            HtmlNode? currIdentifier = null;
            HtmlNode? content = null;
            HtmlNode? header = null;
            HtmlNode? novelName = null;
            if (body != null)
            {
                FindBookNextLinkAndContents(wndMain, datacontext, body, ref currIdentifier, ref header, ref content, ref novelName);
                if (currIdentifier != null || content !=null)
                {
                    FinishDocAnalyis(wndMain, datacontext, strURL, currIdentifier, header, content, novelName, bSilenceMode, status, bForceDownload);
                    return true;
                }

            }
            return true;
        }
    }
#pragma warning restore CS8601 // Null 参照代入の可能性があります。
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning restore CS8604 // Null 参照引数の可能性があります。
#pragma warning restore CS8632 // Null 参照代入の可能性があります。
}