using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
    public partial class BaseWndContextData : INotifyPropertyChanged
    {
        #region Variable_and_Properties
        public int MAX_WAIT_DOC_INIT_SCRIPT_COMPLETE { get; } = 30;
        public int MAX_REFRESH_CNT { get; } = 60;
        public int SLEEP_BETWEEN_RFRESH_MILI_SECONDS { get; } = 1000;
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public Dictionary<string, DownloadStatus> DictDownloadStatus { get; } = new Dictionary<string, DownloadStatus>();

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

        bool _bYouHub = false;
        public bool IsYouTube { get { return _bYouHub; } set { _bYouHub = value; } }

        bool _bPornHub = false;
        public bool IsPornTube { get { return _bPornHub; } set { _bPornHub = value; } }
        public string FileSavePath { get; set; } = "";
        public string FileTempPath { get; set; } = "";
        
        #endregion Variable_and_Properties
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
                case (int)BatchQueryNovelContents.NOVEL69:
                    return "https://69shuba.cx/txt/38164/26461664";
                case (int)BatchQueryNovelContents.UUNOVEL_COM:
                    return "https://www.uuxs.com/chapter/72206/46164826.html";
                case (int)BatchQueryNovelContents.UUNOVEL_CC:
                    return "http://www.uuxs8.cc/r46844/29229950.html";
                case (int)BatchQueryNovelContents.FINISHEDNOVEL:
                    return "https://www.qbxsw.com/du_134667/49689977.html";

                case (int)BatchQueryNovelContents.YOUTUBE:
                    return "https://www.youtube.com/watch?v=0pPPXeXKdfg";
                case (int)BatchQueryNovelContents.PORNHUB:
                    return "https://jp.pornhub.com/view_video.php?viewkey=6617c17b03771";
                case (int)BatchQueryNovelContents.REDPORN:
                    return "https://redporn.porn/15960451?title=bellesa-films-riley-reyes-romantic-missionary-sex-porn";
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
                case BatchQueryNovelContents.NOVEL69:
                    return new Novel69PCBookNovelContent();
                case BatchQueryNovelContents.UUNOVEL_COM:
                    return new NovelUuComBookNovelContent();
                case BatchQueryNovelContents.UUNOVEL_CC:
                    return new NovelUuCCBookNovelContent();
                case BatchQueryNovelContents.FINISHEDNOVEL:
                    return new NovelFinishBookNovelContent();
                //case BatchQueryNovelContents.UUNOVEL_NET:
                //    return new NovelUuNetPCBookNovelContent();

                case BatchQueryNovelContents.YOUTUBE:
                    return new YouTubeStreamPageContent();
                case BatchQueryNovelContents.PORNHUB:
                    return new PornHubStreamPageContent();
                case BatchQueryNovelContents.REDPORN:
                    return new RedPornStreamPageContent();
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
        [EnumCode("69电子书")]
        NOVEL69 = 17,
        //[EnumCode("UU电子书")]
        //UUNOVEL_NET = 18,
        [EnumCode("UU电子书")]
        UUNOVEL_COM = 18,
        [EnumCode("UU小说网")]
        UUNOVEL_CC = 19,
        [EnumCode("UU小说网")]
        FINISHEDNOVEL = 20,

        [EnumCode("YouTube")]
        YOUTUBE = FINISHEDNOVEL + 1,
        [EnumCode("PornHub")]
        PORNHUB = YOUTUBE + 1,
        [EnumCode("RedPorn")]
        REDPORN = PORNHUB + 1,
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