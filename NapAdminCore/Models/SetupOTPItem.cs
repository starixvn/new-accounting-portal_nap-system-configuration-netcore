namespace NapAdminCore.Models;

public class SetupOTPItem
{
    public int CustId { get; set; }

    public string Username { get; set; }

    public string SecretKey { get; set; }

    public bool IgnoreOTP { get; set; }

    public string System { get; set; }

    public int Creator { get; set; }

    public string CreatorName { get; set; }

    public string Status { get; set; }

    public string Firstname { get; set; }
}
