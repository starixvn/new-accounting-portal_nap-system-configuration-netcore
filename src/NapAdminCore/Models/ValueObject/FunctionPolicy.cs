
namespace NapAdminCore.Models.ValueObject;

using System;

public class FunctionPolicy
{
    public Function Function { get; set; }

    public Site Site { get; set; }

    public int Policy { get; set; }

    public User Creator { get; set; }

    public DateTime CreatedDate { get; set; }

    public User Modifier { get; set; }

    public DateTime ModifiedDate { get; set; }
}
