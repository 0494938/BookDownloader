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
        //Dictionary<string, DownloadStatus> dictDownloadStatus = new Dictionary<string, DownloadStatus>();

        private void AnalysisURL(string strUrl, bool bWaitOptoin = true)
        {
            Debug.WriteLine("btnAnalysisCurURL_Click invoked...");
            string? strBody = GetWebDocHtmlBody(strUrl, bWaitOptoin);
            
            if (!string.IsNullOrEmpty(strBody.Trim()))
            {
               datacontext.AnalysisHtmlBody(this,bWaitOptoin, strUrl, strBody);
            }
        }

        class _DocContents
        {
            public string sHtml="";
        }

        public string? GetWebDocHtmlBody(string strUrl, bool bWaitOptoin = true)
        {
            _DocContents doc = new _DocContents() ;
            string ?strBody = null;
            GetWebDocHtml(doc);
            this.Invoke(() => { strBody = txtHtml.Text; });
            while (string.IsNullOrEmpty(doc.sHtml))
            {
                Thread.Sleep(200);
                GetWebDocHtml(doc);
                this.Invoke(() => { 
                    strBody = doc.sHtml;
                    if (strBody.Length > 0 && strBody.Length < 200)
                        Debug.Assert(false);
                });
            }
            return strBody;
        }
        //string strTmp = "";
        private void GetWebDocHtml(_DocContents doc)
        {
            if (browser == null || browser.IsLoading == true)
                return ;

            browser.GetSourceAsync().ContinueWith(taskHtml =>
            {
                this.Invoke(() =>
                {
                    doc.sHtml = taskHtml.Result;
                });
            });
        }
#if false
        public void AnalysisHtmlBody(BaseWndContextData? datacontext, bool bWaitOption, string strURL, string strBody, bool bSilenceMode = false, DownloadStatus? status = null)
        {
            Debug.Assert(datacontext != null);
            if (!string.IsNullOrEmpty(strBody))
            {
                try
                {
                    Thread thread = new Thread(() => AnalysisHtmlBodyThreadFunc(datacontext, this, strURL, strBody, bSilenceMode, status));
                    thread.Start();
                }
                catch (TaskCanceledException)
                {
                    //ignore TaskCanceledException
                }
            }
            else
            {
                Debug.Assert(true);//got a empty body.

            }
            //AnalysisHtmlBodyThreadFunc(datacontext, this, strURL, strBody, bSilenceMode, status);
        }


        private void DownloadOneURLAndGetNext(BaseWndContextData? datacontext, IBaseMainWindow wndMain, string strURL, bool bRefresh)
        {
            if ((datacontext != null))
            {
                try
                {
                    if (bRefresh) {
                        DownloadStatus status = dictDownloadStatus[strURL];
                        status.DownloadFinished = false;
                        Debug.Assert(status.URL == strURL);
                        status.NextUrl="";
                    }
                    else
                        dictDownloadStatus[strURL] = new DownloadStatus { DownloadFinished = false, URL = strURL, NextUrl = "", StartTime = DateTime.Now, PageNum = dictDownloadStatus.Count + 1 };

                    datacontext.PageLoaded = false;
                    datacontext.NextLinkAnalysized = false;
                    wndMain.UpdateAnalysisPageButton();
                    wndMain.UpdateNextPageButton();
                    browser.LoadUrl(strURL);

                    UpdateStatusMsg(datacontext, strURL + " : Begin to download ...", (int)((100.0 / DownloadStatus.MaxPageToDownload * (dictDownloadStatus[strURL].PageNum - 1))));
                    WaitFinishForNext(datacontext, wndMain, strURL, true);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    if (browser== null || browser.IsLoading == true)
                    {
                        Debug.Assert(false);
                        WaitFinishForNext(datacontext, wndMain, strURL, true);
                    }

                    Debug.WriteLine(strURL + " : Loading Status <" + (browser.IsLoading?"Loading":"Loaded") + ">");
                    if (browser.IsLoading == true)
                        return;
                }
            }
        }

        void WaitFinishForNext(BaseWndContextData datacontext, IBaseMainWindow wndMain, string strURL, bool bSilenceMode = false)
        {
            DownloadStatus status = dictDownloadStatus[strURL];

            Thread thread = new Thread(() => WaitAndLaunchAnalsysi(datacontext, wndMain, strURL, bSilenceMode, status));
            thread.Start();
        }

        public void WaitAndLaunchAnalsysi(BaseWndContextData datacontext, IBaseMainWindow wndMain, string strURL, bool bSilenceMode, DownloadStatus status)
        {
            string? strBody = null;
            if (status != null)
            {
                const int MAX_RETRY = 60 * 5 * 2; //wait loading up to 2 minutes.
                int nWaitRetry = 0;
                while (status.DownloadFinished == false && !datacontext.UnloadPgm && nWaitRetry < MAX_RETRY)
                {
                    Thread.Sleep(200);
                    nWaitRetry++;
                }
            }

            strBody = GetWebDocHtmlBody(strURL, true);

            Debug.WriteLine($"{strURL} : Download Finished, Begin Analysis ...");
            Debug.Assert(browser != null || browser.IsLoading!=true);

            wndMain.UpdateWebBodyOuterHtml(strBody);
            AnalysisHtmlBody(datacontext, true, strURL, strBody, bSilenceMode, status);

        }
