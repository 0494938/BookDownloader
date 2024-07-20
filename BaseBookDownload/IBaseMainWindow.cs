namespace BaseBookDownload
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public interface IBaseMainWindow
    {
        public void UpdateStatusMsg(BaseWndContextData datacontext, string msg, int value);
        public void UpdateStatusProgress(BaseWndContextData datacontext, int value);

        //public Dispatcher GetDispatcher();
        public string? GetWebDocHtmlBody(string strUrl, bool bWaitOptoin = true);
        
        public void UpdateNextPageButton();
        public void UpdateInitPageButton();
        public void UpdateAutoDownloadPageButton();

        public void UpdateNextUrl(string url);
        public void UpdateCurUrl(string url);

        public void UpdateWebBodyOuterHtml(string? strBody);
        public void UpdateAnalysizedContents(string strContents);
        public void UpdateAggragatedContents(string strContents);
        public void UpdateAggragatedContentsWithLimit(string strContents);
        
        //public void ParseResultToUI(IBaseMainWindow wndMain, bool bSilenceMode, string strContents, string strNextLink);
        //public void ParseResultToUI(bool bSilenceMode, string strContents, string strNextLink);

    }
}