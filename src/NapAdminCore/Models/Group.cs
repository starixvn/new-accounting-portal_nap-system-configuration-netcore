namespace NapAdminCore.Models;

using System;

[Serializable]
public class Group
{
    public short Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public bool IsActive { get; set; }

    public int CreatorId { get; set; }

    public string CreatorUsername { get; set; }

    public int ModifierId { get; set; }

    public string ModifierUsername { get; set; }

    public User Creator => new() { Id = CreatorId, Username = CreatorUsername };

    public DateTime CreatedDate { get; set; }

    public User Modifier => new() { Id = ModifierId, Username = ModifierUsername };

    public DateTime ModifiedDate { get; set; }

    public int NumOfUser { get; set; }
}
