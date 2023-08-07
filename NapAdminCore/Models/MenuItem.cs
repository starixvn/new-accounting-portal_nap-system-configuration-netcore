namespace NapAdminCore.Models;

using System;

[Serializable]
public class MenuItem
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public int ParentId { get; set; }

    public string ParentName { get; set; }

    public int FunctionId { get; set; }

    public string FunctionName { get; set; }

    public int SiteId { get; set; }
    
    public string SiteName { get; set; }

    public int CreatorId { get; set; }

    public string CreatorUserName { get; set; }

    public int ModifierId { get; set; }

    public string ModifierUserName { get; set; }

    public DateTime ModifiedDate { get; set; }

    public DateTime CreatedDate { get; set; }

    public bool IsActive { get; set; }

    public short DisplayOrder { get; set; }

    public string Environment { get; set; }

    public string Url { get; set; }

    public int Policy { get; set; }

    public string ValidUrl
    {
        get
        {
            if (!string.IsNullOrEmpty(Url))
            {
                return Url.StartsWith("/", StringComparison.CurrentCulture) ? "~" + Url : Url;
            }

            return "/";
        }
    }
}
