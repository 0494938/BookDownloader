namespace BaseBookDownload
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
#pragma warning disable CS8632 // Null 参照代入の可能性があります。
    public interface IBaseMainWindow
    {
        public void UpdateStatusMsg(BaseWndContextData datacontext, string msg, int value);
        public void UpdateStatusProgress(BaseWndContextData datacontext, int value);

        //public Dispatcher GetDispatcher();
        public string? GetWebDocHtmlBody(string strUrl, bool bWaitOptoin = true);
        public string GetLogContents();
        public void RefreshPage();

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

        public void NovelTypeChangeEvent(BaseWndContextData datacontext, int nIndex);
        public void AnalysisHtmlBodyThreadFunc(BaseWndContextData datacontext, IBaseMainWindow wndMain, string strURL, string strBody, bool bSilenceMode = false, DownloadStatus? status = null);

        //public void ParseResultToUI(IBaseMainWindow wndMain, bool bSilenceMode, string strContents, string strNextLink);
        //public void ParseResultToUI(bool bSilenceMode, string strContents, string strNextLink);

    }
#pragma warning restore CS8632 // Null 参照代入の可能性があります。
}