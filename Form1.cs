using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Text;

namespace YMH219_SpeechApp
{
    public partial class Form1 : Form
    {
        private readonly SpeechSynthesizer _synthesizer = new();
        private SpeechRecognitionEngine? _recognizer;
        private readonly RichTextBox _speechText = new();
        private readonly RichTextBox _recognizedText = new();
        private readonly Label _statusLabel = new();
        private readonly Button _speakButton = new();
        private readonly Button _pauseButton = new();
        private readonly Button _stopSpeakingButton = new();
        private readonly Button _clearSpeechButton = new();
        private readonly Button _listenButton = new();
        private readonly Button _stopListeningButton = new();
        private readonly Button _clearRecognizedButton = new();
        private readonly Button _saveRecognizedButton = new();
        private bool _isPaused;
        private bool _isListening;

        public Form1()
        {
            InitializeComponent();
            BuildInterface();
            _synthesizer.SpeakCompleted += Synthesizer_SpeakCompleted;
            FormClosing += Form1_FormClosing;
        }

        private void BuildInterface()
        {
            Text = "Ses ve Metin Dönüştürücü";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(850, 600);
            ClientSize = new Size(1100, 740);
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 10F);
            DoubleBuffered = true;

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(28, 22, 28, 18),
                ColumnCount = 1,
                RowCount = 3,
                BackColor = BackColor
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 96));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
            Controls.Add(root);

            var heading = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = BackColor,
                Margin = new Padding(0)
            };
            heading.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
            heading.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            heading.Controls.Add(new Label
            {
                Text = "Ses ve Metin Dönüştürücü",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 48, 71),
                TextAlign = ContentAlignment.BottomLeft,
                Margin = new Padding(0)
            }, 0, 0);
            heading.Controls.Add(new Label
            {
                Text = "Metni dinleyin veya mikrofonla konuşup metne dönüştürün.",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(103, 119, 138),
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0)
            }, 0, 1);
            root.Controls.Add(heading, 0, 0);

            var panels = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = BackColor
            };
            panels.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            panels.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            root.Controls.Add(panels, 0, 1);

            panels.Controls.Add(CreateTextToSpeechPanel(), 0, 0);
            panels.Controls.Add(CreateSpeechToTextPanel(), 1, 0);

            _statusLabel.Text = "●  Hazır. Bir işlem başlatın.";
            _statusLabel.Dock = DockStyle.Fill;
            _statusLabel.TextAlign = ContentAlignment.MiddleLeft;
            _statusLabel.ForeColor = Color.FromArgb(65, 105, 75);
            _statusLabel.BackColor = Color.FromArgb(232, 245, 236);
            _statusLabel.Padding = new Padding(14, 0, 14, 0);
            _statusLabel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            root.Controls.Add(_statusLabel, 0, 2);
        }

        private Control CreateTextToSpeechPanel()
        {
            var panel = CreateSection("METİNDEN SESE");
            var content = (TableLayoutPanel)panel.Controls[0];

            _speechText.Dock = DockStyle.Fill;
            _speechText.BorderStyle = BorderStyle.None;
            _speechText.Font = new Font("Segoe UI", 11F);
            _speechText.Text = string.Empty;
            _speechText.BackColor = Color.FromArgb(249, 251, 253);
            _speechText.ForeColor = Color.FromArgb(39, 54, 72);
            _speechText.Margin = new Padding(0, 6, 0, 0);
            content.Controls.Add(_speechText, 0, 2);

            var buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(0, 12, 0, 0),
                Margin = new Padding(0)
            };
            ConfigureButton(_speakButton, "▶ Başlat", Color.FromArgb(32, 114, 190));
            ConfigureButton(_pauseButton, "Ⅱ Duraklat", Color.FromArgb(81, 99, 120));
            ConfigureButton(_stopSpeakingButton, "■ Durdur", Color.FromArgb(183, 65, 65));
            ConfigureButton(_clearSpeechButton, "Temizle", Color.FromArgb(81, 99, 120));
            _speakButton.Click += SpeakButton_Click;
            _pauseButton.Click += PauseButton_Click;
            _stopSpeakingButton.Click += StopSpeakingButton_Click;
            _clearSpeechButton.Click += (_, _) => _speechText.Clear();
            buttons.Controls.AddRange(new Control[] { _speakButton, _pauseButton, _stopSpeakingButton, _clearSpeechButton });
            content.Controls.Add(buttons, 0, 3);
            return panel;
        }

        private Control CreateSpeechToTextPanel()
        {
            var panel = CreateSection("SESTEN METNE");
            var content = (TableLayoutPanel)panel.Controls[0];

            _recognizedText.Dock = DockStyle.Fill;
            _recognizedText.BorderStyle = BorderStyle.None;
            _recognizedText.Font = new Font("Segoe UI", 11F);
            _recognizedText.ReadOnly = true;
            _recognizedText.BackColor = Color.FromArgb(249, 251, 253);
            _recognizedText.ForeColor = Color.FromArgb(39, 54, 72);
            _recognizedText.Margin = new Padding(0, 6, 0, 0);
            content.Controls.Add(_recognizedText, 0, 2);

            var buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(0, 12, 0, 0),
                Margin = new Padding(0)
            };
            ConfigureButton(_listenButton, "🎙 Dinlemeyi Başlat", Color.FromArgb(32, 114, 190));
            ConfigureButton(_stopListeningButton, "■ Dinlemeyi Durdur", Color.FromArgb(183, 65, 65));
            ConfigureButton(_saveRecognizedButton, "TXT Kaydet", Color.FromArgb(38, 133, 98));
            ConfigureButton(_clearRecognizedButton, "Temizle", Color.FromArgb(81, 99, 120));
            _listenButton.Click += ListenButton_Click;
            _stopListeningButton.Click += StopListeningButton_Click;
            _saveRecognizedButton.Click += SaveRecognizedButton_Click;
            _clearRecognizedButton.Click += (_, _) => _recognizedText.Clear();
            buttons.Controls.AddRange(new Control[] { _listenButton, _stopListeningButton, _saveRecognizedButton, _clearRecognizedButton });
            content.Controls.Add(buttons, 0, 3);
            return panel;
        }

        private void SaveRecognizedButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_recognizedText.Text))
            {
                ShowError("Kaydedilecek bir konuşma metni bulunmuyor.");
                return;
            }

            using var dialog = new SaveFileDialog
            {
                Title = "Konuşma metnini kaydet",
                Filter = "Metin dosyası (*.txt)|*.txt",
                DefaultExt = "txt",
                AddExtension = true,
                FileName = "konusma-metni.txt",
                OverwritePrompt = true
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
                return;

            try
            {
                File.WriteAllText(dialog.FileName, _recognizedText.Text, new UTF8Encoding(true));
                SetStatus("Konuşma metni TXT dosyasına kaydedildi.");
            }
            catch (Exception ex)
            {
                SetStatus("Metin dosyası kaydedilemedi.");
                ShowError($"Dosya kaydedilemedi: {ex.Message}");
            }
        }

        private static Panel CreateSection(string title)
        {
            var section = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(18),
                Margin = new Padding(8, 4, 8, 8)
            };
            section.Paint += (_, e) => ControlPaint.DrawBorder(e.Graphics, section.ClientRectangle,
                Color.FromArgb(225, 231, 238), ButtonBorderStyle.Solid);
            var content = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = Color.White
            };
            content.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            content.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            content.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            content.RowStyles.Add(new RowStyle(SizeType.Absolute, 108));
            var heading = new Label
            {
                Text = title,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 48, 71),
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0)
            };
            var description = new Label
            {
                Text = title == "METİNDEN SESE"
                    ? "Metninizi yazın ve sesli okumayı kontrol edin."
                    : "Mikrofonu açın, konuşmanız metin olarak eklensin.",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(112, 127, 144),
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0)
            };
            content.Controls.Add(heading, 0, 0);
            content.Controls.Add(description, 0, 1);
            section.Controls.Add(content);
            return section;
        }

        private static void ConfigureButton(Button button, string text, Color color)
        {
            button.Text = text;
            button.AutoSize = true;
            button.Height = 40;
            button.Padding = new Padding(10, 0, 10, 0);
            button.Margin = new Padding(0, 0, 8, 6);
            button.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = ControlPaint.Light(color, 0.12F);
            button.BackColor = color;
            button.ForeColor = Color.White;
            button.Cursor = Cursors.Hand;
        }

        private void SpeakButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_speechText.Text))
            {
                ShowError("Lütfen okutulacak bir metin girin.");
                return;
            }

            try
            {
                if (_isPaused)
                {
                    _synthesizer.Resume();
                    _isPaused = false;
                    SetStatus("Metin okuma devam ediyor.");
                }
                else
                {
                    _synthesizer.SpeakAsync(_speechText.Text);
                    SetStatus("Metin sesli olarak okunuyor.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Metin seslendirilemedi: {ex.Message}");
            }
        }

        private void PauseButton_Click(object? sender, EventArgs e)
        {
            try
            {
                _synthesizer.Pause();
                _isPaused = true;
                SetStatus("Metin okuma duraklatıldı. Devam etmek için Başlat'a basın.");
            }
            catch (Exception ex)
            {
                ShowError($"Okuma duraklatılamadı: {ex.Message}");
            }
        }

        private void StopSpeakingButton_Click(object? sender, EventArgs e)
        {
            _synthesizer.SpeakAsyncCancelAll();
            _isPaused = false;
            SetStatus("Metin okuma durduruldu.");
        }

        private void ListenButton_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_recognizer is null)
                {
                    _recognizer = new SpeechRecognitionEngine();
                    _recognizer.SetInputToDefaultAudioDevice();
                    _recognizer.LoadGrammar(new DictationGrammar());
                    _recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
                    _recognizer.RecognizeCompleted += Recognizer_RecognizeCompleted;
                }

                _recognizer.RecognizeAsync(RecognizeMode.Multiple);
                _isListening = true;
                SetStatus("Mikrofon etkin; konuşmanız dinleniyor.");
            }
            catch (Exception ex)
            {
                _isListening = false;
                SetStatus("Konuşma tanıma başlatılamadı.");
                ShowError($"Mikrofon veya konuşma tanıma hizmetine erişilemiyor. Windows'ta mikrofon iznini ve konuşma dil paketini kontrol edin. Ayrıntı: {ex.Message}");
                DisposeRecognizer();
            }
        }

        private void StopListeningButton_Click(object? sender, EventArgs e)
        {
            if (_recognizer is null || !_isListening)
            {
                SetStatus("Mikrofon dinlemesi zaten kapalı.");
                return;
            }

            try
            {
                _recognizer.RecognizeAsyncStop();
                _isListening = false;
                SetStatus("Mikrofon dinlemesi durduruldu.");
            }
            catch (Exception ex)
            {
                ShowError($"Mikrofon dinlemesi durdurulamadı: {ex.Message}");
            }
        }

        private void Recognizer_SpeechRecognized(object? sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result is null || e.Result.Confidence < 0.35F)
                return;

            RunOnUiThread(() =>
            {
                _recognizedText.AppendText($"{DateTime.Now:HH:mm:ss}  {e.Result.Text}{Environment.NewLine}");
                SetStatus("Konuşma metne dönüştürüldü; mikrofon dinlemeye devam ediyor.");
            });
        }

        private void Recognizer_RecognizeCompleted(object? sender, RecognizeCompletedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                _isListening = false;
                if (e.Error is not null)
                {
                    SetStatus("Konuşma tanıma durdu.");
                    ShowError($"Konuşma tanıma sırasında bir hata oluştu: {e.Error.Message}");
                }
            });
        }

        private void Synthesizer_SpeakCompleted(object? sender, SpeakCompletedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                _isPaused = false;
                if (e.Error is not null)
                    ShowError($"Sesli okuma sırasında bir hata oluştu: {e.Error.Message}");
                else if (!e.Cancelled)
                    SetStatus("Metin okuma tamamlandı.");
            });
        }

        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            _synthesizer.SpeakAsyncCancelAll();
            _synthesizer.Dispose();
            DisposeRecognizer();
        }

        private void DisposeRecognizer()
        {
            if (_recognizer is null)
                return;

            _recognizer.SpeechRecognized -= Recognizer_SpeechRecognized;
            _recognizer.RecognizeCompleted -= Recognizer_RecognizeCompleted;
            _recognizer.Dispose();
            _recognizer = null;
            _isListening = false;
        }

        private void ShowError(string message)
        {
            MessageBox.Show(this, message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void SetStatus(string message)
        {
            _statusLabel.Text = $"●  {message}";
            if (message.Contains("hata", StringComparison.OrdinalIgnoreCase)
                || message.Contains("başlatılamadı", StringComparison.OrdinalIgnoreCase)
                || message.Contains("durdurulamadı", StringComparison.OrdinalIgnoreCase))
            {
                _statusLabel.ForeColor = Color.FromArgb(155, 55, 55);
                _statusLabel.BackColor = Color.FromArgb(252, 235, 235);
            }
            else if (message.Contains("dinleniyor", StringComparison.OrdinalIgnoreCase)
                || message.Contains("okunuyor", StringComparison.OrdinalIgnoreCase)
                || message.Contains("devam ediyor", StringComparison.OrdinalIgnoreCase))
            {
                _statusLabel.ForeColor = Color.FromArgb(38, 91, 150);
                _statusLabel.BackColor = Color.FromArgb(231, 241, 252);
            }
            else
            {
                _statusLabel.ForeColor = Color.FromArgb(65, 105, 75);
                _statusLabel.BackColor = Color.FromArgb(232, 245, 236);
            }
        }

        private void RunOnUiThread(Action action)
        {
            if (IsDisposed || !IsHandleCreated)
                return;

            if (InvokeRequired)
                BeginInvoke(action);
            else
                action();
        }
    }
}
