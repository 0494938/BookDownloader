using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BaseBookDownloader
{
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning disable CS8604 // Null 参照代入の可能性があります。
#pragma warning disable CS8632 // Null 参照代入の可能性があります。

    public partial class BaseWndContextData : INotifyPropertyChanged
    {
        #region Functions
        public void DownloadOneURLAndGetNext(IBaseMainWindow wndMain, string strURL, bool bRefresh)
        {
            if (!UnloadPgm)
            {
                try
                {
                    if (bRefresh)
                    {
                        DownloadStatus status = DictDownloadStatus[strURL];
                        status.DownloadFinished = false;
                        Debug.Assert(status.URL == strURL);
                        status.NextUrl = "";
                    }
                    else
                        DictDownloadStatus[strURL] = new DownloadStatus { DownloadFinished = false, URL = strURL, NextUrl = "", StartTime = DateTime.Now, PageNum = DictDownloadStatus.Count + 1 };

                    wndMain.LoadUiUrl(this, strURL);
                    wndMain.UpdateStatusMsg(this, strURL + " : Begin to download ...", (int)((100.0 / DownloadStatus.MaxPageToDownload * (DictDownloadStatus[strURL].PageNum - 1))));
                    WaitFinishForNext(wndMain, strURL, true);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    if (wndMain.isWebBrowserEmpty())
                    {
                        Debug.Assert(false);
                        WaitFinishForNext(wndMain, strURL, true);
                    }

                    if (!wndMain.isWebPageLoadComplete(strURL))
                        return ;
                }
            }
            else
            {
                Debug.Assert(true);
            }
        }

        void WaitFinishForNext(IBaseMainWindow wndMain, string strURL, bool bSilenceMode = false)
        {
            DownloadStatus status = DictDownloadStatus[strURL];

            try
            {
                new Thread(() => WaitAndLaunchAnalsys(wndMain, strURL, bSilenceMode, status)).Start();
            }
            catch (TaskCanceledException)
            {
                //ignore TaskCanceledException
            }
        }

        public void WaitAndLaunchAnalsys(IBaseMainWindow wndMain, string strURL, bool bSilenceMode, DownloadStatus status)
        {
            const int MAX_RETRY = 60 * 5 * 2; //wait loading up to 2 minutes.
            int nWaitRetry = 0;

            if (bSilenceMode)
            {
                while (status.DownloadFinished == false && !this.UnloadPgm && nWaitRetry < MAX_RETRY)
                {
                    Thread.Sleep(200);
                    nWaitRetry++;
                }
            }
            else
            {
                while (this.PageLoaded == false && !this.UnloadPgm && nWaitRetry < MAX_RETRY)
                {
                    Thread.Sleep(200);
                    nWaitRetry++;
                }
            }

            Debug.WriteLine($"{strURL} : Download Finished, Begin Analysis ...");

            if (!UnloadPgm)
            {
                try
                {
                    string? strBody = wndMain.GetWebDocHtmlSource(strURL);
                    wndMain.UpdateWebBodyOuterHtml(strBody);
                    if (!string.IsNullOrEmpty(strBody))
                    {
                        AnalysisHtml4Nolvel(wndMain, true, strURL, strBody, bSilenceMode, status);
                    }
                    else if(bSilenceMode && status.DownloadRetried == 0)
                    {
                        status.DownloadRetried = status.DownloadRetried + 1;
                        wndMain.LoadHtmlString("<html><body></body></html> ", "blank");
                        wndMain.UpdateStatusMsg(this, "Faield to Get Document Contents of <" + status.URL + ">, Force Reload to Retry",-1);
                        Thread.Sleep (200);
                        status.DownloadFinished = false;
                        //wndMain.LoadUiUrl(this, status.URL);
                        wndMain.Back(this);
                        //strBody = wndMain.GetWebDocHtmlBody(strURL);
                        //if (!string.IsNullOrEmpty(strBody))
                        //{
                        //    AnalysisHtmlBody(wndMain, true, strURL, strBody, bSilenceMode, status);
                        //}
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.StackTrace);
                }
            }
        }

        public void AnalysisHtml4Nolvel(IBaseMainWindow wndMain, bool bWaitOption, string strURL, string strHtml, bool bSilenceMode = false, DownloadStatus? status = null)
        {
            try
            {
                new Thread(() => AnalysisHtmlThreadFunc4Novel(wndMain, strURL, strHtml, bSilenceMode, status)).Start();
            }
            catch (TaskCanceledException)
            {
                //ignore TaskCanceledException
            }
            //AnalysisHtmlBodyThreadFunc(datacontext, this, strURL, strBody, bSilenceMode, status);
        }

        public void AnalysisHtmlThreadFunc4Novel(IBaseMainWindow wndMain, string strURL, string strHtml, bool bSilenceMode = false, DownloadStatus? status = null)
        {
            Debug.Assert(!bSilenceMode || (bSilenceMode && status != null));

            int nMaxRetry = 60; //span is 3s.
            IFetchNovelContent? fetchNovelContent = GetImplByNoveTypeIdx(SiteType, ref nMaxRetry);


            if (bSilenceMode)
            {
                while (status?.DownloadFinished == false && !UnloadPgm)
                {
                    Thread.Sleep(200);
                }
                wndMain.UpdateStatusMsg(this, strURL + " : Begin to Analysize downloaded Contents Body using " + SiteType.ToCode() + "<" + fetchNovelContent?.GetType()?.Name + "> ...", (int)((100.0 / DownloadStatus.MaxPageToDownload * (status?.PageNum ?? 1 - 1 + 0.5))));
            }
            else
            {
                while (this.PageLoaded == false && !this.UnloadPgm)
                {
                    Thread.Sleep(200);
                }
                wndMain.UpdateStatusMsg(this, strURL + " : Begin to Analysize downloaded Contents Body using " + this.SiteType.ToCode() + "<" + fetchNovelContent?.GetType()?.Name + "> ...", 50);
            }

            bool bNoNeedFresh = true;
            if (fetchNovelContent != null)
            {
                bNoNeedFresh = fetchNovelContent.AnalysisHtmlBook(wndMain, this, strURL, strHtml, bSilenceMode, status, nMaxRetry);
            }
            if (bSilenceMode)
            {
                wndMain.UpdateStatusMsg(this, strURL + " : Finished Analysing of downloaded Uri Contents Body, No." + (status?.PageNum ?? 1) + " / " + DownloadStatus.MaxPageToDownload + " ...", (int)((100.0 / DownloadStatus.MaxPageToDownload * (status?.PageNum ?? 1))));
            }
            else
                wndMain.UpdateStatusMsg(this, strURL + " : Finished Analysing of downloaded Uri Contents Body, No. " + (status?.PageNum ?? 1) + " / 1 ...", 100);

            if (bSilenceMode)
            {
                if (!bNoNeedFresh)
                {
                    if (this.RefreshCount > 0)
                    {
                        Debug.Assert(true);
                        this.RefreshCount--;
                        if (this.RefreshCount == 0)
                        {
                            wndMain.UpdateStatusMsg(this, "Failed as over access reqencey by website, After Retry Max to 60 seconds ...", 100 / 20 * this.RefreshCount);
                            return;
                        }
                        wndMain.UpdateStatusMsg(this, "Failed as over access reqencey by website, Retry No." + this.RefreshCount + " to 60 seconds ...", 100 / this.MAX_REFRESH_CNT * this.RefreshCount);
                        Thread.Sleep(this.SLEEP_BETWEEN_RFRESH_MILI_SECONDS);
                        if (status != null)
                            status.DownloadFinished = false;
                        //wndMain.RefreshPage();
                        DownloadOneURLAndGetNext(wndMain, strURL, true);
                        return;
                    }
                    else
                    {
                        this.RefreshCount = this.MAX_REFRESH_CNT;
                        Debug.Assert(true);
                        wndMain.UpdateStatusMsg(this, "Failed as over access reqencey by website, Retry No." + this.RefreshCount + " to 60 seconds ...", 100 / this.MAX_REFRESH_CNT * this.RefreshCount);
                        Thread.Sleep(this.SLEEP_BETWEEN_RFRESH_MILI_SECONDS);
                        if (status != null)
                            status.DownloadFinished = false;
                        //wndMain.RefreshPage();
                        DownloadOneURLAndGetNext(wndMain, strURL, true);
                    }
                }
                else
                {
                    this.RefreshCount = 0;
                }

                if (status?.PageNum < DownloadStatus.MaxPageToDownload && !string.IsNullOrEmpty(status.NextUrl))
                {
                    DownloadOneURLAndGetNext(wndMain, status.NextUrl, false);
                }
                else
                {
                    string sDownloadFileName = ((FileStream)DownloadStatus.ContentsWriter.BaseStream).Name;
                    DownloadStatus.ContentsWriter = null;

                    string sName = Path.GetFileName(sDownloadFileName);
                    string sPath = Path.GetDirectoryName(sDownloadFileName).ToString();
                    if (sName.StartsWith("Novel", StringComparison.CurrentCultureIgnoreCase) && !string.IsNullOrEmpty(wndMain.GetNovelName()))
                    {
                        sName = wndMain.GetNovelName() + sName.Substring("Novel".Length);
                        File.Move(sDownloadFileName, sPath + Path.DirectorySeparatorChar+ sName);
                        sDownloadFileName = sPath + Path.DirectorySeparatorChar + sName;
                    }

                    const Int32 BufferSize = 2048;
                    using (FileStream fileStream = File.OpenRead(sDownloadFileName))
                    using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
                    {
                        wndMain.UpdateStatusMsg(this, "Analysize of Chapter in download file : " + sDownloadFileName, -1);
                        String? sLine;
                        long lLine = 0;
                        while ((sLine = streamReader.ReadLine()) != null)
                        {
                            lLine++;
                            if (!string.IsNullOrEmpty(sLine.Trim()))
                            {
                                //Match matche = Regex.Match(line, "[^第]*(第[]*[][^章节页]*");// Process line
                                string sPattern = "第[0-9０-９ \t一二三四五六七八九十百千万亿壹贰叁肆伍陆柒捌玖拾佰仟零]+[章节页]";
                                bool result = Regex.IsMatch(sLine, sPattern);
                                if (result)
                                    wndMain.UpdateStatusMsg(this, lLine.ToString() + " : " + sLine.Trim(), -1);
                                //Match matche = Regex.Match(line, sPattern);// Process line
                            }
                        }
                    }

                    this.BackGroundNotRunning = true;

                    wndMain.UpdateStatusMsg(this, "Finished batch download(Total " + status?.PageNum + " Downloaded) ...", 100);
                    String strMsgAreaLog = wndMain.BatchDownloadNotified(this, status, sDownloadFileName);
                    using (FileStream fileStream = File.OpenWrite(sDownloadFileName.Replace(".txt", "_log.log")))
                    using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8, BufferSize))
                    {
                        streamWriter.WriteLine(strMsgAreaLog);
                    }
                }
            }
        }
        public void AnalysisHtmlThreadFunc4YouTube(IBaseMainWindow wndMain, string strURL, string strHtml, bool bSilenceMode = false)
        {
            int nMaxRetry = 60; //span is 3s.
            IFetchNovelContent? fetchNovelContent = GetImplByNoveTypeIdx(SiteType, ref nMaxRetry);
            while (this.PageLoaded == false && !UnloadPgm)
            {
                Thread.Sleep(200);
            }
            wndMain.UpdateStatusMsg(this, strURL + " : Begin to Analysize downloaded Contents Body using " + SiteType.ToCode() + "<" + fetchNovelContent?.GetType()?.Name + "> ...", 50);

            bool bNoNeedFresh = true;
            if (fetchNovelContent != null)
            {
                bNoNeedFresh = fetchNovelContent.AnalysisHtmlStream(wndMain, this, strURL, strHtml, bSilenceMode, null, nMaxRetry);
            }
        }

        public void AnalysisHtmlThreadFunc4PornHub(IBaseMainWindow wndMain, string strURL, string strHtml, bool bSilenceMode = false)
        {
            int nMaxRetry = 60; //span is 3s.
            IFetchNovelContent? fetchNovelContent = GetImplByNoveTypeIdx(SiteType, ref nMaxRetry);
            while (this.PageLoaded == false && !UnloadPgm)
            {
                Thread.Sleep(200);
            }
            wndMain.UpdateStatusMsg(this, strURL + " : Begin to Analysize downloaded Contents Body using " + SiteType.ToCode() + "<" + fetchNovelContent?.GetType()?.Name + "> ...", 50);

            bool bNoNeedFresh = true;
            if (fetchNovelContent != null)
            {
                bNoNeedFresh = fetchNovelContent.AnalysisHtmlStream(wndMain, this, strURL, strHtml, bSilenceMode, null, nMaxRetry, bSilenceMode);
            }
        }

        public void AnalysisHtmlThreadFunc4RedPorn(IBaseMainWindow wndMain, string strURL, string strHtml, bool bSilenceMode = false)
        {
            int nMaxRetry = 60; //span is 3s.
            IFetchNovelContent? fetchNovelContent = GetImplByNoveTypeIdx(SiteType, ref nMaxRetry);
            while (this.PageLoaded == false && !UnloadPgm)
            {
                Thread.Sleep(200);
            }
            wndMain.UpdateStatusMsg(this, strURL + " : Begin to Analysize downloaded Contents Body using " + SiteType.ToCode() + "<" + fetchNovelContent?.GetType()?.Name + "> ...", 50);

            bool bNoNeedFresh = true;
            if (fetchNovelContent != null)
            {
                bNoNeedFresh = fetchNovelContent.AnalysisHtmlStream(wndMain, this, strURL, strHtml, bSilenceMode, null, nMaxRetry, bSilenceMode);
            }
        }
        #endregion Functions
    }


#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning restore CS8604 // Null 参照代入の可能性があります。
#pragma warning restore CS8632 // Null 参照代入の可能性があります。
}