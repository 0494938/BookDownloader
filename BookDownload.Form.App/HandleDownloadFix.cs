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
        public void NovelTypeChangeEvent(BaseWndContextData datacontext, int nIndex)
        {
            if (txtInitURL != null)
            {
                Debug.WriteLine("Select Combox Index : " + cmbNovelType.SelectedIndex);
                txtInitURL.Text = datacontext.GetDefaultUrlByIdx(cmbNovelType.SelectedIndex);
            }
        }
    }
#pragma warning restore CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
}
