
namespace NapAdminCore.Web;

using System;

public static class DateTimeExtensions
{
    public const string ShortDateFormat = "MM/dd/yyyy";
    public const string ShortMonthFormatedDate = "dd MMM yyyy";

    public static string ToShortFormatedDate(this DateTime? date)
    {
        if (date.HasValue)
        {
            return date.Value.ToShortFormatedDate();
        }

        return string.Empty;
    }

    public static string ToShortFormatedDate(this DateTime date)
    {
        return date.ToString(ShortDateFormat);
    }

    public static string ToShortMonthFormatedDate(this DateTime? date)
    {
        if (date.HasValue)
        {
            return date.Value.ToShortFormatedDate();
        }

        return string.Empty;
    }

    public static string ToShortMonthFormatedDate(this DateTime date)
    {
        return date.ToString(ShortMonthFormatedDate);
    }
}
