using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseBookDownloader
{
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning disable CS8604 // Null 参照代入の可能性があります。
#pragma warning disable CS8632 // Null 参照代入の可能性があります。
    public class DownloadStatus
    {
        public bool DownloadFinished { get; set; } = false;
        public int DownloadRetried { get; set; } = 0;
        public string? URL { get; set; }
        public string? NextUrl { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        //public int Depth { get; set; } = 0;
        public int PageNum { get; set; }
        public static int MaxPageToDownload { get; set; }
        private static StreamWriter? _output_writer = null;
        public static StreamWriter? ContentsWriter
        {
            get
            {
                return _output_writer;
            }
            set
            {
                if (_output_writer != null)
                {
                    _output_writer.Flush();
                    _output_writer.Close();
                }
                _output_writer = value;
            }
        }
    }

    public class BaseWndContextData : INotifyPropertyChanged
    {
        public int MAX_WAIT_DOC_INIT_SCRIPT_COMPLETE { get; } = 30;
        public int MAX_REFRESH_CNT { get; } = 60;
        public int SLEEP_BETWEEN_RFRESH_MILI_SECONDS { get; } = 1000;
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public Dictionary<string, DownloadStatus> DictDownloadStatus { get;  } = new Dictionary<string, DownloadStatus>();

        public bool BackGroundNotRunning { get; set; } = true;
        bool _pageLoaded = false;
        public bool PageLoaded { get { return _pageLoaded && BackGroundNotRunning; } set { _pageLoaded = value; } }
        bool _contentsAnalysised = false;
        public bool ContentsAnalysised { get { return _contentsAnalysised && BackGroundNotRunning; } set { _contentsAnalysised = value; } }
        bool _nextLinkAnalysized = false;
        public bool NextLinkAnalysized { get { return _nextLinkAnalysized && BackGroundNotRunning; } set { _nextLinkAnalysized = value; } } 
        public BatchQueryNovelContents SiteType { get; set; } = BatchQueryNovelContents.IKBOOK8;
        public string? StartBarMsg { get; set; }
        public int ProcessBarValue { get; set; }
        public string? PgmNaviUrl { get; set; }
        public bool UnloadPgm { get; set; } = false;
        public int RefreshCount { get; set; } = 0;


        public void DownloadOneURLAndGetNext(IBaseMainWindow wndMain, string strURL, bool bRefresh)
        {
            if (!UnloadPgm)
            {
                try
                {
                    if (bRefresh)
                    {
                        DownloadStatus status = DictDownloadStatus[strURL];
                        status.DownloadFinished = false;
                        Debug.Assert(status.URL == strURL);
                        status.NextUrl = "";
                    }
                    else
                        DictDownloadStatus[strURL] = new DownloadStatus { DownloadFinished = false, URL = strURL, NextUrl = "", StartTime = DateTime.Now, PageNum = DictDownloadStatus.Count + 1 };

                    wndMain.LoadUiUrl(this, strURL);
                    wndMain.UpdateStatusMsg(this, strURL + " : Begin to download ...", (int)((100.0 / DownloadStatus.MaxPageToDownload * (DictDownloadStatus[strURL].PageNum - 1))));
                    WaitFinishForNext(wndMain, strURL, true);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    if (wndMain.isWebBrowserEmpty())
                    {
                        Debug.Assert(false);
                        WaitFinishForNext(wndMain, strURL, true);
                    }

                    if (!wndMain.isWebPageLoadComplete(strURL))
                        return ;
                }
            }
            else
            {
                Debug.Assert(true);
            }
        }

        void WaitFinishForNext(IBaseMainWindow wndMain, string strURL, bool bSilenceMode = false)
        {
            DownloadStatus status = DictDownloadStatus[strURL];

            try
            {
                Thread thread = new Thread(() => WaitAndLaunchAnalsysi(wndMain, strURL, bSilenceMode, status));
                thread.Start();
            }
            catch (TaskCanceledException)
            {
                //ignore TaskCanceledException
            }
        }

#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning disable CS8604 // Null 参照引数の可能性があります。
        public void WaitAndLaunchAnalsysi(IBaseMainWindow wndMain, string strURL, bool bSilenceMode, DownloadStatus status)
        {
            const int MAX_RETRY = 60 * 5 * 2; //wait loading up to 2 minutes.
            int nWaitRetry = 0;

            if (bSilenceMode)
            {
                while (status.DownloadFinished == false && !this.UnloadPgm && nWaitRetry < MAX_RETRY)
                {
                    Thread.Sleep(200);
                    nWaitRetry++;
                }
            }
            else
            {
                while (this.PageLoaded == false && !this.UnloadPgm && nWaitRetry < MAX_RETRY)
                {
                    Thread.Sleep(200);
                    nWaitRetry++;
                }
            }

            Debug.WriteLine($"{strURL} : Download Finished, Begin Analysis ...");

            if (!UnloadPgm)
            {
                try
                {
                    string? strBody = wndMain.GetWebDocHtmlBody(strURL);
                    wndMain.UpdateWebBodyOuterHtml(strBody);
                    if (!string.IsNullOrEmpty(strBody))
                    {
                        AnalysisHtmlBody(wndMain, true, strURL, strBody, bSilenceMode, status);
                    }
                    else if(bSilenceMode && status.DownloadRetried == 0)
                    {
                        status.DownloadRetried = status.DownloadRetried + 1;
                        wndMain.LoadHtmlString("<html><body></body></html> ", "blank");
                        wndMain.UpdateStatusMsg(this, "Faield to Get Document Contents of <" + status.URL + ">, Force Reload to Retry",-1);
                        Thread.Sleep (200);
                        status.DownloadFinished = false;
                        //wndMain.LoadUiUrl(this, status.URL);
                        wndMain.Back(this);
                        //strBody = wndMain.GetWebDocHtmlBody(strURL);
                        //if (!string.IsNullOrEmpty(strBody))
                        //{
                        //    AnalysisHtmlBody(wndMain, true, strURL, strBody, bSilenceMode, status);
                        //}
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.StackTrace);
                }
            }
        }

        public void AnalysisHtmlBody(IBaseMainWindow wndMain, bool bWaitOption, string strURL, string strBody, bool bSilenceMode = false, DownloadStatus? status = null)
        {
            try
            {
                Thread thread = new Thread(() => AnalysisHtmlBodyThreadFunc(wndMain, strURL, strBody, bSilenceMode, status));
                thread.Start();
            }
            catch (TaskCanceledException)
            {
                //ignore TaskCanceledException
            }
            //AnalysisHtmlBodyThreadFunc(datacontext, this, strURL, strBody, bSilenceMode, status);
        }

        public void AnalysisHtmlBodyThreadFunc(IBaseMainWindow wndMain, string strURL, string strBody, bool bSilenceMode = false, DownloadStatus? status = null)
        {
            Debug.Assert(!bSilenceMode || (bSilenceMode && status != null));

            int nMaxRetry = 60; //span is 3s.
            IFetchNovelContent? fetchNovelContent = GetImplByNoveTypeIdx(SiteType, ref nMaxRetry);


            if (bSilenceMode)
            {
                while (status?.DownloadFinished == false && !UnloadPgm)
                {
                    Thread.Sleep(200);
                }
                wndMain.UpdateStatusMsg(this, strURL + " : Begin to Analysize downloaded Contents Body using " + SiteType.ToCode() + "<" + fetchNovelContent?.GetType()?.Name + "> ...", (int)((100.0 / DownloadStatus.MaxPageToDownload * (status?.PageNum ?? 1 - 1 + 0.5))));
            }
            else
            {
                while (this.PageLoaded == false && !this.UnloadPgm)
                {
                    Thread.Sleep(200);
                }
                wndMain.UpdateStatusMsg(this, strURL + " : Begin to Analysize downloaded Contents Body using " + this.SiteType.ToCode() + "<" + fetchNovelContent?.GetType()?.Name + "> ...", 50);
            }

            bool bNoNeedFresh = true;
            if (fetchNovelContent != null)
            {
                bNoNeedFresh = fetchNovelContent.AnalysisHtmlBookBody(wndMain, this, strURL, strBody, bSilenceMode, status, nMaxRetry);
            }
            if (bSilenceMode)
            {
                wndMain.UpdateStatusMsg(this, strURL + " : Finished Analysing of downloaded Uri Contents Body, No." + (status?.PageNum ?? 1) + " / " + DownloadStatus.MaxPageToDownload + " ...", (int)((100.0 / DownloadStatus.MaxPageToDownload * (status?.PageNum ?? 1))));
            }
            else
                wndMain.UpdateStatusMsg(this, strURL + " : Finished Analysing of downloaded Uri Contents Body, No. " + (status?.PageNum ?? 1) + " / 1 ...", 100);

            if (bSilenceMode)
            {
                if (!bNoNeedFresh)
                {
                    if (this.RefreshCount > 0)
                    {
                        Debug.Assert(true);
                        this.RefreshCount--;
                        if (this.RefreshCount == 0)
                        {
                            wndMain.UpdateStatusMsg(this, "Failed as over access reqencey by website, After Retry Max to 60 seconds ...", 100 / 20 * this.RefreshCount);
                            return;
                        }
                        wndMain.UpdateStatusMsg(this, "Failed as over access reqencey by website, Retry No." + this.RefreshCount + " to 60 seconds ...", 100 / this.MAX_REFRESH_CNT * this.RefreshCount);
                        Thread.Sleep(this.SLEEP_BETWEEN_RFRESH_MILI_SECONDS);
                        if (status != null)
                            status.DownloadFinished = false;
                        //wndMain.RefreshPage();
                        DownloadOneURLAndGetNext(wndMain, strURL, true);
                        return;
                    }
                    else
                    {
                        this.RefreshCount = this.MAX_REFRESH_CNT;
                        Debug.Assert(true);
                        wndMain.UpdateStatusMsg(this, "Failed as over access reqencey by website, Retry No." + this.RefreshCount + " to 60 seconds ...", 100 / this.MAX_REFRESH_CNT * this.RefreshCount);
                        Thread.Sleep(this.SLEEP_BETWEEN_RFRESH_MILI_SECONDS);
                        if (status != null)
                            status.DownloadFinished = false;
                        //wndMain.RefreshPage();
                        DownloadOneURLAndGetNext(wndMain, strURL, true);
                    }
                }
                else
                {
                    this.RefreshCount = 0;
                }

                if (status?.PageNum < DownloadStatus.MaxPageToDownload && !string.IsNullOrEmpty(status.NextUrl))
                {
                    DownloadOneURLAndGetNext(wndMain, status.NextUrl, false);
                }
                else
                {
                    string sDownloadFileName = ((FileStream)DownloadStatus.ContentsWriter.BaseStream).Name;
                    DownloadStatus.ContentsWriter = null;

                    string sName = Path.GetFileName(sDownloadFileName);
                    string sPath = Path.GetDirectoryName(sDownloadFileName).ToString();
                    if (sName.StartsWith("Novel") && !string.IsNullOrEmpty(wndMain.GetNovelName()))
                    {
                        sName = wndMain.GetNovelName() + sName.Substring("Novel".Length);
                        File.Move(sDownloadFileName, sPath + Path.DirectorySeparatorChar+ sName);
                        sDownloadFileName = sPath + Path.DirectorySeparatorChar + sName;
                    }

                    const Int32 BufferSize = 2048;
                    using (FileStream fileStream = File.OpenRead(sDownloadFileName))
                    using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
                    {
                        wndMain.UpdateStatusMsg(this, "Analysize of Chapter in download file : " + sDownloadFileName, -1);
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
                                if (result)
                                    wndMain.UpdateStatusMsg(this, lLine.ToString() + " : " + sLine.Trim(), -1);
                                //Match matche = Regex.Match(line, sPattern);// Process line
                            }
                        }
                    }

                    this.BackGroundNotRunning = true;

                    wndMain.UpdateStatusMsg(this, "Finished batch download(Total " + status?.PageNum + " Downloaded) ...", 100);
                    String strMsgAreaLog = wndMain.BatchDownloadNotified(this, status, sDownloadFileName);
                    using (FileStream fileStream = File.OpenWrite(sDownloadFileName.Replace(".txt", "_log.log")))
                    using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8, BufferSize))
                    {
                        streamWriter.WriteLine(strMsgAreaLog);
                    }
                }
            }
        }

        public string GetDefaultUrlByIdx(int nIdx)
        {
            switch (nIdx)
            {
                case (int)BatchQueryNovelContents.IKBOOK8:
                    return "https://m.ikbook8.com/book/i116399132/18897986.html";
                case (int)BatchQueryNovelContents.QQBOOK:
                    return "https://book.qq.com/book-read/47135031/1";
                case (int)BatchQueryNovelContents.BIQUGE:
                    return "https://m.xbiqugew.com/book/50761/32248795.html";
                case (int)BatchQueryNovelContents.BIQUGE_PC:
                    return "https://www.xbiqugew.com/book/18927/12811470.html";
                case (int)BatchQueryNovelContents.WXDZH:
                    return "https://www.wxdzs.net/wxread/94612_43816524.html";
                case (int)BatchQueryNovelContents.CANGQIONG:
                    return "http://www.cqhhhs.com/book/85756/28421368.html";
                case (int)BatchQueryNovelContents.JINYONG:
                    return "http://www.jinhuaja.com/b/184315/976061.html";
                case (int)BatchQueryNovelContents.SHUQI:
                    return "https://www.shuqi.com/reader?bid=8991909&cid=2589796";
                case (int)BatchQueryNovelContents.FANQIE:
                    return "https://fanqienovel.com/reader/7100359997917397512?enter_from=page";
                case (int)BatchQueryNovelContents.FANQIE2:
                    return "https://fanqienovel.com/reader/7268154919981580800?source=seo_fq_juhe";
                case (int)BatchQueryNovelContents.HXTX:
                    return "https://www.hongxiu.com/chapter/7200532503839703/19328808342308247";
                case (int)BatchQueryNovelContents.XXSB:
                    return "https://book.xxs8.com/677942/94808.html";
                case (int)BatchQueryNovelContents.YQXSB:
                    return "https://www.xs8.cn/chapter/3738025904323901/10686192507259378";
                case (int)BatchQueryNovelContents._17K:
                    return "https://www.17k.com/chapter/3589602/48625472.html";
                case (int)BatchQueryNovelContents.COLA:
                    return "https://www.keleshuba.net/book/302396/173366895.html";
                case (int)BatchQueryNovelContents.TIANTIAN:
                    return "https://m.ttshu8.com/book/33693/124470410.html";
                case (int)BatchQueryNovelContents.TIANTIAN_PC:
                    return "https://www.ttshu8.com/book/33693/124470410.html";
                //case (int)BatchQueryNovelContents.TOBEDONE:
                //    break;
                default:
                    Debug.Assert(false);
                    return "";
            }
        }

        public IFetchNovelContent? GetImplByNoveTypeIdx(BatchQueryNovelContents nIdx, ref int nMaxRetry)
        {
            switch (nIdx)
            {
                case BatchQueryNovelContents.IKBOOK8:
                    return new IKBook8NovelContent();
                case BatchQueryNovelContents.QQBOOK:
                    return new OOBookNovelContent();
                case BatchQueryNovelContents.WXDZH:
                    return new WxdzsBookNovelContent();
                case BatchQueryNovelContents.CANGQIONG:
                    return new CangQiongBookNovelContent();
                case BatchQueryNovelContents.JINYONG:
                    return new JinYongBookNovelContent();
                case BatchQueryNovelContents.SHUQI:
                    nMaxRetry = 60;
                    return new ShuQiBookNovelContent();
                case BatchQueryNovelContents.FANQIE:
                case BatchQueryNovelContents.FANQIE2:
                    nMaxRetry = 60;
                    return new FanQieBookNovelContent();
                case BatchQueryNovelContents.BIQUGE:
                case BatchQueryNovelContents.BIQUGE_PC:
                    return new BiQuGeBookNovelContent();
                case BatchQueryNovelContents.HXTX:
                    return new HXTXBookNovelContent();
                case BatchQueryNovelContents.XXSB:
                    return new XXSBBookNovelContent();
                case BatchQueryNovelContents.YQXSB:
                    return new YQXSBBookNovelContent();
                case BatchQueryNovelContents._17K:
                    return new _17KBookNovelContent();
                case BatchQueryNovelContents.COLA:
                    return new CoraBookNovelContent();
                case BatchQueryNovelContents.TIANTIAN:
                    return new TianTianBookNovelContent();
                case BatchQueryNovelContents.TIANTIAN_PC:
                    return new TianTianPCBookNovelContent();
                default:
                    return null;
            }
        }
    }

    public enum BatchQueryNovelContents
    {
        [EnumCode("爱看书吧")]
        IKBOOK8 = 0,
        [EnumCode("QQ电子书")]
        QQBOOK = 1,
        [EnumCode("笔趣阁")]
        BIQUGE = 2,
        [EnumCode("笔趣阁")]
        BIQUGE_PC = 3,
        [EnumCode("无线电子书")]
        WXDZH = 4,
        [EnumCode("苍穹小说")]
        CANGQIONG = 5,
        [EnumCode("金庸小说网")]
        JINYONG = 6,
        [EnumCode("书旗小说网")]
        SHUQI = 7,
        [EnumCode("番茄小说网")]
        FANQIE = 8,
        [EnumCode("番茄小说网")]
        FANQIE2 = 9,
        [EnumCode("红袖添香")]
        HXTX = 10,//红袖添香
        [EnumCode("新小说吧")]
        XXSB = 11, //新小说吧
        [EnumCode("言情小说吧")]
        YQXSB = 12,//言情小说吧
        [EnumCode("17K小说网")]
        _17K = 13, //17K，Script Over Flow
        [EnumCode("可乐小说网")]
        COLA = 14, 
        [EnumCode("天天小说")]
        TIANTIAN = 15,
        [EnumCode("天天小说PC版")]
        TIANTIAN_PC = 16,

        //TOBEDONE = 13,
    }

    public static class BatchQueryNovelContentsExtensions
    {
        static BatchQueryNovelContentsExtensions()
        {
            EnumCodeExtensions.AddEnumCodeCache<BatchQueryNovelContents>();
        }

        public static string ToCode(this BatchQueryNovelContents enumKey)
        {
            return EnumCodeExtensions.GetEnumCode(enumKey);
        }
    }
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning restore CS8604 // Null 参照代入の可能性があります。
#pragma warning restore CS8632 // Null 参照代入の可能性があります。
}