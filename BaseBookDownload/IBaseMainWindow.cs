namespace BaseBookDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
#pragma warning disable CS8632 // Null 参照代入の可能性があります。
    public interface IBaseMainWindow
    {
        public void UpdateStatusMsg(BaseWndContextData datacontext, string msg, int value);
        public void UpdateStatusProgress(BaseWndContextData datacontext, int value);
        public string? GetWebDocHtmlBody(string strUrl, bool bWaitOptoin = true);
        public string GetLogContents();
        public void RefreshPage();
        public void LoadUiUrl(BaseWndContextData datacontext, string strURL);

        public void UpdateNextPageButton();
        public void UpdateInitPageButton();
        public void UpdateAutoDownloadPageButton();
        public void UpdateAnalysisPageButton();

        public void UpdateInitUrl(string url);
        public void UpdateNextUrl(string url);
        public void UpdateCurUrl(string url);

        public void UpdateWebBodyOuterHtml(string? strBody);
        public void UpdateAnalysizedContents(string strContents);
        public void UpdateAggragatedContents(string strContents);
        public void UpdateAggragatedContentsWithLimit(string strContents);
        public bool isWebBrowserEmpty();
        public bool isWebPageLoadComplete(string strURL);
        public void NovelTypeChangeEvent(BaseWndContextData datacontext, int nIndex);
        public string BatchDownloadNotified(BaseWndContextData datacontext, DownloadStatus status, string sDownloadFileName);  //return log area contents
    }
#pragma warning restore CS8632 // Null 参照代入の可能性があります。
}