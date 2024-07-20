using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Xaml;

namespace BookDownloader
{
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。

    public class WndContextData : BaseWndContextData, INotifyPropertyChanged
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
        
    public partial class WPFMainWindow : BaseDownloadWnd
    {
        private void NovelTyhpeChangeToIndex(int nIndex)
        {
            if (txtInitURL != null)
            {
                Debug.WriteLine("Select Combox Index : " + cmbNovelType.SelectedIndex);
                switch (cmbNovelType.SelectedIndex)
                {
                    case (int)BatchQueryNovelContents.IKBOOK8:
                        txtInitURL.Text = "https://m.ikbook8.com/book/i116399132/18897986.html";
                        break;
                    case (int)BatchQueryNovelContents.QQBOOK:
                        txtInitURL.Text = "https://book.qq.com/book-read/47135031/1";
                        break;
                    case (int)BatchQueryNovelContents.BIQUGE:
                        txtInitURL.Text = "https://m.xbiqugew.com/book/50761/32248795.html";
                        break;
                    case (int)BatchQueryNovelContents.BIQUGE2:
                        txtInitURL.Text = "https://www.xbiqugew.com/book/18927/12811470.html";
                        break;
                    case (int)BatchQueryNovelContents.WXDZH:
                        txtInitURL.Text = "https://www.wxdzs.net/wxread/94612_43816524.html";
                        break;
                    case (int)BatchQueryNovelContents.CANGQIONG:
                        txtInitURL.Text = "http://www.cqhhhs.com/book/85756/28421368.html";
                        break;
                    case (int)BatchQueryNovelContents.JINYONG:
                        txtInitURL.Text = "http://www.jinhuaja.com/b/184315/976061.html";
                        break;
                    case (int)BatchQueryNovelContents.SHUQI:
                        txtInitURL.Text = "https://www.shuqi.com/reader?bid=8991909&cid=2589796";
                        break;
                    case (int)BatchQueryNovelContents.FANQIE:
                        txtInitURL.Text = "https://fanqienovel.com/reader/7100359997917397512?enter_from=page";
                        break;
                    case (int)BatchQueryNovelContents.FANQIE2:
                        txtInitURL.Text = "https://fanqienovel.com/reader/7268154919981580800?source=seo_fq_juhe";
                        break;
                    case (int)BatchQueryNovelContents.HXTX:
                        txtInitURL.Text = "https://www.hongxiu.com/chapter/7200532503839703/19328808342308247";
                        break;
                    case (int)BatchQueryNovelContents.XXSB:
                        txtInitURL.Text = "https://book.xxs8.com/677942/94808.html";
                        break;
                    case (int)BatchQueryNovelContents.YQXSB:
                        txtInitURL.Text = "https://www.xs8.cn/chapter/3738025904323901/10686192507259378";
                        break;
                    case (int)BatchQueryNovelContents._17K:
                        txtInitURL.Text = "https://www.17k.com/chapter/3589602/48625472.html";
                        break;
                    //case (int)BatchQueryNovelContents.TOBEDONE:
                    //    break;
                    default:
                        Debug.Assert(false);
                        break;
                }
            }
        }

