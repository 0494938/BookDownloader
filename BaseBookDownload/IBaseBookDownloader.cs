using HtmlAgilityPack;
using System;

namespace BaseBookDownload
{
#pragma warning disable CS8632 // Null 参照代入の可能性があります。
    public interface IFetchNovelContent
    {
        public bool AnalysisHtmlBookBody(IBaseMainWindow wndMain, BaseWndContextData datacontext, string strURL, string strBody, bool bSilenceMode = false, DownloadStatus? status = null, int nMaxRetry = 0);
        public void FindBookNextLinkAndContents(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? parent, ref HtmlNode? nextLink, ref HtmlNode? header, ref HtmlNode? content);
        public string GetBookHeader(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? header);
        public string GetBookNextLink(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? nextLink);
        public string GetBookContents(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? content);
        public string GetBookName(IBaseMainWindow wndMain, BaseWndContextData datacontext, HtmlNode? content);
    }
#pragma warning restore CS8632 // Null 参照代入の可能性があります。
}
