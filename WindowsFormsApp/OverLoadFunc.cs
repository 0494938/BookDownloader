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
            this.Invoke(new Action(() =>
            {
                txtLog.AppendText(msg.TrimEnd(new char[] { '\r', '\n', ' ', '\t' }) + "\r\n");
            }));
            //throw new NotImplementedException();
        }

        public void UpdateStatusProgress(BaseWndContextData datacontext, int value)
        {
            throw new NotImplementedException();
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
            //throw new NotImplementedException();
            this.txtNextUrl.Text = url;
        }

        public void UpdateCurUrl(string url)
        {
            throw new NotImplementedException();
        }

        public void UpdateWebBodyOuterHtml(string strBody)
        {
            throw new NotImplementedException();
        }

        public void UpdateAnalysizedContents(string strContents)
        {
            throw new NotImplementedException();
        }

        public void UpdateAggragatedContents(string strContents)
        {
            throw new NotImplementedException();
        }

        public void UpdateAggragatedContentsWithLimit(string strContents)
        {
            throw new NotImplementedException();
        }


    }
}
