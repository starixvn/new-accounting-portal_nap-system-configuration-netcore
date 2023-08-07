namespace NapAdminCore.Models.ValueObject;

using System;

public class UserPolicy
{
    public int Policy { get; set; }

    public int UserId { get; set; }

    public string Username { get; set; }

    public DateTime LastLoginDate { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public bool Closed { get; set; }

    public short GroupId { get; set; }

    public string GroupName { get; set; }

    public int CreatorId { get; set; }

    public string CreatorUsername { get; set; }

    public int ModifierId { get; set; }

    public string ModifierUsername { get; set; }

}
