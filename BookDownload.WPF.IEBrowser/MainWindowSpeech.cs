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
                datacontext.FileTempPath = (registryKey.GetValue("FileTempPath") as string) ?? "";
                datacontext.FileSavePath = (registryKey.GetValue("FileSavePath") as string) ?? "";
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

        static string GetVoiceInfoDesc(VoiceInfo vi)
        {
            return vi.Culture.Name + " - " + vi.AdditionalInfo["Name"] + " (ID = " + vi.AdditionalInfo["Language"] + ", " + vi.AdditionalInfo["Age"] + ", " + vi.AdditionalInfo["Gender"] + ", " + vi.Culture + ")";
        }

        private void OnConvertToMp3(object sender, RoutedEventArgs e)
        {
            //SpeechSynthesizer mp3Synthesizer = InitAndHookSpeech(null);
            //LameMP3FileWriter mp3 = new LameMP3FileWriter("testoutput1.mp3", new WaveFormat(22050, 16, 1), LAMEPreset.VBR_90);
            //WaveFileWriter wave = new WaveFileWriter(mp3, new WaveFormat(22050, 16, 1));
            //mp3Synthesizer.SetOutputToWaveStream(wave);

            //PromptBuilder builder = new PromptBuilder();
            //if (cmbInstalledVoice.SelectedIndex == -1)
            //    builder.StartVoice(new CultureInfo("zh-Hans"));
            //else
            //{
            //    string strCul = cmbInstalledVoice.Text;
            //    builder.StartVoice(new CultureInfo(strCul.Substring(0, strCul.IndexOf(" - "))));
            //}

            //string strText = txtAnalysizedContents.SelectedText.Trim(new char[] { ' ', '\t', '\n', '\r' });
            //if (!string.IsNullOrEmpty(strText) && strText.Length > 4)
            //{
            //    nShift = txtAnalysizedContents.SelectionStart;
            //}
            //else
            //{
            //    nShift = 0; strText = "";
            //}

            //builder.AppendText(string.IsNullOrEmpty(strText) ? txtAnalysizedContents.Text : strText);
            //builder.EndVoice();

            //nShift4Errr = -1;
            //new Thread(() =>
            //{
            //    try
            //    {
            //        mp3Synthesizer.Speak(builder);
            //    }
            //    catch (Exception)
            //    {
            //    }

            //    mp3.Close();
            //    nShift4Errr = -1;
            //    bSpeaking = false;
            //    threadSpeech = null;

            //}).Start();
            OnConvertToMp3_MemoryStream(sender, e);
        }

        private void OnConvertToMp3_MemoryStream(object sender, RoutedEventArgs e)
        {
            SpeechSynthesizer mp3Synthesizer = new SpeechSynthesizer();
            mp3Synthesizer.Volume = 100;
            mp3Synthesizer.Rate = 0;
            MemoryStream ms = new MemoryStream();
            mp3Synthesizer.SetOutputToWaveStream(ms);

            PromptBuilder builder = new PromptBuilder();
            if (cmbInstalledVoice.SelectedIndex == -1)
                builder.StartVoice(new CultureInfo("zh-Hans"));
            else
            {
                string strCul = cmbInstalledVoice.Text;
                builder.StartVoice(new CultureInfo(strCul.Substring(0, strCul.IndexOf(" - "))));
            }

            string strText = txtAnalysizedContents.SelectedText.Trim(new char[] { ' ', '\t', '\n', '\r' });
            if (!string.IsNullOrEmpty(strText) && strText.Length > 4)
            {
                nShift = txtAnalysizedContents.SelectionStart;
            }
            else
            {
                nShift = 0; strText = "";
            }

            builder.AppendText(string.IsNullOrEmpty(strText) ? txtAnalysizedContents.Text : strText);
            builder.EndVoice();

            nShift4Errr = -1;
            new Thread(() =>
            {
                try
                {
                    mp3Synthesizer.Speak(builder);
                }
                catch (Exception)
                {
                }
                ms.Seek(0, SeekOrigin.Begin);
                using (var rdr = new WaveFileReader(ms))
                using (var wtr = new LameMP3FileWriter("testoutput.mp3", rdr.WaveFormat, LAMEPreset.VBR_90))
                {
                    rdr.CopyTo(wtr);
                }

                nShift4Errr = -1;
                bSpeaking = false;
                threadSpeech = null;

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
                    btnSpeech.Content = "Speech Play";
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
                    
                    btnSpeech.Content = "Speech Stop";
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
                        this.Dispatcher.Invoke(() => { btnSpeech.Content = "Speech Play"; });
                        
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
            //Debug.WriteLine("Synthesizer_SpeakProgress(bSpeaking:" + bSpeaking + ", Text:" + e.Text +
            //    ", CharacterPosition:" + (nShift4Errr == -1 ? e.CharacterPosition + nShift : e.CharacterPosition - nShift4Errr + nShift) +
            //    ", CharacterCount:" + e.CharacterCount +
            //    ", Cancelled:" + e.Cancelled +
            //    ")");
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
}