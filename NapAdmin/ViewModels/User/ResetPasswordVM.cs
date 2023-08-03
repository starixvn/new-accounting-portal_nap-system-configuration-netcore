namespace NapAdmin.ViewModels.User;

public class ResetPasswordVM
{
    public ReturnData ReturnData { get; set; } = new ReturnData();

    public ResetPasswordCustCriteria Criteria { get; set; }
}