namespace NapAdminCore.Models;

using System;

[Serializable]
public class Site
{
    public byte Id { get; set; }

    public string Name { get; set; }

    public string SiteKey { get; set; }

    public string SecretAccessKey { get; set; }

    public string Description { get; set; }

    public bool IsActive { get; set; }

    public string Environment { get; set; }

    public int NumOfUser { get; set; }
}
