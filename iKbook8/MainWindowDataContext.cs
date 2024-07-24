using BaseBookDownload;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;

namespace BookDownloader
{
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。

    public class WndContextData : BaseWndContextData
    {
        public Visibility EnabledDbgButtons { get; set; } =
#if DEBUG
#if true
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
        public void NovelTypeChangeEvent(BaseWndContextData datacontext, int nIndex)
        {
            if (txtInitURL != null)
            {
                Debug.WriteLine("Select Combox Index : " + cmbNovelType.SelectedIndex);
                if (txtInitURL != null)
                {
                    Debug.WriteLine("Select Combox Index : " + cmbNovelType.SelectedIndex);
                    txtInitURL.Text = datacontext.GetDefaultUrlByIdx(cmbNovelType.SelectedIndex);
                }

            }
        }

        public bool isWebBrowserEmpty()
        {
            try
            {
                bool bRet = false;
                this.Dispatcher.Invoke(() => { bRet = webBrowser == null || webBrowser.Document == null; });
                return bRet;
            }catch(Exception) { 
                return false; 
            }
        }

        public bool isWebPageLoadComplete(string strURL)
        {
            SHDocVw.WebBrowser? webBrowserPtr = GetWebBrowserPtr(webBrowser);
            if (webBrowserPtr == null)
                return false;
                Debug.WriteLine(strURL + " : Status <" + webBrowserPtr?.ReadyState.ToString() + ">");
            if (webBrowserPtr?.ReadyState != SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
                return false;
            else
                return true;
        }
    }
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
}