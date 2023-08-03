
namespace NapAdminCore.Models;

using System;
using Domain;

[Serializable]
public class Function : IFunction
{
    public short Id { get; set; }

    public string Name { get; set; }

    public string Key { get; set; }

    public string Description { get; set; }

    public IFunction Parent { get; set; }

    public short DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public byte Crud { get; set; }

    public string Environment { get; set; }

    public ISite Site { get; set; }

    public IUser Creator { get; set; }

    public DateTime CreatedDate { get; set; }

    public IUser Modifier { get; set; }

    public DateTime ModifiedDate { get; set; }

    public bool Readonly => Crud == (byte)Business.Constant.Crud.Read;
}
