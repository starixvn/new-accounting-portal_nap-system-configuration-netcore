namespace NapAdminCore.Criteria;

public class CommandResult
{
    public int Code { get; set; }

    public string Message { get; set; } 

    public bool IsSuccessful => Code == 0;
}