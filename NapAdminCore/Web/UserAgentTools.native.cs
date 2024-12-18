using System.Text.RegularExpressions;

namespace NapAdminCore.Web;

public static class UserAgentTools
{
    private const string WindowsNtPattern = @"Windows NT (\d+)";

    public static string[] GetOS(string userAgent)
    {
        if (userAgent.Contains("Windows-NT"))
        {
            return GetArray("Win", "Window NT");
        }

        if (userAgent.Contains("Windows NT"))
        {
            if (userAgent.Contains("Windows NT)"))
            {
                return GetArray("Win", "Windows NT");
            }

            if (userAgent.Contains("Windows NT;"))
            {
                return GetArray("Win", "Windows NT");
            }

            var match = Regex.Match(userAgent, WindowsNtPattern);
            if (match.Success)
            {
                return GetArray("Win", "Window " + match.Groups[1].Value);
            }
        }

        if (userAgent.Contains("Linux"))
        {
            return GetArray("Linux", userAgent.Contains("Ubuntu/") ? "Ubuntu" : "Linux");
        }

        if (userAgent.Contains("CentOS"))
        {
            return GetArray("Linux", "Linux CentOS");
        }

        if (userAgent.Contains("Unix"))
        {
            return GetArray("Linux", "Linux");
        }

        return userAgent == "Win32NT" ? GetArray("Win32NT", "Win32NT") : GetArray("<b>?</b>", "<b>?</b>");
    }

    private static string[] GetArray(string a, string b)
    {
        return [a, b];
    }

}
