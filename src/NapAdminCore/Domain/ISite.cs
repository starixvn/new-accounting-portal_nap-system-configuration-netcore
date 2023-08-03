
namespace NapAdminCore.Domain;

public interface ISite : IEntity<byte, string>
{
    string SiteKey { get; set; }

    string SecretAccessKey { get; set; }

    string Description { get; set; }

    bool IsActive { get; set; }

    string Environment { get; set; }

    int NumOfUser { get; set; }
}
