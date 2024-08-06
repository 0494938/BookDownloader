using BaseBookDownloader;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace BookDownloadFormApp
{
#pragma warning disable CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
    public partial class WndContextData : BaseWndContextData
    {

    }

    partial class WindowsFormWebView2 : IBaseMainWindow
    {
        private void AnalysisURL(string strUrl, bool bWaitOptoin = true)
        {
            Debug.WriteLine("btnAnalysisCurURL_Click invoked...");
            string? strBody = GetWebDocHtmlSource(strUrl, bWaitOptoin);
            
            if (!string.IsNullOrEmpty(strBody.Trim()))
            {
               datacontext.AnalysisHtml4Nolvel(this,bWaitOptoin, strUrl, strBody);
            }
        }

        class _DocContents
        {
            public string sHtml="";
        }

        public string? GetWebDocHtmlSource(string strUrl, bool bWaitOptoin = true, BaseWndContextData? datacontext = null)
        {
            _DocContents doc = new _DocContents() ;
            string html = "";
            this.Invoke(() =>
            {
                if (webBrowser != null && webBrowser.CoreWebView2 != null /*&& webBrowser.CoreWebView2.load == true*/)
                {
                    webBrowser.ExecuteScriptAsync("document.documentElement.outerHTML;").ContinueWith(taskHtml =>
                    {
                        doc.sHtml = taskHtml.Result;
                    });
                }
            });
            int nMaxRetry = 5 * 20;
            int nRetry = 0;
            while (nRetry < nMaxRetry && string.IsNullOrEmpty(doc.sHtml))
            {
                nRetry++;
                Thread.Sleep(200);
            }
            html = doc.sHtml;
            html = Regex.Unescape(html);
            html = html.Remove(0, 1);
            html = html.Remove(html.Length - 1, 1);
            return html;
        }


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
                webBrowser.CoreWebView2.Navigate(txtInitURL.Text.Trim());
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
                webBrowser.CoreWebView2.Navigate(txtNextUrl.Text.Trim());
            }
        }

        private void btnAutoDownload_Click(object sender, EventArgs e)
        {
            txtHtml.Clear();
            txtContent.Clear();
            txtLog.Clear();
            datacontext.RefreshCount = 0;

            Debug.WriteLine("btnAutoDownload_Click invoked...");

            if (datacontext != null)
            {
                datacontext.BackGroundNotRunning = false;
                datacontext.SiteType = (BatchQueryNovelContents)cmbNovelType.SelectedIndex;
                datacontext.DictDownloadStatus.Clear();
                datacontext.DictHandledContentsForDupCheck.Clear();

                int nMaxPage = string.IsNullOrEmpty(txtPages.Text.Trim()) ? 20 : int.Parse(txtPages.Text.Trim());
                DownloadStatus.MaxPageToDownload = nMaxPage;

                DownloadStatus.ContentsWriter = new StreamWriter(
                    File.Open(datacontext.FileSavePath + "DumpNovel" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss") + ".txt",
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
                UpdateStatusMsg(datacontext, "### Refresh " + webBrowser.Source.ToString() + " ...", 0);
                //DownloadStatus.MaxPageToDownload++;
                webBrowser.Refresh();
            });
        }
    }
#pragma warning restore CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
}
