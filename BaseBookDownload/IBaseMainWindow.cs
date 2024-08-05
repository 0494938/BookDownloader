namespace BaseBookDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
#pragma warning disable CS8632 // Null 参照代入の可能性があります。
    public interface IBaseMainWindow
    {
        public void UpdateStatusMsg(BaseWndContextData? datacontext, string msg, int value);
        public void UpdateChapterMsg(BaseWndContextData? datacontext, string msg, int value);
        public void UpdateStatusProgress(BaseWndContextData? datacontext, int value);
        public string? GetWebDocHtmlSource(string strUrl, bool bWaitOptoin = true, BaseWndContextData? datacontext=null);
        public string GetLogContents(BaseWndContextData? datacontext = null);
        public void RefreshPage(BaseWndContextData? datacontext = null);
        public void LoadHtmlString(string strHtml, string url, BaseWndContextData? datacontext = null);
        public void LoadUiUrl(BaseWndContextData? datacontext, string strURL);
        public void Back(BaseWndContextData? datacontext);

        public void UpdateNextPageButton(BaseWndContextData? datacontext = null);
        public void UpdateInitPageButton(BaseWndContextData? datacontext = null);
        public void UpdateAutoDownloadPageButton(BaseWndContextData? datacontext = null);
        public void UpdateAnalysisPageButton(BaseWndContextData? datacontext = null);

        public void UpdateInitUrl(string url, BaseWndContextData? datacontext = null);
        public void UpdateNextUrl(string url, BaseWndContextData? datacontext = null);
        public void UpdateCurUrl(string url, BaseWndContextData? datacontext = null);
        public void UpdateNovelName(string sNovelName, BaseWndContextData? datacontext = null);

        public void UpdateWebBodyOuterHtml(string? strBody, BaseWndContextData? datacontext = null);
        public void UpdateAnalysizedContents(string strContents, BaseWndContextData? datacontext = null);
        public void UpdateAggragatedContents(string strContents, BaseWndContextData? datacontext = null);
        public void UpdateAggragatedContentsWithLimit(string strContents, BaseWndContextData? datacontext = null);
        public bool isWebBrowserEmpty(BaseWndContextData? datacontext = null);
        public bool isWebPageLoadComplete(string strURL, BaseWndContextData? datacontext = null);
        public void NovelTypeChangeEvent(BaseWndContextData? datacontext, int nIndex);
        public string BatchDownloadNotified(BaseWndContextData? datacontext, DownloadStatus status, string sDownloadFileName);  //return log area contents
        public string GetNovelName(BaseWndContextData? datacontext = null);
        public bool DownloadFile(BaseWndContextData? datacontext, string sDownloadURL, bool bForceDownload = false);  //return log area contents
        public bool DownloadFile(BaseWndContextData? datacontext, System.Collections.Generic.Dictionary<string, string> dictUrls, bool bForceDownload = false);  //return log area contents
    }
#pragma warning restore CS8632 // Null 参照代入の可能性があります。
}