namespace NapAdminCore.Models;

using System;

[Serializable]
public class Function
{
    public short Id { get; set; }

    public string Name { get; set; }

    public string Key { get; set; }

    public string Description { get; set; }

    public short DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public byte Crud { get; set; }

    public string Environment { get; set; }

    public DateTime CreatedDate { get; set; }

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
