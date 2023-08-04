namespace NapAdminCore.Models.ValueObject;

using System;

public class LogUserLogin
{
    public long Id { get; set; }

    public string IP { get; set; }

    public DateTime LogDate { get; set; }

    public string Doing { get; set; }

    public string DomainName { get; set; }

    public string BrowserName { get; set; }

    public string Platform { get; set; }

    public string CountryCode { get; set; }

    public int Duration { get; set; }

    public User User => new() { Id = UserId };

    public Site Site => new() { Id = SiteId, Name = SiteName };

    public byte SiteId { get; set; }

    public string SiteName { get; set; }

    public int UserId { get; set; }
}
