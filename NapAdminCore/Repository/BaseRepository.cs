
namespace NapAdminCore.Repository;

using System;
using System.Collections;
using Command;
using Fanex.Data.Repository;


public class BaseRepository : DynamicRepository
{
    public CommandResult GetCommandResult(Hashtable hashtable)
    {
        var ignoredCaseHashtable = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
        foreach (DictionaryEntry entry in hashtable)
        {
            ignoredCaseHashtable.Add(entry.Key, entry.Value);
        }

        var errCode = ignoredCaseHashtable["errcode"];
        var errMsg = ignoredCaseHashtable["errmsg"];

        return new CommandResult
        {
            Code = errCode is DBNull ? 0 : Convert.ToInt32(errCode),
            Message = errMsg is DBNull ? string.Empty : Convert.ToString(errMsg)
        };
    }
}
