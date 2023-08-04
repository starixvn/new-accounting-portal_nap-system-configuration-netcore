namespace NapAdminCore.Models.ValueObject;

using System;

public class GroupPolicy
{
    public int Policy { get; set; }

    public Group Group { get; set; }

    public User Creator { get; set; }

    public DateTime CreatedDate { get; set; }

    public User Modifier { get; set; }

    public DateTime ModifiedDate { get; set; }
}
