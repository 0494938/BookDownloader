using BaseBookDownload;
using CefSharp;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp
{
    public partial class WndContextData : BaseWndContextData
    {

    }

    partial class WindowsFormChrome : IBaseMainWindow
    {
        public void UpdateStatusMsg(BaseWndContextData datacontext, string msg, int value)
        {
            this.Invoke(() =>
            {
                txtStatus.Text = msg;
                txtLog.AppendText(msg.TrimEnd(new char[] { '\r', '\n', ' ', '\t' }) + "\r\n");
                if(value >=0) 
                    txtProgress.Value = value;
            });
            Debug.WriteLine(msg);
        }

        public void UpdateStatusProgress(BaseWndContextData datacontext, int value)
        {
            if(value >= 0)
            {
                this.Invoke(() =>
                {
                    txtProgress.Value = value;
                });

            }
        }

        public void UpdateNextPageButton()
        {
            //throw new NotImplementedException();
        }

        public void UpdateInitPageButton()
        {
            //throw new NotImplementedException();
        }
        public void UpdateAnalysisPageButton()
        {
            //throw new NotImplementedException();
        }

        public void UpdateAutoDownloadPageButton()
        {
            //throw new NotImplementedException();
        }

        public void UpdateNextUrl(string url)
        {
            this.Invoke(() => {
                txtNextUrl.Text = url;
            });
        }

        public void UpdateInitUrl(string url)
        {
            this.Invoke(() => {
                this.txtInitURL.Text = url;
            });
        }

        public void UpdateCurUrl(string url)
        {
            //throw new NotImplementedException();
        }

        public void UpdateWebBodyOuterHtml(string strBody)
        {
            this.Invoke(() => { 
                txtHtml.Text = strBody.Replace("\r","").Replace("\n","\r\n"); 
            });
        }

        public void UpdateAnalysizedContents(string strContents)
        {
            this.Invoke(() => {
                txtContent.Text = strContents.Replace("\r", "").Replace("\n", "\r\n");
            });
        }

        public void UpdateAggragatedContents(string strContents)
        {
            //throw new NotImplementedException();
        }

        public void UpdateAggragatedContentsWithLimit(string strContents)
        {
            //throw new NotImplementedException();
        }

        public string GetLogContents(){
            string strLog="";
            this.Invoke(() => {
                strLog= txtLog.Text.Replace("\r", "").Replace("\n", "\r\n");
            });
            return strLog;
        }
    }
}
