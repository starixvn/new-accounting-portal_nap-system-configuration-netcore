
namespace NapAdminCore.Web;

using System;

public static class DateTimeExtensions
{
    public const string ShortDateFormat = "MM/dd/yyyy";
    public const string ShortMonthFormatDate = "dd MMM yyyy";

    public static string ToShortFormatDate(this DateTime? date)
    {
        return date.HasValue ? date.Value.ToShortFormatDate() : string.Empty;
    }

    public static string ToShortFormatDate(this DateTime date)
    {
        return date.ToString(ShortDateFormat);
    }

    public static string ToShortMonthFormatDate(this DateTime? date)
    {
        return date.HasValue ? date.Value.ToShortFormatDate() : string.Empty;
    }

    public static string ToShortMonthFormatDate(this DateTime date)
    {
        return date.ToString(ShortMonthFormatDate);
    }
}
