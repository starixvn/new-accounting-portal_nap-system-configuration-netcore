namespace NapAdminCore.Models.ValueObject;

using System.Diagnostics.CodeAnalysis;

public class SiteUrl
{
    public int SiteUrlId { get; set; }

    [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Reviewed")]
    public string Url { get; set; }

    public string Environment { get; set; }

    public bool IsDefault { get; set; }

    public bool IsActive { get; set; }

    public Site Site { get; set; }
}