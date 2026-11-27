using System.Collections.Generic;

namespace HsFirestoneLauncher
{
    public sealed class LangStrings
    {
        public string Title;
        public string Subtitle;
        public string SyncLabel;
        public string StartupLabel;
        public string TrayOn;
        public string TrayOff;
        public string MenuExit;
    }

    public static class Languages
    {
        public static readonly IReadOnlyDictionary<string, LangStrings> All =
            new Dictionary<string, LangStrings>
            {
                { "en", new LangStrings { Title = "HS-Firestone Launcher", Subtitle = "Auto-manage Overwolf & Firestone",                       SyncLabel = "Auto-Sync",              StartupLabel = "Run at Windows Startup",        TrayOn = "HS-Firestone Launcher: ACTIVE",   TrayOff = "HS-Firestone Launcher: PAUSED",     MenuExit = "Quit"     } },
                { "tr", new LangStrings { Title = "HS-Firestone Launcher", Subtitle = "Overwolf & Firestone Otomatik Yönetici",                 SyncLabel = "Otomatik Senkronizasyon", StartupLabel = "Windows Başlangıcında Çalıştır", TrayOn = "HS-Firestone Launcher: AÇIK",    TrayOff = "HS-Firestone Launcher: KAPALI",     MenuExit = "Kapat"    } },
                { "de", new LangStrings { Title = "HS-Firestone Launcher", Subtitle = "Overwolf & Firestone automatisch verwalten",             SyncLabel = "Auto-Sync",              StartupLabel = "Mit Windows starten",           TrayOn = "HS-Firestone Launcher: AKTIV",    TrayOff = "HS-Firestone Launcher: PAUSIERT",   MenuExit = "Beenden"  } },
                { "fr", new LangStrings { Title = "HS-Firestone Launcher", Subtitle = "Gestion auto d'Overwolf & Firestone",                   SyncLabel = "Synchro auto",           StartupLabel = "Lancer avec Windows",           TrayOn = "HS-Firestone Launcher : ACTIF",   TrayOff = "HS-Firestone Launcher : PAUSE",     MenuExit = "Quitter"  } },
                { "es", new LangStrings { Title = "HS-Firestone Launcher", Subtitle = "Gestión automática de Overwolf & Firestone",            SyncLabel = "Sincronización auto",    StartupLabel = "Iniciar con Windows",           TrayOn = "HS-Firestone Launcher: ACTIVO",   TrayOff = "HS-Firestone Launcher: PAUSADO",    MenuExit = "Salir"    } },
                { "it", new LangStrings { Title = "HS-Firestone Launcher", Subtitle = "Gestione auto di Overwolf & Firestone",                 SyncLabel = "Sincronizzazione auto",  StartupLabel = "Avvia con Windows",             TrayOn = "HS-Firestone Launcher: ATTIVO",   TrayOff = "HS-Firestone Launcher: IN PAUSA",   MenuExit = "Esci"     } },
                { "pt", new LangStrings { Title = "HS-Firestone Launcher", Subtitle = "Gerenciar Overwolf & Firestone auto",                   SyncLabel = "Sincronização auto",     StartupLabel = "Iniciar com o Windows",         TrayOn = "HS-Firestone Launcher: ATIVO",    TrayOff = "HS-Firestone Launcher: PAUSADO",    MenuExit = "Sair"     } },
                { "ru", new LangStrings { Title = "HS-Firestone Launcher", Subtitle = "Автоуправление Overwolf & Firestone",                   SyncLabel = "Автосинхронизация",      StartupLabel = "Запускать с Windows",           TrayOn = "HS-Firestone Launcher: АКТИВЕН",  TrayOff = "HS-Firestone Launcher: ПАУЗА",      MenuExit = "Выход"    } },
                { "pl", new LangStrings { Title = "HS-Firestone Launcher", Subtitle = "Automatyczne zarządzanie Overwolf & Firestone",         SyncLabel = "Auto-synchronizacja",    StartupLabel = "Uruchamiaj z Windows",          TrayOn = "HS-Firestone Launcher: AKTYWNY",  TrayOff = "HS-Firestone Launcher: WSTRZYMANY", MenuExit = "Zakończ"  } },
                { "ja", new LangStrings { Title = "HS-Firestone Launcher", Subtitle = "Overwolf & Firestone 自動管理",                           SyncLabel = "自動同期",                StartupLabel = "Windows起動時に実行",            TrayOn = "HS-Firestone Launcher: 有効",      TrayOff = "HS-Firestone Launcher: 一時停止",   MenuExit = "終了"     } },
                { "ko", new LangStrings { Title = "HS-Firestone Launcher", Subtitle = "Overwolf & Firestone 자동 관리",                         SyncLabel = "자동 동기화",              StartupLabel = "Windows 시작 시 실행",           TrayOn = "HS-Firestone Launcher: 활성",      TrayOff = "HS-Firestone Launcher: 일시중지",   MenuExit = "종료"     } },
                { "zh", new LangStrings { Title = "HS-Firestone Launcher", Subtitle = "自动管理 Overwolf & Firestone",                          SyncLabel = "自动同步",                StartupLabel = "开机自动启动",                   TrayOn = "HS-Firestone Launcher: 运行中",    TrayOff = "HS-Firestone Launcher: 已暂停",     MenuExit = "退出"     } },
            };

        public static readonly IReadOnlyList<KeyValuePair<string, string>> List =
            new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("en", "English"),
                new KeyValuePair<string, string>("tr", "Türkçe"),
                new KeyValuePair<string, string>("de", "Deutsch"),
                new KeyValuePair<string, string>("fr", "Français"),
                new KeyValuePair<string, string>("es", "Español"),
                new KeyValuePair<string, string>("it", "Italiano"),
                new KeyValuePair<string, string>("pt", "Português"),
                new KeyValuePair<string, string>("ru", "Русский"),
                new KeyValuePair<string, string>("pl", "Polski"),
                new KeyValuePair<string, string>("ja", "日本語"),
                new KeyValuePair<string, string>("ko", "한국어"),
                new KeyValuePair<string, string>("zh", "中文"),
            };
    }
}
