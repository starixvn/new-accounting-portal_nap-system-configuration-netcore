namespace NapAdminCore.Models;

using System;
using System.ComponentModel.DataAnnotations;
using Web;

[Serializable]
public class User
{
    public int Id { get; set; }

    [Display(Name = "Name", ResourceType = typeof(ViewResources))]
    public string Name { get; set; }

    [Display(Name = "Username", ResourceType = typeof(ViewResources))]
    public string Username { get; set; }

    [Display(Name = "Password", ResourceType = typeof(ViewResources))]
    public string Password { get; set; }

    [Display(Name = "Last_Login_Date", ResourceType = typeof(ViewResources))]
    public DateTime LastLoginDate { get; set; }

    [Display(Name = "Created_Date", ResourceType = typeof(ViewResources))]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Modified_Date", ResourceType = typeof(ViewResources))]
    public DateTime ModifiedDate { get; set; }

    [Display(Name = "First_Name", ResourceType = typeof(ViewResources))]
    public string FirstName { get; set; }

    [Display(Name = "Last_Name", ResourceType = typeof(ViewResources))]
    public string LastName { get; set; }

    [Display(Name = "Email", ResourceType = typeof(ViewResources))]
    public string Email { get; set; }

    public bool Closed { get; set; }

    public short GroupId { get; set; }

    [Display(Name = "Group", ResourceType = typeof(ViewResources))]
    public string GroupName { get; set; }

    public int CreatorId { get; set; }

    [Display(Name = "Creator", ResourceType = typeof(ViewResources))]
    public string CreatorUsername { get; set; }

    public int ModifierId { get; set; }

    [Display(Name = "Modifier", ResourceType = typeof(ViewResources))]
    public string ModifierUsername { get; set; }

}
