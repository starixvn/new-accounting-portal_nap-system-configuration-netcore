
namespace NapAdminCore.Domain;

using System;

public interface IGroup : IEntity<short, string>
{
    string Description { get; set; }

    bool IsActive { get; set; }

    IUser Creator { get; set; }

    DateTime CreatedDate { get; set; }

    IUser Modifier { get; set; }

    DateTime ModifiedDate { get; set; }
}
