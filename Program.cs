using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace HsFirestoneLauncher
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            bool createdNew;
            using (var mutex = new Mutex(true, "Local\\HearthstoneOverwolfSyncMutex", out createdNew))
            {
                if (!createdNew) return;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new TrayAppContext());
            }
        }
    }

    internal sealed class ToggleSwitch : CheckBox
    {
        public ToggleSwitch(bool initial)
        {
            Appearance = Appearance.Button;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            AutoSize = false;
            Size = new Size(44, 24);
            Checked = initial;
            Cursor = Cursors.Hand;
            Text = "";
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Parent != null ? Parent.BackColor : BackColor);

            Color trackColor = Checked ? Color.FromArgb(46, 204, 113) : Color.FromArgb(108, 112, 134);
            using (var path = new GraphicsPath())
            {
                path.AddArc(1, 1, 21, 21, 90, 180);
                path.AddArc(Width - 23, 1, 21, 21, 270, 180);
                path.CloseFigure();

                using (var trackBrush = new SolidBrush(trackColor))
                    g.FillPath(trackBrush, path);
            }

            int thumbX = Checked ? Width - 21 : 3;
            using (var thumbBrush = new SolidBrush(Color.White))
                g.FillEllipse(thumbBrush, thumbX, 3, 18, 18);
        }
    }

    internal sealed class TrayAppContext : ApplicationContext
    {
        private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string RunValueName = "HearthstoneOverwolfSync";

        private readonly IReadOnlyDictionary<string, LangStrings> _strings;
        private readonly IReadOnlyList<KeyValuePair<string, string>> _languages;
        private readonly Dictionary<string, ToolStripMenuItem> _langMenuItems = new Dictionary<string, ToolStripMenuItem>();

        private string _currentLang;
        private bool _isDark = true;
        private bool _isEnabled = true;
        private bool _hsWasRunning = false;
        private bool _isExiting = false;
        private DateTime _lastOverwolfStart = DateTime.MinValue;
        private Process _monitoredHsProcess = null;
        private readonly string _overwolfExe;

        private readonly Icon _iconOn;
        private readonly Icon _iconOff;
        private readonly NotifyIcon _notifyIcon;
        private readonly ManagementEventWatcher _watcher;

        private readonly Form _form;
        private readonly Label _lblTitle;
        private readonly Label _lblSubtitle;
        private readonly Button _btnTheme;
        private readonly Panel _cardPanel;
        private readonly Label _lblSync;
        private readonly ToggleSwitch _chkSync;
        private readonly Label _lblStartup;
        private readonly ToggleSwitch _chkStartup;
        private readonly Label _lblLangIcon;
        private readonly Button _btnLang;
        private readonly ContextMenuStrip _langMenu;
        private readonly Button _btnExit;

        public TrayAppContext()
        {
            RepairLegacyStartupEntry();
            _overwolfExe = FindOverwolfExe();

            _strings   = Languages.All;
            _languages = Languages.List;

            string sysLang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            _currentLang = _strings.ContainsKey(sysLang) ? sysLang : "en";

            _iconOn = NewStatusIcon(Color.FromArgb(46, 204, 113));
            _iconOff = NewStatusIcon(Color.FromArgb(149, 165, 166));

            _notifyIcon = new NotifyIcon { Icon = _iconOn, Visible = true };
            _notifyIcon.MouseClick += (s, e) =>
            {
                if (_form.Visible) _form.Hide();
                else ShowTrayFlyout();
            };

            _form = new Form
            {
                Size = new Size(360, 234),
                FormBorderStyle = FormBorderStyle.None,
                ShowInTaskbar = false,
                TopMost = true,
                StartPosition = FormStartPosition.Manual,
                AutoScaleMode = AutoScaleMode.Dpi,
                Font = new Font("Segoe UI", 10f),
                Icon = _iconOn
            };
            _form.Deactivate += (s, e) => { if (!_isExiting) _form.Hide(); };

            _lblTitle = new Label { Location = new Point(20, 18), Size = new Size(276, 24), Font = new Font("Segoe UI", 12f, FontStyle.Bold) };
            _lblSubtitle = new Label { Location = new Point(20, 44), Size = new Size(276, 18), Font = new Font("Segoe UI", 8.5f), UseMnemonic = false };

            _btnTheme = new Button { Location = new Point(308, 18), Size = new Size(32, 32), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            _btnTheme.FlatAppearance.BorderSize = 0;
            SetRoundedRegion(_btnTheme, 8);
            _btnTheme.Click += (s, e) => { _isDark = !_isDark; ApplyTheme(); };

            _cardPanel = new Panel { Location = new Point(20, 76), Size = new Size(320, 96) };
            _lblSync = new Label { Location = new Point(16, 16), Size = new Size(220, 24), TextAlign = ContentAlignment.MiddleLeft };
            _chkSync = new ToggleSwitch(true) { Location = new Point(260, 16) };
            _chkSync.CheckedChanged += (s, e) =>
            {
                _isEnabled = _chkSync.Checked;
                if (!_isEnabled) _hsWasRunning = false;
                UpdateSyncVisuals();
            };

            _lblStartup = new Label { Location = new Point(16, 54), Size = new Size(220, 24), TextAlign = ContentAlignment.MiddleLeft };
            _chkStartup = new ToggleSwitch(IsStartupEnabled()) { Location = new Point(260, 54) };
            _chkStartup.CheckedChanged += (s, e) => SetStartupEnabled(_chkStartup.Checked);

            _cardPanel.Controls.AddRange(new Control[] { _lblSync, _chkSync, _lblStartup, _chkStartup });

            _lblLangIcon = new Label
            {
                Location = new Point(20, 190),
                Size = new Size(22, 24),
                Font = new Font("Segoe UI Emoji", 11f),
                Text = "\U0001F310",
                TextAlign = ContentAlignment.MiddleLeft
            };

            _btnLang = new Button
            {
                Location = new Point(48, 188),
                Size = new Size(102, 28),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5f),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            _btnLang.FlatAppearance.BorderSize = 0;

            _langMenu = new ContextMenuStrip { ShowImageMargin = false, Font = new Font("Segoe UI", 9.5f) };
            foreach (var lang in _languages)
            {
                var item = new ToolStripMenuItem(lang.Value) { Tag = lang.Key };
                item.Click += (s, e) =>
                {
                    _currentLang = (string)((ToolStripMenuItem)s).Tag;
                    ApplyLanguage();
                };
                _langMenu.Items.Add(item);
                _langMenuItems[lang.Key] = item;
            }
            _btnLang.Click += (s, e) => _langMenu.Show(_btnLang, new Point(0, _btnLang.Height));

            _btnExit = new Button
            {
                Location = new Point(268, 188),
                Size = new Size(72, 28),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5f),
                Cursor = Cursors.Hand
            };
            _btnExit.FlatAppearance.BorderSize = 0;
            _btnExit.Click += (s, e) => ExitApp();

            _form.Controls.AddRange(new Control[] { _lblTitle, _lblSubtitle, _btnTheme, _cardPanel, _btnExit, _lblLangIcon, _btnLang });
            var unusedHandle = _form.Handle;

            ApplyTheme();
            ApplyLanguage();

            // Halihazırda açıksa kontrol et, ardından WMI EventWatcher başlat
            CheckAndAttachHearthstone();
            _watcher = new ManagementEventWatcher(
                new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Process' AND TargetInstance.Name = 'Hearthstone.exe'")
            );
            _watcher.EventArrived += (s, e) =>
            {
                if (!_isExiting && _form.IsHandleCreated)
                    _form.BeginInvoke((Action)CheckAndAttachHearthstone);
            };
            try { _watcher.Start(); } catch { }
        }

        private void CheckAndAttachHearthstone()
        {
            try
            {
                if (!_isEnabled || _isExiting) return;

                var hearthstone = Process.GetProcessesByName("Hearthstone");
                if (hearthstone.Length > 0)
                {
                    _hsWasRunning = true;
                    if (_monitoredHsProcess == null || _monitoredHsProcess.HasExited)
                    {
                        var hsProc = hearthstone[0];
                        try
                        {
                            hsProc.EnableRaisingEvents = true;
                            hsProc.SynchronizingObject = _form;
                            hsProc.Exited += (s, ev) =>
                            {
                                _monitoredHsProcess = null;
                                if (_isEnabled && !_isExiting && _hsWasRunning)
                                {
                                    _hsWasRunning = false;
                                    StopOverwolfProcesses();
                                }
                            };
                            _monitoredHsProcess = hsProc;
                        }
                        catch { }
                    }

                    bool overwolfRunning = Process.GetProcessesByName("Overwolf").Length > 0;
                    bool cooldownElapsed = (DateTime.Now - _lastOverwolfStart).TotalSeconds >= 15;
                    if (!overwolfRunning && cooldownElapsed && File.Exists(_overwolfExe))
                    {
                        Process.Start(_overwolfExe);
                        _lastOverwolfStart = DateTime.Now;
                    }
                }
            }
            catch { }
        }

        private static string FindOverwolfExe()
        {
            string? programFilesX86 = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            if (string.IsNullOrWhiteSpace(programFilesX86))
                programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            var installDirs = new List<string>();
            if (!string.IsNullOrWhiteSpace(programFilesX86))
                installDirs.Add(Path.Combine(programFilesX86, "Overwolf"));

            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Overwolf"))
                {
                    string? installPath = key?.GetValue("InstallPath") as string;
                    if (!string.IsNullOrWhiteSpace(installPath))
                        installDirs.Add(installPath);
                }
            }
            catch { }

            foreach (string installDir in installDirs.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (File.Exists(installDir) &&
                    string.Equals(Path.GetFileName(installDir), "Overwolf.exe", StringComparison.OrdinalIgnoreCase))
                    return installDir;

                try
                {
                    var exe = Directory.GetFiles(installDir, "Overwolf.exe", SearchOption.AllDirectories)
                        .Select(f => new FileInfo(f))
                        .OrderByDescending(f => f.LastWriteTime)
                        .FirstOrDefault();
                    if (exe != null) return exe.FullName;
                }
                catch { }
            }

            string fallbackDir = string.IsNullOrWhiteSpace(programFilesX86)
                ? @"C:\Program Files (x86)"
                : programFilesX86;
            return Path.Combine(fallbackDir, "Overwolf", "Overwolf.exe");
        }

        private static void StopOverwolfProcesses()
        {
            foreach (string name in new[] { "Overwolf", "OverwolfBrowser", "OverwolfHelper" })
            {
                foreach (var p in Process.GetProcessesByName(name))
                {
                    try { p.Kill(); } catch { }
                }
            }
        }

        private static bool IsStartupEnabled()
        {
            string executablePath = GetStartupExecutablePath();
            using (var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, false))
                return key?.GetValue(RunValueName) is string value &&
                    StartupRegistration.MatchesExecutable(value, executablePath);
        }

        private static void SetStartupEnabled(bool enabled)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, true))
            {
                if (key == null) return;
                if (enabled) key.SetValue(RunValueName, StartupRegistration.BuildCommand(GetStartupExecutablePath()));
                else key.DeleteValue(RunValueName, false);
            }
        }

        private static string GetStartupExecutablePath()
        {
            return Environment.ProcessPath ?? Application.ExecutablePath;
        }

        private static void RepairLegacyStartupEntry()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, true))
            {
                if (key == null) return;
                var value = key.GetValue(RunValueName);
                if (value is string registryValue &&
                    !StartupRegistration.MatchesExecutable(registryValue, GetStartupExecutablePath()))
                {
                    key.SetValue(RunValueName, StartupRegistration.BuildCommand(GetStartupExecutablePath()));
                }
            }
        }

        private void ShowTrayFlyout()
        {
            var pos = Cursor.Position;
            var area = Screen.FromPoint(pos).WorkingArea;
            int x = Math.Min(pos.X - (_form.Width / 2), area.Right - _form.Width - 10);
            x = Math.Max(area.Left + 10, x);
            int y = area.Bottom - _form.Height - 10;
            _form.Location = new Point(x, y);
            _form.Show();
            _form.Activate();
        }

        private void ApplyTheme()
        {
            Color bg = _isDark ? Color.FromArgb(30, 30, 46) : Color.FromArgb(245, 246, 250);
            Color card = _isDark ? Color.FromArgb(42, 42, 60) : Color.FromArgb(255, 255, 255);
            Color fg = _isDark ? Color.FromArgb(248, 248, 242) : Color.FromArgb(30, 30, 46);
            Color sub = _isDark ? Color.FromArgb(166, 173, 200) : Color.FromArgb(87, 96, 111);

            _form.BackColor = bg;
            _form.ForeColor = fg;
            _lblTitle.ForeColor = fg;
            _lblSubtitle.ForeColor = sub;
            _btnTheme.BackColor = card;
            _btnTheme.ForeColor = fg;
            _btnTheme.Image = NewThemeIcon(_isDark, fg);
            _btnExit.BackColor = card;
            _btnExit.ForeColor = Color.FromArgb(231, 76, 60);
            _cardPanel.BackColor = card;
            _lblSync.ForeColor = fg;
            _lblStartup.ForeColor = fg;
            _lblLangIcon.ForeColor = sub;
            _btnLang.BackColor = card;
            _btnLang.ForeColor = fg;
            _langMenu.BackColor = card;
            _langMenu.ForeColor = fg;
            _chkSync.Invalidate();
            _chkStartup.Invalidate();
        }

        private void ApplyLanguage()
        {
            var t = _strings[_currentLang];
            _form.Text = t.Title;
            _lblTitle.Text = t.Title;
            _lblSubtitle.Text = t.Subtitle;
            _lblSync.Text = t.SyncLabel;
            _lblStartup.Text = t.StartupLabel;
            _btnExit.Text = t.MenuExit;

            string langName = _languages.First(l => l.Key == _currentLang).Value;
            _btnLang.Text = langName + "  \u25BE";

            foreach (var lang in _languages)
            {
                var item = _langMenuItems[lang.Key];
                if (lang.Key == _currentLang)
                {
                    item.Text = "\u2713 " + lang.Value;
                    item.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                }
                else
                {
                    item.Text = "   " + lang.Value;
                    item.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
                }
            }

            UpdateLayout();
            UpdateSyncVisuals();
        }

        private void UpdateLayout()
        {
            int wSync = TextRenderer.MeasureText(_lblSync.Text, _lblSync.Font).Width;
            int wStartup = TextRenderer.MeasureText(_lblStartup.Text, _lblStartup.Font).Width;
            int wTitle = TextRenderer.MeasureText(_lblTitle.Text, _lblTitle.Font).Width;
            int wSub = TextRenderer.MeasureText(_lblSubtitle.Text, _lblSubtitle.Font).Width;
            int wBtnLang = TextRenderer.MeasureText(_btnLang.Text, _btnLang.Font).Width + 24;

            int maxRowW = Math.Max(wSync, wStartup) + 44 + 48;
            int maxHeadW = Math.Max(wTitle, wSub) + 48;
            int minFooterW = 28 + wBtnLang + 32 + 72;
            int cardW = Math.Max(250, Math.Max(Math.Max(maxRowW, maxHeadW), minFooterW));
            int formW = cardW + 40;

            _form.Size = new Size(formW, 234);
            SetRoundedRegion(_form, 14);

            _lblTitle.Width = cardW - 44;
            _lblSubtitle.Width = cardW - 44;
            _btnTheme.Location = new Point(formW - 52, 18);
            _btnTheme.BringToFront();

            _cardPanel.Size = new Size(cardW, 96);
            SetRoundedRegion(_cardPanel, 10);

            _lblSync.Width = cardW - 80;
            _lblStartup.Width = cardW - 80;
            _chkSync.Location = new Point(cardW - 60, 16);
            _chkStartup.Location = new Point(cardW - 60, 54);

            _lblLangIcon.Location = new Point(20, 190);
            _btnLang.Location = new Point(48, 188);
            _btnLang.Size = new Size(wBtnLang, 28);
            SetRoundedRegion(_btnLang, 8);

            _btnExit.Location = new Point(formW - 92, 188);
            _btnExit.Size = new Size(72, 28);
            SetRoundedRegion(_btnExit, 8);
        }

        private void UpdateSyncVisuals()
        {
            var t = _strings[_currentLang];
            _notifyIcon.Icon = _isEnabled ? _iconOn : _iconOff;
            _notifyIcon.Text = _isEnabled ? t.TrayOn : t.TrayOff;
            _chkSync.Invalidate();
        }

        private void ExitApp()
        {
            _isExiting = true;
            if (_watcher != null)
            {
                try { _watcher.Stop(); } catch { }
                _watcher.Dispose();
            }
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            _iconOn.Dispose();
            _iconOff.Dispose();
            _form.Close();
            Application.Exit();
        }

        private static void SetRoundedRegion(Control control, int radius)
        {
            int w = control.Width;
            int h = control.Height;
            int d = radius * 2;
            using (var path = new GraphicsPath())
            {
                path.AddArc(0, 0, d, d, 180, 90);
                path.AddArc(w - d, 0, d, d, 270, 90);
                path.AddArc(w - d, h - d, d, d, 0, 90);
                path.AddArc(0, h - d, d, d, 90, 90);
                path.CloseFigure();
                control.Region = new Region(path);
            }
        }

        private static Icon NewStatusIcon(Color statusColor)
        {
            using (var bmp = new Bitmap(64, 64))
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.Clear(Color.Transparent);

                using (var bgBrush = new SolidBrush(Color.FromArgb(52, 73, 94)))
                    g.FillEllipse(bgBrush, 0, 4, 58, 58);

                using (var innerBrush = new SolidBrush(Color.FromArgb(93, 173, 226)))
                    g.FillPolygon(innerBrush, new[] { new Point(29, 14), new Point(45, 33), new Point(29, 52), new Point(13, 33) });

                using (var statusBrush = new SolidBrush(statusColor))
                    g.FillEllipse(statusBrush, 38, 0, 24, 24);

                using (var pen = new Pen(Color.White, 4))
                    g.DrawEllipse(pen, 38, 0, 24, 24);

                return Icon.FromHandle(bmp.GetHicon());
            }
        }

        private static Bitmap NewThemeIcon(bool isSun, Color color)
        {
            var bmp = new Bitmap(20, 20);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.Clear(Color.Transparent);

                using (var pen = new Pen(color, 1.8f))
                using (var brush = new SolidBrush(color))
                {
                    if (isSun)
                    {
                        g.DrawEllipse(pen, 6, 6, 8, 8);
                        for (int i = 0; i < 8; i++)
                        {
                            double ang = i * Math.PI / 4;
                            g.DrawLine(pen,
                                (float)(10 + Math.Cos(ang) * 6.2), (float)(10 + Math.Sin(ang) * 6.2),
                                (float)(10 + Math.Cos(ang) * 8.4), (float)(10 + Math.Sin(ang) * 8.4));
                        }
                    }
                    else
                    {
                        g.FillEllipse(brush, 4, 4, 12, 12);
                        g.CompositingMode = CompositingMode.SourceCopy;
                        using (var trans = new SolidBrush(Color.Transparent))
                            g.FillEllipse(trans, 8, 2, 10, 10);
                    }
                }
            }
            return bmp;
        }
    }
}
