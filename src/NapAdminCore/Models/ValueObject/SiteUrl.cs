namespace NapAdminCore.Models.ValueObject;

using System.Diagnostics.CodeAnalysis;

public class SiteUrl
{
    public int SiteUrlId { get; set; }

    public string Url { get; set; }

    public string Environment { get; set; }

    public bool IsDefault { get; set; }

    public bool IsActive { get; set; }

    public Site Site { get; set; }
}
