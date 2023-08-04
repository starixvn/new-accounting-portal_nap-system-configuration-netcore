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

    public User User { get; set; }

    public Site Site { get; set; }
}