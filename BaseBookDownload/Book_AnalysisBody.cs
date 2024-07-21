using BaseBookDownload;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace BaseBookDownload
{
#pragma warning disable CS8632 // Null 参照代入の可能性があります。
    public class BaseBookNovelContent
    {
        protected string? URL { get; set; } = null;
        public static JToken? GetValueByKeyFromJObject(JObject jObj, string sKey)
        {
            JToken? value;
            if (jObj.TryGetValue(sKey, out value))
            {
                return value;
            }
            return null;
        }

        protected void ParseResultToUI(IBaseMainWindow wndMain, bool bSilenceMode, string strContents, string strNextLink)
        {
            //WPFMainWindow wndMain = (WPFMainWindow)IWndMain;
            wndMain.UpdateAnalysizedContents(strContents);
            wndMain.UpdateNextUrl(strNextLink);
            wndMain.UpdateCurUrl(strNextLink);
            if (bSilenceMode)
            {
                wndMain.UpdateAggragatedContentsWithLimit(strContents);
            }
            else
            {
                wndMain.UpdateAggragatedContents(strContents);
            }
        }
    }
#pragma warning restore CS8632 // Null 参照代入の可能性があります。
}