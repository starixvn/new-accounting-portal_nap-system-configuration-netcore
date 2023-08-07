namespace NapAdminCore.Models.ValueObject;

public class SiteUrl
{
    public int SiteUrlId { get; set; }

    public string Url { get; set; }

    public string Environment { get; set; }

    public bool IsDefault { get; set; }

    public bool IsActive { get; set; }

    public int SiteId { get; set; }

    public string SiteName { get; set; }

    public string Description { get; set; }
}
