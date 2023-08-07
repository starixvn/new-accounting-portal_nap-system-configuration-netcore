namespace NapAdminCore.Web;

using System;
using System.Globalization;
using Microsoft.AspNetCore.Html;
using Models;

public static class HtmlHelperExtensions
{
    public static HtmlString ToFullDateString(
        this DateTime value,
        string dateFormat = "MMM dd, yyyy",
        string timeFormat = "HH:mm tt")
    {
        return new HtmlString(
            $"<span class='datetime'><span class='date'>{value.ToString(dateFormat)}</span> <span class='time'>{value.ToString(timeFormat)}</span></span>");
    }

    public static HtmlString ToUserLevel(this int value)
    {
        return value switch
        {
            (int)AdminConstant.Level.PortalAdmin => new HtmlString(ViewResources.Portal_Admin),
            (int)AdminConstant.Level.SiteAdmin => new HtmlString(ViewResources.Site_Admin),
            (int)AdminConstant.Level.User => new HtmlString(ViewResources.User),
            _ => new HtmlString(string.Empty)
        };
    }

    public static HtmlString ToPolicy(this int value)
    {
        return value switch
        {
            (int)AdminConstant.Policy.Host => new HtmlString(ViewResources.Host),
            (int)AdminConstant.Policy.Read => new HtmlString(ViewResources.Read),
            _ => new HtmlString(ViewResources.None)
        };
    }

    public static string DecodeXMLValue(this string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            return value.Replace("&amp;", "&")
                .Replace("&lt;", "<")
                .Replace("&gt;", ">")
                .Replace("&quot;", "\"")
                .Replace("&apos;", "''");
        }

        return value;
    }

    public static string GetGeoName(this string geoId)
    {
        var listCulture = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

        foreach (var culture in listCulture)
        {
            try
            {
                var region = new RegionInfo(culture.ToString());

                if (region.GeoId.ToString().Equals(geoId))
                {
                    return region.DisplayName;
                }
            }
            catch(Exception) 
            {
            }
        }

        return geoId;
    }

    public static string GetOperatingSystem(this string platform)
    {
        var array = UserAgentTools.GetOS(platform);
        return array[1];
    }
}
