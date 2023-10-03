namespace NapAdminCore.Models.ValueObject;

using System;

public class FunctionPolicy
{
    public short Id { get; set; }

    public string Name { get; set; }

    public string Key { get; set; }

    public string Description { get; set; }

    public byte Crud { get; set; }

    public short DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public short ParentId { get; set; }

    public string ParentName { get; set; }

    public byte SiteId { get; set; }

    public string SiteName { get; set; }

    public int Policy { get; set; }

    public int CreatorId { get; set; }

    public string CreatorUserName { get; set; }

    public DateTime CreatedDate { get; set; }

    public int ModifierId { get; set; }

    public string ModifierUserName { get; set; }

    public DateTime ModifiedDate { get; set; }
}
