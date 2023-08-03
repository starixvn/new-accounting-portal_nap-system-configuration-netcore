
namespace NapAdminCore.Service;

public interface IServiceContext
{
    int UserId { get; set; }

    int SiteId { get; set; }

    string Ip { get; set; }
}

public class ServiceContext : IServiceContext
{
    public int UserId { get; set; }

    public int SiteId { get; set; }

    public string Ip { get; set; }
}
