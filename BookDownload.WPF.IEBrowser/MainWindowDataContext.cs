using BaseBookDownloader;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace WpfIEBookDownloader
{
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。

    public class WndContextData : BaseWndContextData
    {
        public Visibility EnabledDbgButtons { get; set; } =
#if DEBUG
#if false
            Visibility.Visible;
#else
            Visibility.Hidden;
#endif
#else
            Visibility.Hidden;
#endif
    }
        
    public partial class WPFMainWindow : Window, IBaseMainWindow
    {
        public void NovelTypeChangeEvent(BaseWndContextData? datacontext, int nIndex)
        {
            if (txtInitURL != null && datacontext != null)
            {
                //Debug.WriteLine("Select Combox Index : " + cmbNovelType.SelectedIndex);
                UpdateStatusMsg(App.Current.MainWindow.DataContext as WndContextData, "Select Combox Index : " + cmbNovelType.SelectedIndex + " " + ((BatchQueryNovelContents)cmbNovelType.SelectedIndex).ToCode(), -1);
                if (txtInitURL != null)
                {
                    Debug.WriteLine("Select Combox Index : " + cmbNovelType.SelectedIndex);
                    txtInitURL.Text = BaseWndContextData.GetDefaultUrlByIdx(cmbNovelType.SelectedIndex);

                    if (string.IsNullOrEmpty(txtCurURL.Text.Trim()))
                    {
                        txtCurURL.Text = txtInitURL.Text;
                    }
                }
            }
            if ((datacontext != null))
            {
                datacontext.SelectedIdx = nIndex;
                //btnDownloadYouTube.GetBindingExpression(Button.VisibilityProperty).UpdateTarget();
                //btnDownloadPornHub.GetBindingExpression(Button.VisibilityProperty).UpdateTarget();
            }
            UpdateInitPageButton();
            UpdateNextPageButton();
            UpdateAutoDownloadPageButton();
        }

        public bool isWebBrowserEmpty(BaseWndContextData? datacontext = null)
        {
            try
            {
                bool bLoadUnFinish = false;
                //this.Dispatcher.Invoke(() => { bRet = webBrowser == null || webBrowser.Document == null; });
                this.Dispatcher.Invoke(() => { bLoadUnFinish = webBrowser == null || webBrowser.CoreWebView2 == null || webBrowser.IsLoaded == false; });
                return bLoadUnFinish;
            }catch(Exception) { 
                return false; 
            }
        }

        public bool isWebPageLoadComplete(string strURL, BaseWndContextData? datacontext = null)
        {
            try
            {
                bool bLoadFinish = false;
                //this.Dispatcher.Invoke(() => { bRet = webBrowser == null || webBrowser.Document == null; });
                this.Dispatcher.Invoke(() => { bLoadFinish = webBrowser != null && webBrowser.CoreWebView2 != null || webBrowser.IsLoaded == true; });
                return bLoadFinish;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
}