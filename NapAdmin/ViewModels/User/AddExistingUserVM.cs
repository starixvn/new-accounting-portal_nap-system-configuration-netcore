namespace NapAdmin.ViewModels.User;

public class AddExistingUserVM
{
    public ReturnData ReturnData { get; set; } = new ReturnData();

    public PagerList<IUser> UserList { get; set; }

    public string UserName { get; set; } = string.Empty;

    public int PageIndex { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}