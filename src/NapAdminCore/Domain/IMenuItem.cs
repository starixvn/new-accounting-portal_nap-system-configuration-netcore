
namespace NapAdminCore.Domain;

using System;

public interface IMenuItem : IEntity<int, string>
{
    string Description { get; set; }

    IMenuItem Parent { get; set; }

    IFunction Function { get; set; }

    ISite Site { get; set; }

    IUser Creator { get; set; }

    IUser Modifier { get; set; }

    DateTime ModifiedDate { get; set; }

    DateTime CreatedDate { get; set; }

    bool IsActive { get; set; }

    short DisplayOrder { get; set; }

    string Environment { get; set; }

    string Url { get; set; }

    int Policy { get; set; }
}
