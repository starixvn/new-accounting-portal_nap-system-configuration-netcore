using System.Text.RegularExpressions;

namespace NapAdminCore.Web;

public static class UserAgentTools
{
    public static string[] GetArray(string a, string b)
    {
        return new[] { a, b };
    }

    public static string[] GetBotName(string userAgentValue)
    {
        var userAgent = userAgentValue.ToLowerInvariant();

        if (userAgent.Contains("help.yahoo.com/"))
        {
            return GetArray("Yahoo", "Yahoo");
        }

        if (userAgent.Contains("google/"))
        {
            return GetArray("Google", "Google");
        }
        if (userAgent.Contains("msnbot/"))
        {
            return GetArray("MSNBot", "MSNBot");
        }
        if (userAgent.Contains("googlebot/"))
        {
            return GetArray("Google", "Google");
        }
        if (userAgent.Contains("webcrawler/"))
        {
            return GetArray("WebCrawler", "WebCrawler");
        }
        if (userAgent.Contains("inktomi"))
        {
            return GetArray("Inktomi", "Inktomi");
        }
        return userAgent.Contains("teoma") ? GetArray("Teoma", "Teoma") : Array.Empty<string>();
    }

    private const string WindowsNtPattern = @"Windows NT (\d+)";

    public static string[] GetOS(string userAgent)
    {
        var arrayBog = GetBotName(userAgent);

        if (arrayBog is { Length: > 0 })
        {
            return arrayBog;
        }

        if (userAgent.Contains("Windows-NT"))
        {
            return GetArray("Win", "Window NT");
        }

        if (userAgent.Contains("Windows NT"))
        {
            if (userAgent.Contains("Windows NT 5.1"))
            {
                return GetArray("Win", "Window XP");
            }

            if (userAgent.Contains("Windows NT 6.0"))
            {
                return GetArray("Win", "Window Vista");
            }

            if (userAgent.Contains("Windows NT 6.1"))
            {
                return GetArray("Win", "Window 7");
            }

            if (userAgent.Contains("Windows NT 5.0"))
            {
                return GetArray("Win", "Window 2000");
            }

            if (userAgent.Contains("Windows NT 5.2"))
            {
                return GetArray("Win", "Window 2003");
            }

            if (userAgent.Contains("Windows NT 4.0"))
            {
                return GetArray("Win", "Windows NT4");
            }

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

        if (userAgent.Contains("Win") && userAgent.Contains("Windows"))
        {
            if (userAgent.Contains("Windows 98"))
            {
                return GetArray("Win", "Windows 98");
            }

            if (userAgent.Contains("Windows_98"))
            {
                return GetArray("Win", "Windows 98");
            }

            if (userAgent.Contains("Windows 2000"))
            {
                return GetArray("Win", "Window 2000");
            }

            if (userAgent.Contains("Windows 95"))
            {
                return GetArray("Win", "Window 95");
            }

            if (userAgent.Contains("Windows 9x"))
            {
                return GetArray("Win", "Window 9x");
            }

            if (userAgent.Contains("Windows ME"))
            {
                return GetArray("Win", "Window ME");
            }

            if (userAgent.Contains("Windows CE;"))
            {
                return GetArray("Win", "Window CE");
            }

            if (userAgent.Contains("Windows 3.1"))
            {
                return GetArray("Win", "Window 3.1");
            }
        }

        if (userAgent.Contains("Mac OS X"))
        {
            if (userAgent.Contains("iPhone"))
            {
                return userAgent.Contains("iPod") ? GetArray("iOS", "iOS-iPod") : GetArray("iOS", "iOS-iPhone");
            }

            return userAgent.Contains("iPad") ?
                GetArray("iOS", "iOS-iPad") :
                GetArray("Mac", "MacOSX");
        }

        if (userAgent.Contains("Android"))
        {
            return GetArray("Linux", "Android");
        }

        if (userAgent.Contains("Mac_PowerPC"))
        {
            return GetArray("Mac", "MacPPC");
        }

        if (userAgent.Contains("Macintosh"))
        {
            return userAgent.Contains("PPC") ? GetArray("Mac", "MacPPC") : GetArray("Mac?", "Mac?");
        }

        if (userAgent.Contains("FreeBSD"))
        {
            return GetArray("*BSD", "*BSD FreeBSD");
        }

        if (userAgent.Contains("OpenBSD"))
        {
            return GetArray("*BSD", "*BSD OpenBSD");
        }

        if (userAgent.Contains("Linux"))
        {
            return GetArray("Linux", userAgent.Contains("Ubuntu/") ? "Ubuntu" : "Linux");
        }

        if (userAgent.Contains("CentOS"))
        {
            return GetArray("Linux", "Linux CentOS");
        }

        if (userAgent.Contains("NetBSD"))
        {
            return GetArray("*BSD", "*BSD NetBSD");
        }

        if (userAgent.Contains("Unix"))
        {
            return GetArray("Linux", "Linux");
        }

        if (userAgent.Contains("SunOS"))
        {
            return GetArray("Unix", "SunOS");
        }

        if (userAgent.Contains("IRIX"))
        {
            return GetArray("Unix", "IRIX");
        }

        if (userAgent.Contains("SonyEricsson"))
        {
            return GetArray("SonyEricsson", "SonyEricsson");
        }

        if (userAgent.Contains("Nokia"))
        {
            return GetArray("Nokia", "Nokia");
        }

        if (userAgent.Contains("BlackBerry"))
        {
            return GetArray("BlackBerry", "BlackBerry");
        }

        if (userAgent.Contains("SymbianOS"))
        {
            return GetArray("SymbianOS", "SymbianOS");
        }

        if (userAgent.Contains("BeOS"))
        {
            return GetArray("BeOS", "BeOS");
        }

        if (userAgent.Contains("Nintendo Wii"))
        {
            return GetArray("Nintendo Wii", "Nintendo Wii");
        }

        if (userAgent == "Win32NT")
        {
            return GetArray("Win32NT", "Win32NT");
        }

        return userAgent.Contains("J2ME/MIDP") ? GetArray("Java", "J2ME") : GetArray("<b>?</b>", "<b>?</b>");
    }
}
