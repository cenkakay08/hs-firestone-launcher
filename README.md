# hs-firestone-launcher

**Automatic Hearthstone, Overwolf & Firestone Launcher and Closer for Windows System Tray**

`hs-firestone-launcher` is a lightweight, zero-dependency Windows system tray utility that automatically launches **Overwolf / Firestone** when **Hearthstone** starts, and cleanly terminates all Overwolf background processes the moment Hearthstone exits.

No more leaving Overwolf running in the background eating RAM and CPU after you finish playing Hearthstone.

---

## Key Features

- **Automatic Launch**: Detects when `Hearthstone.exe` starts and automatically launches Overwolf (and Firestone) with a 15-second startup cooldown.
- **Instant Shutdown Detection**: Uses Windows `Process.Exited` events to immediately terminate `Overwolf`, `OverwolfBrowser`, and `OverwolfHelper` as soon as Hearthstone closes.
- **Modern System Tray Flyout**:
  - **High-DPI 32×32 Tray Icon** with a top-right status dot (**Green** = Active, **Gray** = Paused).
  - **Left or Right-click** the tray icon to open the rounded, borderless flyout menu (auto-hides when clicking outside).
  - **iOS-Style Toggle Switches** for **Auto-Sync** and **Run at Windows Startup**.
  - **Dark & Light Mode**: Switch themes anytime via the top-right Sun/Moon icon.
- **12 Languages Built-In**: Automatically matches your Windows system language (defaults to English if unmatched), with a language selector for:
  `English`, `Türkçe`, `Deutsch`, `Français`, `Español`, `Italiano`, `Português`, `Русский`, `Polski`, `日本語`, `한국어`, `中文`.
- **Run at Windows Startup**: Built-in toggle to enable or disable automatic startup with Windows (`HKCU\...\Run`, no admin rights required).
- **Single-Instance Protection**: Uses a named `Mutex` so duplicate instances cannot run simultaneously.
- **Zero Dependencies**: Built with native Windows PowerShell, WinForms, and VBScript—no `.NET` SDK, Python, or third-party installers needed.

---

## Installation & Usage

1. **Download or Clone** this repository:
   ```bash
   git clone https://github.com/<your-username>/hs-firestone-launcher.git
   ```
2. **Run the Launcher**:
   Double-click `Baslat.vbs` to start the app silently in the background (no console window).
3. **Control from System Tray**:
   Click the Hearthstone/Firestone icon in the bottom-right Windows system tray to:
   - Toggle **Auto-Sync** ON / OFF
   - Toggle **Run at Windows Startup** ON / OFF
   - Switch between **Dark / Light Mode**
   - Change **Language**
   - **Quit** the application

---

## Requirements

- Windows 10 / Windows 11
- [Hearthstone](https://hearthstone.blizzard.com/)
- [Overwolf](https://www.overwolf.com/) & [Firestone](https://www.firestoneapp.com/)

---

## How It Works

- `Baslat.vbs` launches `HearthstoneMonitor.ps1` hidden in STA mode (`-STA -WindowStyle Hidden`).
- A non-blocking WinForms `Timer` checks for `Hearthstone.exe` every 3 seconds and launches Overwolf if it is not already running.
- Once Hearthstone is detected, `Process.Exited` is attached directly to the Hearthstone process for instant cleanup of `Overwolf`, `OverwolfBrowser`, and `OverwolfHelper` upon exit.

---

## License

MIT