#endif
        private void btnBrowser_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtInitURL.Text.Trim()))
            {
                datacontext.RefreshCount = 0;
                txtHtml.Clear();
                txtContent.Clear();
                txtLog.Clear();
                datacontext.SiteType = (BatchQueryNovelContents)cmbNovelType.SelectedIndex;
                datacontext.PgmNaviUrl = txtInitURL.Text.Trim();
                browser.LoadUrl(txtInitURL.Text.Trim());
            }
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNextUrl.Text.Trim()))
            {
                datacontext.RefreshCount = 0;
                txtHtml.Clear();
                txtContent.Clear();
                txtLog.Clear();
                datacontext.SiteType = (BatchQueryNovelContents)cmbNovelType.SelectedIndex;
                datacontext.PgmNaviUrl = txtNextUrl.Text.Trim();
                browser.LoadUrl(txtNextUrl.Text.Trim());
            }

        }

        private void btnAutoDownload_Click(object sender, EventArgs e)
        {
            txtHtml.Clear();
            txtContent.Clear();
            txtLog.Clear();
            datacontext.RefreshCount = 0;

            datacontext.DictDownloadStatus.Clear();
            Debug.WriteLine("btnAutoDownload_Click invoked...");

            if (datacontext != null)
            {
                datacontext.BackGroundNotRunning = false;
                datacontext.SiteType = (BatchQueryNovelContents)cmbNovelType.SelectedIndex;
                datacontext.DictDownloadStatus.Clear();

                int nMaxPage = string.IsNullOrEmpty(txtPages.Text.Trim()) ? 20 : int.Parse(txtPages.Text.Trim());
                DownloadStatus.MaxPageToDownload = nMaxPage;

                DownloadStatus.ContentsWriter = new StreamWriter(
                    File.Open(AppDomain.CurrentDomain.BaseDirectory + "DumpNovel" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss") + ".txt",
                    FileMode.CreateNew,
                    FileAccess.ReadWrite,
                    FileShare.Read),
                    Encoding.UTF8
                );

                datacontext.DownloadOneURLAndGetNext(this, txtInitURL.Text, false);
            }
        }

        public void FreshPage()
        {
            this.Invoke(() => {
                UpdateStatusMsg(datacontext, "### Refresh " + browser.Address + " ...", 0);
                //DownloadStatus.MaxPageToDownload++;
                browser.Refresh();
            });
        }
    }
#pragma warning restore CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
}
