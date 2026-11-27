# HS-Firestone Launcher

> **Automatic Overwolf & Firestone manager for Hearthstone on Windows** — zero config, lives in your system tray.

[![Build & Release](https://github.com/cenkakay08/hs-firestone-launcher/actions/workflows/release.yml/badge.svg)](https://github.com/cenkakay08/hs-firestone-launcher/actions/workflows/release.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/platform-Windows%2010%2F11-informational)](https://github.com/cenkakay08/hs-firestone-launcher/releases/latest)
[![Latest Release](https://img.shields.io/github/v/release/cenkakay08/hs-firestone-launcher)](https://github.com/cenkakay08/hs-firestone-launcher/releases/latest)

**HS-Firestone Launcher** is a lightweight Windows system tray utility that automatically launches **Overwolf** and **Firestone** the moment **Hearthstone** starts, and cleanly kills all Overwolf background processes the moment Hearthstone exits — no manual intervention, no leftover RAM or CPU usage.

---

## ✨ Features

| Feature | Detail |
|---|---|
| 🚀 **Auto-Launch** | Detects `Hearthstone.exe` via WMI event watcher and starts Overwolf instantly |
| ⚡ **Instant Cleanup** | Kills `Overwolf`, `OverwolfBrowser`, `OverwolfHelper` the moment Hearthstone exits |
| 🔔 **System Tray Flyout** | Borderless, rounded flyout — click tray icon to open/close |
| 🌗 **Dark & Light Mode** | Toggle anytime with the Sun/Moon button |
| 🌍 **12 Languages** | Auto-detects Windows system language; manually switchable |
| 🔁 **Run at Startup** | One-click Windows startup toggle (no admin rights required) |
| 🛡️ **Single Instance** | Named `Mutex` prevents duplicate processes |
| 📦 **No Installer** | Single `.exe`, no setup wizard, no registry bloat |

### Supported Languages
`English` · `Türkçe` · `Deutsch` · `Français` · `Español` · `Italiano` · `Português` · `Русский` · `Polski` · `日本語` · `한국어` · `中文`

---

## 📥 Download & Install

1. Go to [**Releases**](https://github.com/cenkakay08/hs-firestone-launcher/releases/latest) and download `HsFirestoneLauncher.exe`
2. Place it anywhere (e.g. `C:\Tools\`)
3. Double-click to run — it goes straight to the system tray

> **Requirements:** Windows 10 / 11 · [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) · [Hearthstone](https://hearthstone.blizzard.com/) · [Overwolf](https://www.overwolf.com/) + [Firestone](https://www.firestoneapp.gg/)

---

## 🖥️ Usage

| Action | How |
|---|---|
| Open settings flyout | Left or right-click the tray icon |
| Toggle auto-sync | Flip the **Auto-Sync** switch in the flyout |
| Toggle Windows startup | Flip the **Run at Startup** switch |
| Switch dark / light mode | Click the ☀️/🌙 button (top-right of flyout) |
| Change language | Click the 🌐 globe button (bottom-left of flyout) |
| Quit | Click **Quit** in the flyout |

---

## ⚙️ How It Works

```
Hearthstone.exe starts
        │
        ▼
  WMI EventWatcher fires (no polling, event-driven)
        │
        ▼
  Overwolf.exe launched (15s cooldown prevents double-launch)
        │
        ▼
  Process.Exited hook attached to Hearthstone
        │
        ▼
  Hearthstone closes → Overwolf + OverwolfBrowser + OverwolfHelper killed
```

- Uses `ManagementEventWatcher` with a WQL query — **no CPU polling**
- Overwolf path is auto-discovered from `C:\Program Files (x86)\Overwolf\` (picks newest version)
- Startup registry key: `HKCU\Software\Microsoft\Windows\CurrentVersion\Run` — **no admin rights needed**

---

## 🔒 Security & SmartScreen

Builds are fully automated via [GitHub Actions](.github/workflows/release.yml) — no manual steps, no unsigned binaries injected. You can audit every build step in the workflow file.

**If Windows shows "Windows protected your PC":**
1. Click **More info**
2. Click **Run anyway**

This prompt appears because the app does not yet have an EV code-signing certificate (pending SSL.com open-source program approval). The source code and build pipeline are fully public.

---

## 🏗️ Build from Source

```bash
git clone https://github.com/cenkakay08/hs-firestone-launcher.git
cd hs-firestone-launcher

# Requires .NET 8 SDK
dotnet new winforms -n HsFirestoneLauncher -o HsFirestoneLauncher -f net8.0 --force
copy Program.cs HsFirestoneLauncher\Program.cs
cd HsFirestoneLauncher
dotnet add package System.Management --version 7.0.0
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o ../out
```

The output `out\HsFirestoneLauncher.exe` is your ready-to-run binary.

---

## 🤝 Contributing

Pull requests are welcome! Please branch off `dev` and open a PR targeting `dev`.

---

## 📄 License

[MIT](LICENSE) © 2026 cenkakay08
