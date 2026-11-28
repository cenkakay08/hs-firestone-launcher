using System;

namespace HsFirestoneLauncher
{
    public static class StartupRegistration
    {
        public static string BuildCommand(string executablePath)
        {
            return "\"" + executablePath.Replace("\"", "\"\"") + "\"";
        }

        public static bool MatchesExecutable(string? registryValue, string executablePath)
        {
            if (string.IsNullOrWhiteSpace(registryValue))
                return false;

            string command = registryValue.Trim();
            string expected = executablePath.Trim();

            if (command.StartsWith("\"", StringComparison.Ordinal))
            {
                if (command.Length >= 2 &&
                    command[command.Length - 1] == '"' &&
                    command.Length == expected.Length + 2 &&
                    string.Equals(command.Substring(1, expected.Length), expected, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return string.Equals(command, expected, StringComparison.OrdinalIgnoreCase);
        }
    }
}
