namespace NapAdmin.ViewModels.User;

using NapAdminCore.Criteria.User;

public class ResetPasswordVM
{
    public ReturnData ReturnData { get; set; } = new();

    public ResetPasswordCustCommand Criteria { get; set; }
}
