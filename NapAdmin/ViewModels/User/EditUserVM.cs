namespace NapAdmin.ViewModels.User;

using NapAdminCore.Criteria.User;

public class EditUserVM
{
    public ReturnData ReturnData { get; set; } = new();

    public UpdateCustCommand Criteria { get; set; }
}
