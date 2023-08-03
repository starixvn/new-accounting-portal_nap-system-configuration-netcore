
namespace NapAdminCore.Models;

using System;
using Domain;

[Serializable]
public class Group : IGroup
{
    public short Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public bool IsActive { get; set; }

    public IUser Creator { get; set; }

    public DateTime CreatedDate { get; set; }

    public IUser Modifier { get; set; }

    public DateTime ModifiedDate { get; set; }

    public int NumOfUser { get; set; }
}
