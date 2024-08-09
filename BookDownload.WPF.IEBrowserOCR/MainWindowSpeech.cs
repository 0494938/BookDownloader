using BaseBookDownloader;
using Microsoft.Win32;
using NAudio.Lame;
using NAudio.Wave;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Speech.Synthesis;
using System.Windows;

namespace WpfIEBookDownloader
{
#pragma warning disable CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning disable CS8604 // Null 参照引数の可能性があります。
    public partial class WPFMainWindow : Window, IBaseMainWindow
    {

        Thread? threadSpeech=null;
        bool bSpeaking = false;
        int nShift4Errr = -1;
        int nShift = 0;
        SpeechSynthesizer? synthesizer = null;

        private SpeechSynthesizer InitAndHookSpeech(SpeechSynthesizer? synthesizer=null)
        {
            if (synthesizer == null)
            {
                synthesizer = new SpeechSynthesizer();
                synthesizer.Volume = 100;
                synthesizer.Rate = 0;
            }
            synthesizer.SetOutputToDefaultAudioDevice();
            synthesizer.SpeakCompleted += Synthesizer_SpeakCompleted;
            synthesizer.SpeakProgress += Synthesizer_SpeakProgress;
            synthesizer.SpeakStarted += Synthesizer_SpeakStarted;
            synthesizer.VoiceChange += Synthesizer_VoiceChange;
            synthesizer.StateChanged += Synthesizer_StateChanged;
            synthesizer.PhonemeReached += Synthesizer_PhonemeReached;
            synthesizer.VisemeReached += Synthesizer_VisemeReached;
            synthesizer.BookmarkReached += Synthesizer_BookmarkReached;

            return synthesizer;
        }

        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnMainWindowLoaded invoked...");
            //HideScriptErrors(webBrowser, true);
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if (datacontext != null)
            {
                RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\zdhe\\batchdownload\\1.0", false);
                datacontext.FileTempPath = (registryKey?.GetValue("FileTempPath") as string) ?? "";
                datacontext.FileSavePath = (registryKey?.GetValue("FileSavePath") as string) ?? "";
                registryKey?.Close();
            }
            
            if (cmbInstalledVoice.Items.Count == 0)
            {
                synthesizer = InitAndHookSpeech(synthesizer);
                var installed_voices = synthesizer.GetInstalledVoices();

                for (int i = 0; i < installed_voices.Count; i++)
                {
                    Console.WriteLine(GetVoiceInfoDesc(installed_voices[i].VoiceInfo));
                    cmbInstalledVoice.Items.Add(GetVoiceInfoDesc(installed_voices[i].VoiceInfo));
                }
            }
        }

