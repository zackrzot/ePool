using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;

namespace ePool
{
    public class SpeechRecognizer : IDisposable
    {
        private readonly Dictionary<string, Commands> gameplayPhrases = new Dictionary<string, Commands>
            {
                { "Front Right", Commands.FrontRight },
                { "Front Left", Commands.FrontLeft },
                { "Center Right", Commands.CenterRight },
                { "Center Left", Commands.CenterLeft },
                { "Back Right",  Commands.BackRight },
                { "Back Left", Commands.BackLeft },
                { "Break", Commands.Break },
                { "Lights Off", Commands.LightsOff },
                { "Strobe On", Commands.StrobeOn },
                { "Strobe Off", Commands.StrobeOff }
            };

        private SpeechRecognitionEngine sre;
        private KinectAudioSource kinectAudioSource;
        private bool isDisposed;

        private SpeechRecognizer()
        {
            RecognizerInfo ri = GetKinectRecognizer();
            this.sre = new SpeechRecognitionEngine(ri);
            this.LoadGrammar(this.sre);
        }

        public enum Commands
        {
            None = 0,
            FrontRight,
            FrontLeft,
            CenterRight,
            CenterLeft,
            BackRight,
            BackLeft,
            Break,
            LightsOff,
            StrobeOn,
            StrobeOff
        }

        public EchoCancellationMode EchoCancellationMode
        {
            get
            {
                this.CheckDisposed();

                if (this.kinectAudioSource == null)
                    return EchoCancellationMode.None;

                return this.kinectAudioSource.EchoCancellationMode;
            }
            set
            {
                this.CheckDisposed();

                if (this.kinectAudioSource != null)
                    this.kinectAudioSource.EchoCancellationMode = value;
            }
        }

        // This method exists so that it can be easily called and return safely if the speech prereqs aren't installed.
        // We isolate the try/catch inside this class, and don't impose the need on the caller.
        public static SpeechRecognizer Create()
        {
            SpeechRecognizer recognizer = null;

            try
            {
                recognizer = new SpeechRecognizer();
            }
            catch (Exception ex)
            {
                Logger.Log("VR pre-rec not available: " + ex.ToString());
            }

            return recognizer;
        }

        public void Start(KinectAudioSource kinectSource)
        {
            this.CheckDisposed();

            if (kinectSource != null)
            {
                this.kinectAudioSource = kinectSource;
                this.kinectAudioSource.AutomaticGainControlEnabled = false;
                this.kinectAudioSource.BeamAngleMode = BeamAngleMode.Adaptive;
                var kinectStream = this.kinectAudioSource.Start();
                this.sre.SetInputToAudioStream(
                    kinectStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                this.sre.RecognizeAsync(RecognizeMode.Multiple);
            }
        }

        public void Stop()
        {
            this.CheckDisposed();

            if (this.sre != null)
            {
                if (this.kinectAudioSource != null)
                {
                    this.kinectAudioSource.Stop();
                }

                this.sre.RecognizeAsyncCancel();
                this.sre.RecognizeAsyncStop();

                this.sre.SpeechRecognized -= this.SreSpeechRecognized;
                this.sre.SpeechHypothesized -= this.SreSpeechHypothesized;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "sre",
            Justification = "This is suppressed because FXCop does not see our threaded dispose.")]
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Stop();

                if (this.sre != null)
                {
                    // NOTE: The SpeechRecognitionEngine can take a long time to dispose
                    // so we will dispose it on a background thread
                    ThreadPool.QueueUserWorkItem(
                        delegate (object state)
                        {
                            IDisposable toDispose = state as IDisposable;
                            if (toDispose != null)
                            {
                                toDispose.Dispose();
                            }
                        },
                            this.sre);
                    this.sre = null;
                }

                this.isDisposed = true;
            }
        }

        private static RecognizerInfo GetKinectRecognizer()
        {
            Func<RecognizerInfo, bool> matchingFunc = r =>
            {
                string value;
                r.AdditionalInfo.TryGetValue("Kinect", out value);
                return "True".Equals(value, StringComparison.InvariantCultureIgnoreCase) && "en-US".Equals(r.Culture.Name, StringComparison.InvariantCultureIgnoreCase);
            };
            return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();
        }

        private void CheckDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException("SpeechRecognizer");
            }
        }

        private void LoadGrammar(SpeechRecognitionEngine speechRecognitionEngine)
        {
            // Build a simple grammar of shapes, colors, and some simple program control
            var single = new Choices();
            foreach (var phrase in this.gameplayPhrases)
            {
                single.Add(phrase.Key);
            }

            var allChoices = new Choices();
            allChoices.Add(single);

            // This is needed to ensure that it will work on machines with any culture, not just en-us.
            var gb = new GrammarBuilder { Culture = speechRecognitionEngine.RecognizerInfo.Culture };
            gb.Append(allChoices);

            var g = new Grammar(gb);
            speechRecognitionEngine.LoadGrammar(g);
            speechRecognitionEngine.SpeechRecognized += this.SreSpeechRecognized;
            speechRecognitionEngine.SpeechHypothesized += this.SreSpeechHypothesized;
        }

        private void SreSpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            Logger.Log("Speech Hypothesized:" + e.Result.Text);
        }

        private void SreSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Logger.Log("Speech Recognized: " + e.Result.Text);

            if (e.Result.Confidence < 0.3)
            {
                Logger.Log("Confidence too low: " + e.Result.Confidence.ToString());
                return;
            }

            Commands said = Commands.None;

            foreach (var kvp in this.gameplayPhrases)
            {
                if (kvp.Key == e.Result.Text)
                {
                    Logger.Log("Doing VR Action: " + said.ToString());
                    said = kvp.Value;
                }
            }

            if (said == Commands.FrontRight)
                Presets.SetFR();
            if (said == Commands.FrontLeft)
                Presets.SetFL();
            if (said == Commands.CenterRight)
                Presets.SetCR();
            if (said == Commands.CenterLeft)
                Presets.SetCL();
            if (said == Commands.BackRight)
                Presets.SetBR();
            if (said == Commands.BackLeft)
                Presets.SetBL();
            if (said == Commands.Break)
                Presets.SetBreak();
            if (said == Commands.LightsOff)
                Presets.SetOff();
            if (said == Commands.StrobeOn)
            {
                LightThread.Light1.StrobeVal = LightCtrl.Strobe.ON;
                LightThread.Light2.StrobeVal = LightCtrl.Strobe.ON;
            }
            if (said == Commands.StrobeOff)
            {
                LightThread.Light1.StrobeVal = LightCtrl.Strobe.OPEN;
                LightThread.Light2.StrobeVal = LightCtrl.Strobe.OPEN;
            }
        }

        public class SaidSomethingEventArgs : EventArgs
        {
            public Commands Command { get; set; }

            public string Phrase { get; set; }

            public string Matched { get; set; }
        }
    }
}
