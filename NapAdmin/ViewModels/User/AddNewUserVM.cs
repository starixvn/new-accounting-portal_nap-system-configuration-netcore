namespace NapAdmin.ViewModels.User;

using NapAdminCore.Criteria.User;

public class AddNewUserVM
{
    public ReturnData ReturnData { get; set; } = new();

    public InsertCust0Command Criteria { get; set; }
}
