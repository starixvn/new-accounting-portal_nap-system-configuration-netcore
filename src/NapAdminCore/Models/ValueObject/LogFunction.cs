namespace NapAdminCore.Models.ValueObject;

using System;

public class LogFunction
{
    public int Id { get; set; }

    public string IP { get; set; }

    public DateTime LogDate { get; set; }

    public string BrowserName { get; set; }

    public string Params { get; set; }

    public Function Function { get; set; }

    public User User { get; set; }

    public Site Site { get; set; }
}
