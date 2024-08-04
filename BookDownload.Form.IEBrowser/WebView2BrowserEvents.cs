using BaseBookDownloader;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace BookDownloadFormApp
{
#pragma warning disable CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
    public partial class WndContextData: BaseWndContextData
    {

    }
    partial class WindowsFormWebView2 : IBaseMainWindow
    {

        public void RefreshPage()
        {
            this.Invoke(() =>{
                //webBrowser.CoreWebView2.Navigate(webBrowser.Source.ToString());
                webBrowser.CoreWebView2.Reload();
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

    }
#pragma warning restore CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
}
