using BaseBookDownloader;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;

namespace WpfBookDownloader
{
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning disable CA1416 // プラットフォームの互換性を検証

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
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning restore CA1416 // プラットフォームの互換性を検証
}