using HtmlAgilityPack;
using System;

namespace BaseBookDownload
{
    public interface IBookDownloader
    {

    }
    public interface IFetchNovelContent
    {
        public void AnalysisHtmlBookBody(IBaseMainWindow wndMain, BaseWndContextData datacontext, string strURL, string strBody, bool bSilenceMode = false, DownloadStatus? status = null, int nMaxRetry = 0);
        protected void FindBookNextLinkAndContents(HtmlNode? parent, ref HtmlNode nextLink, ref HtmlNode header, ref HtmlNode content);
        protected string GetBookHeader(HtmlNode? header);
        protected string GetBookNextLink(HtmlNode? nextLink);
        protected string GetBookContents(HtmlNode? content);
        public string GetBookName(HtmlNode? content);
    }
}
