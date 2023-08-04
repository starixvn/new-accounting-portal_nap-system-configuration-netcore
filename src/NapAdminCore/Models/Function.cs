namespace NapAdminCore.Models;

using System;
using System.Diagnostics.CodeAnalysis;

[Serializable]
public class Function
{
    public short Id { get; set; }

    public string Name { get; set; }

    public string Key { get; set; }

    public string Description { get; set; }

    public Function Parent => new() { Id = ParentId, Name = ParentName };

    public short DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public byte Crud { get; set; }

    public string Environment { get; set; }

    public Site Site => new() { Id = SiteId, Name = SiteName };

    public User Creator => new() { Id = CreatorId, Username = CreatorUsername };

    public DateTime CreatedDate { get; set; }

    public User Modifier => new() { Id = ModifierId, Username = ModifierUsername };

    public DateTime ModifiedDate { get; set; }

    public bool Readonly => Crud == (byte)AdminConstant.Crud.Read;

    public short ParentId { get; set; }
    public string ParentName { get; set; }
    public byte SiteId { get; set; }
    public string SiteName { get; set; }
    public int CreatorId { get; set; }
    public string CreatorUsername { get; set; }
    public int ModifierId { get; set; }
    public string ModifierUsername { get; set; }
}