        private void OnMainWindowUnloaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnMainWindowUnloaded invoked...");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            datacontext.UnloadPgm = true;
            webBrowser.Dispose();
        }

        static string GetVoiceInfoDesc(VoiceInfo vi)
        {
            return vi.Culture.Name + " - " + vi.AdditionalInfo["Name"] + " (ID = " + vi.AdditionalInfo["Language"] + ", " + vi.AdditionalInfo["Age"] + ", " + vi.AdditionalInfo["Gender"] + ", " + vi.Culture + ")";
        }

        private void OnConvertToMp3(object sender, RoutedEventArgs e)
        {
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            ConvertTextToMp3(datacontext, (string.IsNullOrEmpty(txtBookName.Text.Trim()) ? "Novel" : txtBookName.Text.Trim()) + 
                "_" + (string.IsNullOrEmpty(txtBookName.Text.Trim()) ? "Chapter" : txtBookName.Text.Trim()));
        }

        private void ConvertTextToMp3(WndContextData datacontext, string strMp3FileName)
        {
            string strText = txtAnalysizedContents.SelectedText.Trim(new char[] { ' ', '\t', '\n', '\r' });
            if (string.IsNullOrEmpty(strText.Trim()) || strText.Trim().Length <= 4)
                return;

            string strVoice = cmbInstalledVoice.SelectedIndex == -1 ? "zh-Hans" : cmbInstalledVoice.Text.Substring(0, cmbInstalledVoice.Text.IndexOf(" - "));

            new Thread(() => {
                BaseBookDownloader.Util.ConvertTextToMp3(this, datacontext, strMp3FileName, strText, strVoice);
            }).Start();
        }

        private void OnClickSpeechText(object sender, RoutedEventArgs e)
        {
            if (threadSpeech != null && (threadSpeech.ThreadState == System.Threading.ThreadState.Running || threadSpeech.ThreadState == System.Threading.ThreadState.WaitSleepJoin)) { 
                if(threadSpeech != null && synthesizer != null)
                {

                    threadSpeech = null;
                    Prompt curPromptBuilder = synthesizer.GetCurrentlySpokenPrompt();
                    synthesizer.SpeakAsyncCancel(curPromptBuilder);

                    bSpeaking = false;
                    btnSpeechText.Text = "Speech Play";
                    nShift4Errr = -1;
                }
            }
            else{
                if (!string.IsNullOrEmpty(txtAnalysizedContents.Text.Trim()))
                {
                    if (synthesizer == null)
                    {
                        synthesizer = InitAndHookSpeech(synthesizer);
                    }
                    else
                    {
                        synthesizer.SetOutputToDefaultAudioDevice();
                    }
                    
                    PromptBuilder builder = new PromptBuilder();
                    if(cmbInstalledVoice.SelectedIndex == -1) 
                        builder.StartVoice(new CultureInfo("zh-Hans"));
                    else
                    {
                        string strCul = cmbInstalledVoice.Text;
                        builder.StartVoice(new CultureInfo(strCul.Substring(0, strCul.IndexOf(" - "))));
                    }
          
                    string strText = txtAnalysizedContents.SelectedText.Trim(new char[] { ' ','\t','\n','\r'});
                    if (!string.IsNullOrEmpty(strText) && strText.Length > 4)
                    {
                        nShift = txtAnalysizedContents.SelectionStart;
                    }
                    else {
                        nShift = 0; strText = "";
                    }

                    builder.AppendText(string.IsNullOrEmpty(strText)? txtAnalysizedContents.Text: strText);
                    builder.EndVoice();

                    btnSpeechText.Text = "Speech Stop";
                    bSpeaking = true;
                    nShift4Errr = -1;
                    threadSpeech = new Thread(() =>
                    {
                        try
                        {
                            synthesizer.Speak(builder);
                        }
                        catch (Exception ) { 
                        }
                        nShift4Errr = -1;
                        bSpeaking = false;
                        threadSpeech = null; 
                        this.Dispatcher.Invoke(() => { btnSpeechText.Text = "Speech Play"; });
                        
                    });
                    threadSpeech.Start();
                }
            }
        }

        private void Synthesizer_SpeakProgress(object? sender, SpeakProgressEventArgs e)
        {
            if (nShift4Errr == -1 && e.CharacterPosition >=0) {
                nShift4Errr = e.CharacterPosition;
            }

            this.Dispatcher.Invoke(() => {
                //txtAnalysizedContents.Select(e.CharacterPosition, e.CharacterCount);
                txtAnalysizedContents.Focus();
                txtAnalysizedContents.Select((nShift4Errr == -1 ? e.CharacterPosition + nShift : e.CharacterPosition - nShift4Errr + nShift), e.CharacterCount);
            });
        }

        private void Synthesizer_SpeakStarted(object? sender, SpeakStartedEventArgs e)
        {
            Debug.WriteLine("Synthesizer_SpeakStarted.  UserState:" + e.UserState + ", Error:" + e.Error + ", Prompt:" + e.Prompt);
        }

        private void Synthesizer_SpeakCompleted(object? sender, SpeakCompletedEventArgs e)
        {
            bSpeaking = false;
            Debug.WriteLine("Synthesizer_SpeakCompleted.  bSpeaking:" + bSpeaking + ", UserState:" + e.UserState);
        }

        private void Synthesizer_BookmarkReached(object? sender, BookmarkReachedEventArgs e)
        {
            Debug.WriteLine("Synthesizer_BookmarkReached.  Bookmark:" + e.Bookmark);
        }

        private void Synthesizer_VisemeReached(object? sender, VisemeReachedEventArgs e)
        {
            //Debug.WriteLine("Synthesizer_VisemeReached.  Viseme:" + e.Viseme + ", NextViseme:" + e.NextViseme + ", UserState:" + e.UserState);
        }

        private void Synthesizer_PhonemeReached(object? sender, PhonemeReachedEventArgs e)
        {
            Debug.WriteLine("Synthesizer_PhonemeReached.  Phoneme:" + e.Phoneme + ", NextPhoneme:" + e.NextPhoneme + ", Duration:" + e.Duration + ", UserState:" + e.UserState + ", AudioPosition:" + e.AudioPosition + ", Emphasis:" + e.Emphasis);
        }

        private void Synthesizer_StateChanged(object? sender, StateChangedEventArgs e)
        {
            Debug.WriteLine("Synthesizer_StateChanged.  State:" + e.State + ", PreviousState:" + e.PreviousState);
        }

        private void Synthesizer_VoiceChange(object? sender, VoiceChangeEventArgs e)
        {
            Debug.WriteLine("Synthesizer_VoiceChange.  Prompt:" + e.Prompt + ", UserState:" + e.UserState + ", Prompt:" + e.Prompt + ", Voice:" + e.Voice);
        }
    }
#pragma warning restore CS8604 // Null 参照引数の可能性があります。
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning restore CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。
}