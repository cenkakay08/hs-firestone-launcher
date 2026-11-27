# hs-firestone-launcher

**Automatic Hearthstone, Overwolf & Firestone Launcher and Closer for Windows System Tray**

`hs-firestone-launcher` is a lightweight, zero-dependency Windows system tray utility that automatically launches **Overwolf / Firestone** when **Hearthstone** starts, and cleanly terminates all Overwolf background processes the moment Hearthstone exits.

No more leaving Overwolf running in the background eating RAM and CPU after you finish playing Hearthstone.

---

## Key Features

- **Automatic Launch**: Detects when `Hearthstone.exe` starts and automatically launches Overwolf (and Firestone) with a 15-second startup cooldown.
- **Instant Shutdown Detection**: Uses Windows `Process.Exited` events to immediately terminate `Overwolf`, `OverwolfBrowser`, and `OverwolfHelper` as soon as Hearthstone closes.
- **Modern System Tray Flyout**:
  - **High-DPI 64×64 Tray Icon** with a top-right status dot (**Green** = Active, **Gray** = Paused).
  - **Left or Right-click** the tray icon to open the rounded, borderless flyout menu (auto-hides when clicking outside).
  - **iOS-Style Toggle Switches** for **Auto-Sync** and **Run at Windows Startup**.
  - **Dark & Light Mode**: Switch themes anytime via the top-right Sun/Moon icon.
- **12 Languages Built-In**: Automatically matches your Windows system language (defaults to English if unmatched), with a language selector for:
  `English`, `Türkçe`, `Deutsch`, `Français`, `Español`, `Italiano`, `Português`, `Русский`, `Polski`, `日本語`, `한국어`, `中文`.
- **Run at Windows Startup**: Built-in toggle to enable or disable automatic startup with Windows (`HKCU\\...\\Run`, no admin rights required).
- **Single-Instance Protection**: Uses a named `Mutex` so duplicate instances cannot run simultaneously.
- **Zero Dependencies**: Built with native Windows .NET, no external SDKs or installers needed.

---

## Installation & Usage

1. **Download or Clone** this repository:
   ```bash
   git clone https://github.com/cenkakay08/hs-firestone-launcher.git
   ```
2. **Run the Launcher**:
   - Navigate to the repository folder and double‑click `HS-Firestone-Launcher.exe`.
   - The application runs silently in the background with a system‑tray icon.
3. **Control from System Tray**:
   - Click the Hearthstone/Firestone icon in the bottom‑right Windows system tray to:
     - Toggle **Auto‑Sync** ON/OFF
     - Toggle **Run at Windows Startup** ON/OFF
     - Switch between **Dark / Light Mode**
     - Change **Language**
     - **Quit** the application

> **Note:** The legacy `Baslat.vbs` and `HearthstoneMonitor.ps1` scripts have been removed. The launcher is now a compiled `.exe` and does not require PowerShell.

---

## Requirements

- Windows 10 / Windows 11
- [Hearthstone](https://hearthstone.blizzard.com/)
- [Overwolf](https://www.overwolf.com/) & [Firestone](https://www.firestoneapp.com/)

---

## How It Works

- The launcher uses a `ManagementEventWatcher` to monitor process start events for `Hearthstone.exe` (event‑driven, no admin rights needed).
- Once Hearthstone starts, the launcher launches Overwolf and Firestone after a short cooldown.
- `Process.Exited` events are attached to the Hearthstone process for instant cleanup of Overwolf components when Hearthstone exits.

---

## License

MIT