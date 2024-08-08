using BaseBookDownloader;
using CefSharp;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfBookDownloader
{
#pragma warning disable CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
#pragma warning disable CA1416 // プラットフォームの互換性を検証
    class _DocContents
    {
        public string sHtml = "";
    }

    public partial class WindowsWPFChrome : Window, IBaseMainWindow
    {

        private void ClickBtnOpenUrl(string strUrl)
        {
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if ((datacontext != null))
            {
                datacontext.DictDownloadStatus.Clear();
                datacontext.DictHandledContentsForDupCheck.Clear();
                datacontext.PageLoaded = false;
                datacontext.NextLinkAnalysized = false;
                UpdateAutoDownloadPageButton();
                UpdateNextPageButton();
                UpdateInitPageButton();
                txtWebContents.Text = "";
                txtAnalysizedContents.Text = "";
                UpdateStatusMsg(datacontext, "Selected Site Type: " + cmbNovelType.SelectedIndex, -1);
                datacontext.SiteType = (BatchQueryNovelContents)cmbNovelType.SelectedIndex;
                try
                {
                    datacontext.PgmNaviUrl = strUrl;
                    webBrowser.LoadUrl(strUrl);
                    UpdateStatusMsg(datacontext, strUrl + " : Begin to download ...", 0);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    if (webBrowser == null || webBrowser.IsLoading == true)
                        return;

                    if (webBrowser.IsLoaded != true)
                        return;
                }
            }
        }

        private void GetWebDocHtml(_DocContents doc)
        {
            bool isLoading = false;
            this.Dispatcher.Invoke(() => {
                isLoading = webBrowser.IsLoading;
            });
            int nMaxRetry = 10 * 30, nRetry=0;
            if (nRetry < nMaxRetry && ( webBrowser == null || isLoading == true))
            {
                nRetry++;
                Thread.Sleep(100);
                this.Dispatcher.Invoke(() => {
                    isLoading = webBrowser.IsLoading;
                });
            }
            if (webBrowser == null || isLoading == true)
                return;

            this.Dispatcher.Invoke(() => {
    
                webBrowser.GetMainFrame().GetSourceAsync().ContinueWith(taskHtml =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        doc.sHtml = taskHtml.Result;
                    });
                });
            });
        }

        public void GetBrowserDocAndPutToCtrl()
        {
            _DocContents doc = new _DocContents();
            new Thread(() =>
            {
                try
                {
                    int nMaxRetry = 10 * 60, nRetry = 0;
                    bool bLoaded = false;
                    this.Dispatcher.Invoke(() =>
                    {
                        bLoaded = webBrowser.IsLoaded;
                    });
                    while (nRetry < nMaxRetry && bLoaded == false)
                    {
                        nRetry++;
                        Thread.Sleep(100);
                        this.Dispatcher.Invoke(() =>
                        {
                            bLoaded = webBrowser.IsLoaded;
                        });
                    }
                    if (bLoaded)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            webBrowser.GetSourceAsync().ContinueWith(taskHtml =>
                            {
                                this.Dispatcher.Invoke(() =>
                                {
                                    doc.sHtml = taskHtml.Result;
                                });
                            });
                        });

                        while (nRetry < nMaxRetry && string.IsNullOrEmpty(doc.sHtml))
                        {
                            nRetry++;
                            Thread.Sleep(100);
                        }
                        if (!string.IsNullOrEmpty(doc.sHtml))
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                txtWebContents.Text = doc.sHtml?.Replace("\r\n\r\n\r\n", "\r\n")?.Replace("\r\n\r\n", "\r\n");
                            });
                        }
                    }
                }
                catch (TaskCanceledException) { }

            }).Start();
        }

        public void GetBrowserDocAndPrettyToCtrl()
        {
            _DocContents doc = new _DocContents();
            new Thread(() =>
            {
                try
                {
                    int nMaxRetry = 10 * 60, nRetry = 0;
                    bool bLoaded = false;
                    this.Dispatcher.Invoke(() =>
                    {
                        bLoaded = webBrowser.IsLoaded;
                    });
                    while (nRetry < nMaxRetry && bLoaded == false)
                    {
                        nRetry++;
                        Thread.Sleep(100);
                        this.Dispatcher.Invoke(() =>
                        {
                            bLoaded = webBrowser.IsLoaded;
                        });
                    }
                    if (bLoaded)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            webBrowser.GetSourceAsync().ContinueWith(taskHtml =>
                            {
                                this.Dispatcher.Invoke(() =>
                                {
                                    doc.sHtml = taskHtml.Result;
                                });
                            });
                        });
                        Thread.Sleep(100);

                        while (nRetry < nMaxRetry && string.IsNullOrEmpty(doc.sHtml))
                        {
                            nRetry++;
                            Thread.Sleep(100);
                        }
                        if (!string.IsNullOrEmpty(doc.sHtml))
                        {
                            bool bIgnoreScript = false;
                            bool bIgnoreHead = false;
                            this.Dispatcher.Invoke(() =>
                            {
                                bIgnoreScript = chkboxIgnoreScript.IsChecked ?? false;
                                bIgnoreHead = chkboxIgnoreHeader.IsChecked ?? false;
                            });

                            string strPrettyHtml = PrettyPrintUtil.PrettyPrintHtml(doc.sHtml, bIgnoreScript, bIgnoreHead);

                            this.Dispatcher.Invoke(() =>
                            {
                                txtWebContents.Text = strPrettyHtml;
                            });
                        }

                    }
                }
                catch (TaskCanceledException) { }
            }).Start();
        }
    }
#pragma warning restore CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
#pragma warning restore CA1416 // プラットフォームの互換性を検証
}