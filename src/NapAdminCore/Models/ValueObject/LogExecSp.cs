
namespace NapAdminCore.Models.ValueObject;

using System;

public class LogExecSp
{
    public long Id { get; set; }

    public DateTime LogDate { get; set; }

    public string Name { get; set; }

    public string Modifier { get; set; }

    public string ActionName { get; set; }

    public short ActionId { get; set; }

    public string OldData { get; set; }

    public string Data { get; set; }

    public int LogType { get; set; }

    public string Items { get; set; }

    public string SubItems { get; set; }
}
