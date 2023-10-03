namespace NapAdminCore.Models.ValueObject;

using System;

public class GroupPolicy
{
    public int Policy { get; set; }

    public short Id { get; set; }

    public string Name { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    public int CreatorId { get; set; }

    public string CreatorUsername { get; set; }

    public int ModifierId { get; set; }

    public string ModifierUsername { get; set; }
}
