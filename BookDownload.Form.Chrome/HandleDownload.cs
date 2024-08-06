using BaseBookDownloader;
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

namespace BookDownloadFormApp
{
#pragma warning disable CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
    public partial class WndContextData: BaseWndContextData
    {

    }

    partial class WindowsFormChrome: IBaseMainWindow
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
            string ?strBody = null;
            GetWebDocHtml(doc);
            this.Invoke(() => { strBody = doc.sHtml; });
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

        private void GetWebDocHtml(_DocContents doc)
        {
            if (webBrowser == null || webBrowser.IsLoading == true)
                return ;

            webBrowser.GetSourceAsync().ContinueWith(taskHtml =>
            {
                this.Invoke(() =>
                {
                    doc.sHtml = taskHtml.Result;
                });
            });
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
                webBrowser.LoadUrl(txtInitURL.Text.Trim());
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
                webBrowser.LoadUrl(txtNextUrl.Text.Trim());
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
                UpdateStatusMsg(datacontext, "### Refresh " + webBrowser.Address + " ...", 0);
                //DownloadStatus.MaxPageToDownload++;
                webBrowser.Refresh();
            });
        }

        public bool DownloadFile(BaseWndContextData? datacontext, string sDownloadURL, bool bForceDownload = false)
        {
            throw new NotImplementedException();
        }

        public bool DownloadFile(BaseWndContextData? datacontext, System.Collections.Generic.Dictionary<string, string> dictUrls, bool bForceDownload = false)
        {
            throw new NotImplementedException();
        }
    }
#pragma warning restore CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
}
