using BaseBookDownloader;
using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace BookDownloadFormApp
{
#pragma warning disable CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
    public partial class WndContextData : BaseWndContextData
    {

    }

    partial class WindowsFormWebView2 : IBaseMainWindow
    {
        public void NovelTypeChangeEvent(BaseWndContextData datacontext, int nIndex)
        {
            if (txtInitURL != null)
            {
                Debug.WriteLine("Select Combox Index : " + cmbNovelType.SelectedIndex);
                txtInitURL.Text = datacontext.GetDefaultUrlByIdx(cmbNovelType.SelectedIndex);
            }
        }

        public bool DownloadFile(BaseWndContextData? datacontext, string sDownloadURL)
        {
            throw new NotImplementedException();
        }

        public bool DownloadFile(BaseWndContextData? datacontext, List<string> listUrls)
        {
            throw new NotImplementedException();
        }
    }
#pragma warning restore CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
}
