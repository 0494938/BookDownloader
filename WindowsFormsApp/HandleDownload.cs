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
        Dictionary<string, DownloadStatus> dictDownloadStatus = new Dictionary<string, DownloadStatus>();

        private void AnalysisURL(string strUrl, bool bWaitOptoin = true)
        {
            Debug.WriteLine("btnAnalysisCurURL_Click invoked...");
            string? strBody = GetWebDocHtmlBody(strUrl, bWaitOptoin);
            
            if (!string.IsNullOrEmpty(strBody.Trim()))
            {
                //WndContextData datacontext = new WndContextData();
                AnalysisHtmlBody(datacontext, bWaitOptoin, strUrl, strBody);
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
                this.Invoke(() => { strBody = doc.sHtml; });
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

        public void AnalysisHtmlBody(BaseWndContextData? datacontext, bool bWaitOption, string strURL, string strBody, bool bSilenceMode = false, DownloadStatus? status = null)
        {
            Debug.Assert(datacontext != null);
            try
            {
                Thread thread = new Thread(() => AnalysisHtmlBodyThreadFunc(datacontext, this, strURL, strBody, bSilenceMode, status));
                thread.Start();
            }
            catch (TaskCanceledException)
            {
                //ignore TaskCanceledException
            }
            //AnalysisHtmlBodyThreadFunc(datacontext, this, strURL, strBody, bSilenceMode, status);
        }

        private void DownloadOneURLAndGetNext(BaseWndContextData? datacontext, IBaseMainWindow wndMain, string strURL)
        {
            if ((datacontext != null))
            {
                try
                {
                    dictDownloadStatus[strURL] = new DownloadStatus { DownloadFinished = false, URL = strURL, NextUrl = "", StartTime = DateTime.Now, Depth = 0, ThreadNum = dictDownloadStatus.Count + 1 };

                    datacontext.PageLoaded = false;
                    datacontext.NextLinkAnalysized = false;
                    //btnAnalysisCurURL.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                    wndMain.UpdateAnalysisPageButton();
                    //btnNextPage.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                    wndMain.UpdateNextPageButton();
                    browser.LoadUrl(strURL);

                    UpdateStatusMsg(datacontext, strURL + " : Begin to download ...", (int)((100.0 / DownloadStatus.ThreadMax * (dictDownloadStatus[strURL].ThreadNum - 1))));
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

        void WaitFinishForNext(BaseWndContextData? datacontext, IBaseMainWindow wndMain, string strURL, bool bSilenceMode = false)
        {
            DownloadStatus status = dictDownloadStatus[strURL];

            Thread thread = new Thread(() => WaitAndLaunchAnalsysi(datacontext, wndMain, strURL, bSilenceMode, status));
            thread.Start();
        }

        public void WaitAndLaunchAnalsysi(BaseWndContextData? datacontext, IBaseMainWindow wndMain, string strURL, bool bSilenceMode, DownloadStatus status)
        {
            string? strBody = null;
            if (status != null)
            {
                while (status.DownloadFinished == false)
                {
                    Thread.Sleep(200);
                }
            }

            strBody = GetWebDocHtmlBody(strURL, true);
            if (status != null)
                status.Depth = status.Depth - 1;

            Debug.WriteLine($"{strURL} : Download Finished, Begin Analysis ...");
            Debug.Assert(browser != null || browser.IsLoading!=true);

            wndMain.UpdateWebBodyOuterHtml(strBody);
            AnalysisHtmlBody(datacontext, true, strURL, strBody, bSilenceMode, status);

        }
    }
#pragma warning restore CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
}
