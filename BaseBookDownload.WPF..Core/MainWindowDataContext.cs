using BaseBookDownload;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;

namespace BookDownloaderWpf
{
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning disable CA1416 // プラットフォームの互換性を検証

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
        
    public partial class WindowsWPFChrome : Window, IBaseMainWindow
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
                bool bEmpty = false;
                this.Dispatcher.Invoke(() => { bEmpty = webBrowser == null || webBrowser.IsLoading == true; });
                return bEmpty;
            }catch(Exception) { 
                return false; 
            }
        }

        public bool isWebPageLoadComplete(string strURL)
        {
            try
            {
                bool bComplete = false;
                this.Dispatcher.Invoke(() => { bComplete = webBrowser != null && webBrowser.IsLoaded == true; });
                return bComplete;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning restore CA1416 // プラットフォームの互換性を検証
}