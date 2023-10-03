namespace NapAdminCore.Models.ValueObject;

using System;

public class SiteLevel
{
    public byte SiteId { get; set; }

    public string SiteName { get; set; }

    public int Level { get; set; }

    public bool IsActive { get; set; }

    public DateTime ExpiredDate { get; set; }

    public bool IsNeverExpired { get; set; }

    public DateTime LastLoginDate { get; set; }

    public int CreatorId { get; set; }

    public string CreatorUsername { get; set; }

    public int ModifierId { get; set; }

    public string ModifierUsername { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }
}
