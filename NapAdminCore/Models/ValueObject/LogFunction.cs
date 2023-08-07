namespace NapAdminCore.Models.ValueObject;

using System;

public class LogFunction
{
    public int Id { get; set; }

    public string IP { get; set; }

    public DateTime LogDate { get; set; }

    public string BrowserName { get; set; }

    public string Params { get; set; }

    public short FunctionId { get; set; }

    public string FunctionName { get; set; }

    public Function Function => new() { Id = FunctionId, Name = FunctionName };

    public int UserId { get; set; }
    
    public string UserName { get; set; }

    public User User => new() { Id = UserId, Username = UserName };

    public byte SiteId { get; set; }

    public string SiteName { get; set; }

    public Site Site => new() { Id = SiteId, Name = SiteName };
}
