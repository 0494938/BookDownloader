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
        private void Browser_AddressChanged(object sender, AddressChangedEventArgs e)
        {
            UpdateStatusMsg(datacontext, "Browser_AddressChanged : " + e.Address + " ...", 0);
        }

        public void RefreshPage(BaseWndContextData? datacontext = null)
        {
            this.Invoke(() =>{
                webBrowser.LoadUrl(webBrowser.Address);
                //browser.Refresh();
            });
        }

        private void Browser_ControlAdded(object sender, ControlEventArgs e)
        {
            UpdateStatusMsg(datacontext, "Browser_ControlAdded : Name->" + e.Control.Name + ", AccessibleName=" + e.Control.AccessibleName + " ...", -1);
        }

        private void Browser_ControlRemoved(object sender, ControlEventArgs e)
        {
            UpdateStatusMsg(datacontext, "Browser_ControlRemoved : Name->" + e.Control.Name + ", AccessibleName=" + e.Control.AccessibleName + " ...", -1);
        }

        private void Browser_IsBrowserInitializedChanged(object sender, EventArgs e)
        {
            UpdateStatusMsg(datacontext, "Browser_IsBrowserInitializedChanged : " + e.ToString() + " ...", -1);
        }

        private void Browser_BindingContextChanged(object sender, EventArgs e)
        {
            //UpdateStatusMsg(datacontext, "Browser_BindingContextChanged : " + e.ToString() + " ...", -1);
            Debug.WriteLine("Browser_BindingContextChanged ...");
        }

        private void Browser_LocationChanged(object sender, EventArgs e)
        {
            UpdateStatusMsg(datacontext, "Debug:Browser_LocationChanged : " + e.ToString() + " ...", -1);
        }

        private void Browser_RegionChanged(object sender, EventArgs e)
        {
            UpdateStatusMsg(datacontext, "Debug:Browser_RegionChanged : " + e.ToString() + " ...", -1);
        }

        private void Browser_JavascriptMessageReceived(object sender, JavascriptMessageReceivedEventArgs e)
        {
            UpdateStatusMsg(datacontext, "Browser_JavascriptMessageReceived : " + e.Message + " ...", -1);
        }

        private void Browser_LoadError(object sender, LoadErrorEventArgs e)
        {
            UpdateStatusMsg(datacontext, "Browser_LoadError(" + e.ErrorCode.ToString() + ") : " + e.ErrorText + " ...", -1);
        }

        private void Browser_TitleChanged(object sender, TitleChangedEventArgs e)
        {
            UpdateStatusMsg(datacontext, "Browser_TitleChanged : " + e.Title + " ...", -1);
        }
    }
#pragma warning restore CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
}
