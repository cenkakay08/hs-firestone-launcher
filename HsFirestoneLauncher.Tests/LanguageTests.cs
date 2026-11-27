using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace HsFirestoneLauncher.Tests
{
    public class LanguageTests
    {
        private static readonly string[] ExpectedCodes =
            { "en", "tr", "de", "fr", "es", "it", "pt", "ru", "pl", "ja", "ko", "zh" };

        // ── 1. Toplam dil sayısı ────────────────────────────────────────────
        [Fact]
        public void Languages_All_Contains_12_Entries()
        {
            Assert.Equal(12, Languages.All.Count);
        }

        [Fact]
        public void Languages_List_Contains_12_Entries()
        {
            Assert.Equal(12, Languages.List.Count);
        }

        // ── 2. Tüm beklenen dil kodları mevcut ─────────────────────────────
        [Theory]
        [InlineData("en")]
        [InlineData("tr")]
        [InlineData("de")]
        [InlineData("fr")]
        [InlineData("es")]
        [InlineData("it")]
        [InlineData("pt")]
        [InlineData("ru")]
        [InlineData("pl")]
        [InlineData("ja")]
        [InlineData("ko")]
        [InlineData("zh")]
        public void Languages_All_ContainsCode(string code)
        {
            Assert.True(Languages.All.ContainsKey(code), $"Missing language code: {code}");
        }

        // ── 3. Her dilde tüm alanlar dolu ──────────────────────────────────
        [Theory]
        [InlineData("en")]
        [InlineData("tr")]
        [InlineData("de")]
        [InlineData("fr")]
        [InlineData("es")]
        [InlineData("it")]
        [InlineData("pt")]
        [InlineData("ru")]
        [InlineData("pl")]
        [InlineData("ja")]
        [InlineData("ko")]
        [InlineData("zh")]
        public void Languages_AllFields_AreNonEmpty(string code)
        {
            var s = Languages.All[code];
            Assert.False(string.IsNullOrWhiteSpace(s.Title),       $"[{code}] Title is empty");
            Assert.False(string.IsNullOrWhiteSpace(s.Subtitle),    $"[{code}] Subtitle is empty");
            Assert.False(string.IsNullOrWhiteSpace(s.SyncLabel),   $"[{code}] SyncLabel is empty");
            Assert.False(string.IsNullOrWhiteSpace(s.StartupLabel),$"[{code}] StartupLabel is empty");
            Assert.False(string.IsNullOrWhiteSpace(s.TrayOn),      $"[{code}] TrayOn is empty");
            Assert.False(string.IsNullOrWhiteSpace(s.TrayOff),     $"[{code}] TrayOff is empty");
            Assert.False(string.IsNullOrWhiteSpace(s.MenuExit),    $"[{code}] MenuExit is empty");
        }

        // ── 4. All sözlüğü ve List aynı kodlara sahip ──────────────────────
        [Fact]
        public void Languages_All_And_List_HaveSameKeys()
        {
            var dictKeys = Languages.All.Keys.OrderBy(k => k).ToList();
            var listKeys = Languages.List.Select(kv => kv.Key).OrderBy(k => k).ToList();
            Assert.Equal(dictKeys, listKeys);
        }

        // ── 5. List'teki dil isimlerinin hepsi dolu ────────────────────────
        [Fact]
        public void Languages_List_AllDisplayNames_AreNonEmpty()
        {
            foreach (var kv in Languages.List)
                Assert.False(string.IsNullOrWhiteSpace(kv.Value),
                    $"Display name for '{kv.Key}' is empty");
        }

        // ── 6. TrayOn ve TrayOff birbirinden farklı olmalı ─────────────────
        [Theory]
        [InlineData("en")]
        [InlineData("tr")]
        [InlineData("de")]
        [InlineData("fr")]
        [InlineData("es")]
        [InlineData("it")]
        [InlineData("pt")]
        [InlineData("ru")]
        [InlineData("pl")]
        [InlineData("ja")]
        [InlineData("ko")]
        [InlineData("zh")]
        public void Languages_TrayOn_And_TrayOff_AreDifferent(string code)
        {
            var s = Languages.All[code];
            Assert.NotEqual(s.TrayOn, s.TrayOff);
        }

        // ── 7. Varsayılan dil "en" List'in ilk elemanı olmalı ──────────────
        [Fact]
        public void Languages_List_FirstEntry_IsEnglish()
        {
            Assert.Equal("en", Languages.List[0].Key);
        }

        // ── 8. Her dilde Title değeri aynı (marka tutarlılığı) ─────────────
        [Fact]
        public void Languages_AllTitles_AreConsistent()
        {
            const string expectedTitle = "HS-Firestone Launcher";
            foreach (var (code, s) in Languages.All)
                Assert.Equal(expectedTitle, s.Title,
                    ignoreCase: false, message: $"[{code}] Title mismatch");
        }
    }
}
