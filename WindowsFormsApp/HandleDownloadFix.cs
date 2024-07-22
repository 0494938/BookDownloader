using BaseBookDownload;
using CefSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace WindowsFormsApp
{
#pragma warning disable CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
    public partial class WndContextData: BaseWndContextData
    {

    }

    partial class WindowsFormChrome: IBaseMainWindow
    {
        public void NovelTypeChangeEvent(BaseWndContextData datacontext, int nIndex)
        {
            if (txtInitURL != null)
            {
                Debug.WriteLine("Select Combox Index : " + cmbNovelType.SelectedIndex);
                txtInitURL.Text = datacontext.GetDefaultUrlByIdx(cmbNovelType.SelectedIndex);
            }
        }
#if false
        public void AnalysisHtmlBodyThreadFunc(BaseWndContextData datacontext, IBaseMainWindow wndMain, string strURL, string strBody, bool bSilenceMode = false, DownloadStatus? status = null)
        {
            Debug.Assert(!bSilenceMode || (bSilenceMode && status != null));

            int nMaxRetry = 60; //span is 3s.
            IFetchNovelContent? fetchNovelContent = datacontext.GetImplByNoveTypeIdx(datacontext.SiteType, ref nMaxRetry);

            if (bSilenceMode)
            {
                while (status?.DownloadFinished == false && !datacontext.UnloadPgm)
                {
                    Thread.Sleep(200);
                }
                UpdateStatusMsg(datacontext, strURL + " : Begin to Analysize downloaded Contents Body using " + datacontext.SiteType.ToCode() + "<" + fetchNovelContent?.GetType()?.Name + "> ...", (int)((100.0 / DownloadStatus.MaxPageToDownload * (status?.PageNum ?? 1 - 1 + 0.5))));
            }
            else
            {
                while (datacontext.PageLoaded == false && !datacontext.UnloadPgm)
                {
                    Thread.Sleep(200);
                }
                UpdateStatusMsg(datacontext, strURL + " : Begin to Analysize downloaded Contents Body using " + datacontext.SiteType.ToCode() + "<" + fetchNovelContent?.GetType()?.Name + "> ...", 50);
            }

            bool bNoNeedFresh = true;
            if (fetchNovelContent != null)
            {
                bNoNeedFresh  = fetchNovelContent.AnalysisHtmlBookBody(this, datacontext, strURL, strBody, bSilenceMode, status, nMaxRetry);
            }
            if (bSilenceMode)
            {
                UpdateStatusMsg(datacontext, strURL + " : Finished Analysing of downloaded Uri Contents Body of No." + status?.PageNum ?? 1 + " ...", (int)((100.0 / DownloadStatus.MaxPageToDownload * (status?.PageNum ?? 1))));
            }
            else
                UpdateStatusMsg(datacontext, strURL + " : Finished Analysing of downloaded Uri Contents Body of No. " + status?.PageNum ?? 1 + "...", 100);

            if (bSilenceMode)
            {
                if (!bNoNeedFresh )
                {
                    if(datacontext.RefreshCount > 0)
                    {
                        Debug.Assert(true);
                        datacontext.RefreshCount--;
                        if (datacontext.RefreshCount == 0)
                        {
                            UpdateStatusMsg(datacontext, "Failed as over access reqencey by website, After Retry Max to 60 seconds ...", 100 / 20 * datacontext.RefreshCount);
                            return;
                        }
                        UpdateStatusMsg(datacontext, "Failed as over access reqencey by website, Retry No." + datacontext.RefreshCount + " to 60 seconds ...", 100 / datacontext.MAX_REFRESH_CNT * datacontext.RefreshCount);
                        Thread.Sleep(datacontext.SLEEP_BETWEEN_RFRESH_MILI_SECONDS);
                        if (status != null)
                            status.DownloadFinished = false;
                        //wndMain.RefreshPage();
                        DownloadOneURLAndGetNext(datacontext, wndMain,  strURL, true);
                        return;
                    }
                    else
                    {
                        datacontext.RefreshCount = datacontext.MAX_REFRESH_CNT;
                        Debug.Assert(true);
                        UpdateStatusMsg(datacontext, "Failed as over access reqencey by website, Retry No." + datacontext.RefreshCount + " to 60 seconds ...", 100 / datacontext.MAX_REFRESH_CNT * datacontext.RefreshCount);
                        Thread.Sleep(datacontext.SLEEP_BETWEEN_RFRESH_MILI_SECONDS);
                        if (status != null)
                            status.DownloadFinished = false;
                        //wndMain.RefreshPage();
                        DownloadOneURLAndGetNext(datacontext, wndMain, strURL, true);
                    }
                }
                else
                {
                    datacontext.RefreshCount = 0;
                }
                if (status?.PageNum < DownloadStatus.MaxPageToDownload && !string.IsNullOrEmpty(status.NextUrl))
                {
                    DownloadOneURLAndGetNext(datacontext, wndMain, status.NextUrl,false);
                }
                else
                {
                    string sDownloadFileName = ((FileStream)DownloadStatus.ContentsWriter.BaseStream).Name;
                    DownloadStatus.ContentsWriter = null;

                    const Int32 BufferSize = 2048;
                    using (FileStream fileStream = File.OpenRead(sDownloadFileName))
                    using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
                    {
                        UpdateStatusMsg(datacontext, "Analysize of Chapter in download file : " + sDownloadFileName, -1);
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
                                    UpdateStatusMsg(datacontext, lLine.ToString() + " : " + sLine.Trim(), -1);
                            }
                        }
                    }

                    datacontext.BackGroundNotRunning = true;

                    UpdateStatusMsg(datacontext, "Finished batch download(Total " + status?.PageNum + " Downloaded) ...", 100);
                    //wndMain.btnInitURL.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                    wndMain.UpdateInitPageButton();
                    //wndMain.btnAutoDownload.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                    wndMain.UpdateAutoDownloadPageButton();

                    UpdateStatusMsg(datacontext, "Flush Log to file: " + sDownloadFileName + ".log", -1);
                    if (!string.IsNullOrEmpty(status?.NextUrl))
                        wndMain.UpdateInitUrl(status.NextUrl);

                    string strMsgAreaLog = txtLog.Text;

                    using (FileStream fileStream = File.OpenWrite(sDownloadFileName.Replace(".txt", "_log.log")))
                    using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8, BufferSize))
                    {
                        streamWriter.WriteLine(strMsgAreaLog);
                    }
                    //MessageBox.Show(this, "Batch download finished...", "Web Novel Downloader", MessageBoxButtons.OK);
                }
            }
        }
#endif
    }
#pragma warning restore CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
}