        public void AnalysisHtmlBodyThreadFunc(WndContextData? datacontext, WPFMainWindow wndMain, string strURL, string strBody, bool bSilenceMode = false, DownloadStatus? status = null)
        {
            Debug.Assert(!bSilenceMode || (bSilenceMode && status != null));

            IFetchNovelContent? fetchNovelContent = null;
            int nMaxRetry = 60; //span is 3s.
            switch (datacontext?.SiteType)
            {
                case BatchQueryNovelContents.IKBOOK8:
                    fetchNovelContent = new IKBook8NovelContent();
                    break;
                case BatchQueryNovelContents.QQBOOK:
                    fetchNovelContent = new OOBookNovelContent();
                    break;
                case BatchQueryNovelContents.WXDZH:
                    fetchNovelContent = new WxdzsBookNovelContent();
                    break;
                case BatchQueryNovelContents.CANGQIONG:
                    fetchNovelContent = new CangQiongBookNovelContent();
                    break;
                case BatchQueryNovelContents.JINYONG:
                    fetchNovelContent = new JinYongBookNovelContent();
                    break;
                case BatchQueryNovelContents.SHUQI:
                    nMaxRetry = 60;
                    fetchNovelContent = new ShuQiBookNovelContent();
                    break;
                case BatchQueryNovelContents.FANQIE:
                case BatchQueryNovelContents.FANQIE2:
                    nMaxRetry = 60;
                    fetchNovelContent = new FanQieBookNovelContent();
                    break;
                case BatchQueryNovelContents.BIQUGE:
                case BatchQueryNovelContents.BIQUGE2:
                    fetchNovelContent = new BiQuGeBookNovelContent();
                    break;
                case BatchQueryNovelContents.HXTX:
                    fetchNovelContent = new HXTXBookNovelContent();
                    break;
                case BatchQueryNovelContents.XXSB:
                    fetchNovelContent = new XXSBBookNovelContent();
                    break;
                case BatchQueryNovelContents.YQXSB:
                    fetchNovelContent = new YQXSBBookNovelContent();
                    break;
                default:
                    break;
            }

            if (bSilenceMode)
            {
                while (status?.DownloadFinished == false && !datacontext.UnloadPgm)
                {
                    Thread.Sleep(200);
                }
                UpdateStatusMsg(datacontext, strURL + " : Begin to Analysize downloaded Contents Body using " + datacontext?.SiteType.ToCode() + "<" + fetchNovelContent?.GetType()?.Name + "> ...", (int)((100.0 / DownloadStatus.ThreadMax * (status?.ThreadNum ?? 1 - 1 + 0.5))));
            }
            else
            {
                while(datacontext?.PageLoaded == false && !datacontext.UnloadPgm)
                {
                    Thread.Sleep(200);
                }
                UpdateStatusMsg(datacontext, strURL + " : Begin to Analysize downloaded Contents Body using " + datacontext?.SiteType.ToCode() + "<" + fetchNovelContent?.GetType()?.Name + "> ...", 50);
            }

            if (fetchNovelContent != null)
            {
                fetchNovelContent.AnalysisHtmlBookBody(this, datacontext, strURL, strBody, bSilenceMode, status, nMaxRetry);
            }
            if (bSilenceMode)
            {
                UpdateStatusMsg(datacontext, strURL + " : Finished Analysing of downloaded Uri Contents Body of No." + status?.ThreadNum ?? 1 + " ...", (int)((100.0 / DownloadStatus.ThreadMax * (status?.ThreadNum ?? 1))));
            }
            else
                UpdateStatusMsg(datacontext, strURL + " : Finished Analysing of downloaded Uri Contents Body of No. " + status?.ThreadNum ?? 1 + "...", 100);

            if (bSilenceMode)
            {
                if (status?.ThreadNum < DownloadStatus.ThreadMax && !string.IsNullOrEmpty(status.NextUrl))
                {
                    DownloadOneURLAndGetNext(datacontext, wndMain,status.NextUrl);
                }
                else
                {
                    string sDownloadFileName = ((FileStream)DownloadStatus.ContentsWriter.BaseStream).Name;
                    DownloadStatus.ContentsWriter = null;

                    const Int32 BufferSize = 2048;
                    using (FileStream fileStream = File.OpenRead(sDownloadFileName))
                    using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
                    {
                        UpdateStatusMsg(datacontext, "Analysize of Chapter in download file : " + sDownloadFileName, -1);
                        String? sLine;
                        long lLine = 0;
                        while ((sLine = streamReader.ReadLine()) != null)
                        {
                            lLine++;
                            if (!string.IsNullOrEmpty(sLine.Trim()))
                            {
                                //Match matche = Regex.Match(line, "[^第]*(第[]*[][^章节页]*");// Process line
                                string sPattern = "第[0-9０-９ \t一二三四五六七八九十百千万亿壹贰叁肆伍陆柒捌玖拾佰仟零]+[章节页]";
                                bool result = Regex.IsMatch(sLine, sPattern);
                                if(result)
                                    UpdateStatusMsg(datacontext, lLine.ToString()+ " : " + sLine.Trim(), -1);
                                //Match matche = Regex.Match(line, sPattern);// Process line
                            }
                        }
                    }

                    datacontext.BackGroundNotRunning = true;

                    UpdateStatusMsg(datacontext, "Finished batch download(Total " + status?.ThreadNum + " Downloaded) ...", 100);
                    String strMsgAreaLog="";
                    this.Dispatcher.Invoke(() =>
                    {
                        wndMain.btnInitURL.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                        wndMain.btnAutoDownload.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                        UpdateStatusMsg(datacontext, "Flush Log to file: " + sDownloadFileName + ".log", -1);
                        if (!string.IsNullOrEmpty(status?.NextUrl))
                            txtInitURL.Text = status.NextUrl;

                        strMsgAreaLog = txtLog.Text;
                        MessageBox.Show(this, "Batch download finished...", "Web Novel Downloader", MessageBoxButton.OK);
                    });
                    using (FileStream fileStream = File.OpenWrite(sDownloadFileName.Replace(".txt","_log.log")))
                    using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8, BufferSize))
                    {
                        streamWriter.WriteLine(strMsgAreaLog);
                    }
                }
            }
        }
    }
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
}