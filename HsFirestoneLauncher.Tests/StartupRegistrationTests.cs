using Xunit;

namespace HsFirestoneLauncher.Tests
{
    public class StartupRegistrationTests
    {
        [Fact]
        public void BuildCommand_QuotesExecutablePath()
        {
            Assert.Equal("\"C:\\Apps\\HS Launcher.exe\"", StartupRegistration.BuildCommand(@"C:\Apps\HS Launcher.exe"));
        }

        [Theory]
        [InlineData("\"C:\\Apps\\HS Launcher.exe\"")]
        [InlineData("C:\\Apps\\HS Launcher.exe")]
        public void MatchesExecutable_AcceptsMatchingRegistryValues(string value)
        {
            Assert.True(StartupRegistration.MatchesExecutable(value, @"C:\Apps\HS Launcher.exe"));
        }

        [Fact]
        public void MatchesExecutable_RejectsLegacyVbsCommand()
        {
            string value = "\"C:\\WINDOWS\\System32\\wscript.exe\" \"C:\\Users\\Cenk\\Baslat.vbs\"";
            Assert.False(StartupRegistration.MatchesExecutable(value, @"C:\Apps\HS Launcher.exe"));
        }
    }
}
