
namespace NapAdminCore.Repository;

using System;
using System.Collections;
using Command;
using Fanex.Data.Repository;


public class BaseRepository<T> : DynamicRepository
{
    public CommandResult GetCommandResult(Hashtable hashtable)
    {
        var errCode = hashtable["errcode"];
        var errMsg = hashtable["errmsg"];

        return new CommandResult
        {
            Code = errCode is DBNull ? 0 : Convert.ToInt32(errCode),
            Message = errMsg is DBNull ? string.Empty : Convert.ToString(errMsg)
        };
    }
}
