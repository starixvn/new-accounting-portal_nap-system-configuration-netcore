
namespace NapAdminCore.Models.ValueObject;

public class Downtime
{
    public string From { get; set; }

    public string To { get; set; }

    public bool IsDowntime { get; set; }
}
