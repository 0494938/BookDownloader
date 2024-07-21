using HtmlAgilityPack;
using System;

namespace BaseBookDownload
{
#pragma warning disable CS8632 // Null 参照代入の可能性があります。
    public interface IBookDownloader
    {

    }
    public interface IFetchNovelContent
    {
        public void AnalysisHtmlBookBody(IBaseMainWindow wndMain, BaseWndContextData datacontext, string strURL, string strBody, bool bSilenceMode = false, DownloadStatus? status = null, int nMaxRetry = 0);
        public void FindBookNextLinkAndContents(HtmlNode? parent, ref HtmlNode nextLink, ref HtmlNode header, ref HtmlNode content);
        public string GetBookHeader(HtmlNode? header);
        public string GetBookNextLink(HtmlNode? nextLink);
        public string GetBookContents(HtmlNode? content);
        public string GetBookName(HtmlNode? content);
    }
#pragma warning restore CS8632 // Null 参照代入の可能性があります。
}
