namespace NapAdminCore.Models.ValueObject;

using System;

public class UserLevel
{
    public int Level { get; set; }

    public User User { get; set; }

    public bool IsActive { get; set; }

    public int Status { get; set; }

    public DateTime LastLoginDate { get; set; }

    public User Creator { get; set; }

    public DateTime CreatedDate { get; set; }

    public User Modifier { get; set; }

    public DateTime ModifiedDate { get; set; }

    public Site Site { get; set; }
}
