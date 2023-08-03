
namespace NapAdminCore.Domain;

using System;
using System.ComponentModel.DataAnnotations;
using Web;

public interface IUser : IEntity<int, string>
{
    [Display(Name = "Group", ResourceType = typeof(ViewResources))]
    IGroup Group { get; set; }

    [Display(Name = "Username", ResourceType = typeof(ViewResources))]
    string Username { get; set; }

    [Display(Name = "Password", ResourceType = typeof(ViewResources))]
    string Password { get; set; }

    [Display(Name = "Last_Login_Date", ResourceType = typeof(ViewResources))]
    DateTime LastLoginDate { get; set; }

    [Display(Name = "Creator", ResourceType = typeof(ViewResources))]
    IUser Creator { get; set; }

    [Display(Name = "Created_Date", ResourceType = typeof(ViewResources))]
    DateTime CreatedDate { get; set; }

    [Display(Name = "Modifier", ResourceType = typeof(ViewResources))]
    IUser Modifier { get; set; }

    [Display(Name = "Modified_Date", ResourceType = typeof(ViewResources))]
    DateTime ModifiedDate { get; set; }

    [Display(Name = "First_Name", ResourceType = typeof(ViewResources))]
    string FirstName { get; set; }

    [Display(Name = "Last_Name", ResourceType = typeof(ViewResources))]
    string LastName { get; set; }

    [Display(Name = "Email", ResourceType = typeof(ViewResources))]
    string Email { get; set; }
}
