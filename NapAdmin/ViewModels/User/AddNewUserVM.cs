namespace NapAdmin.ViewModels.User;

using NapAdminCore.Criteria.User;

public class AddNewUserVM
{
    public ReturnData ReturnData { get; set; } = new();

    public InsertCustCommand Criteria { get; set; }
}
