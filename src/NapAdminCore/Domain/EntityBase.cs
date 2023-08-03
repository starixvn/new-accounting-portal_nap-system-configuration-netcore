
namespace NapAdminCore.Domain;

public sealed class EntityBase<TId, TName> : IEntity<TId, TName>
{
    TId IEntity<TId, TName>.Id
    {
        get;
        set;
    }

    TName IEntity<TId, TName>.Name
    {
        get;
        set;
    }
}
