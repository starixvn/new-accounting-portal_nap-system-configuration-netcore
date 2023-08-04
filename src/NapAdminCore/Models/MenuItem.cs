namespace NapAdminCore.Models;

using System;
using System.Diagnostics.CodeAnalysis;

[Serializable]
public class MenuItem
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public MenuItem Parent { get; set; }

    public Function Function { get; set; }

    public Site Site { get; set; }

    public User Creator { get; set; }

    public User Modifier { get; set; }

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
            if (!string.IsNullOrEmpty(this.Url))
            {
                return this.Url.StartsWith("/", StringComparison.CurrentCulture) ? "~" + this.Url : this.Url;
            }

            return "/";
        }
    }
}
