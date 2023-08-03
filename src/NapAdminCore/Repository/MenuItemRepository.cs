
namespace NapAdminCore.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using Command;
using Criteria.MenuItem;
using Criteria.Misc;
using Domain;
using Fanex.Common.Extensions;
using Fanex.Data;
using Fanex.Data.Repository;
using Models;
using Helpers;

public interface IMenuItemRepository 
{
    ICommandResult Insert(InsertMenuItemCriteria criteria, ICriteria logCriteria);

    ICommandResult Update(ICriteria criteria, ICriteria logCriteria);

    ICommandResult Delete(ICriteria criteria, ICriteria logCriteria);
}
public class MenuItemRepository : BaseRepository<IMenuItem>, IMenuItemRepository
{
    public  IMenuItem Get(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var reader = db.ExecuteReader(criteria);
        if (reader.Read())
        {
            return new MenuItem
            {
                Id = reader.ToInt("MenuItemId"),
                Name = reader.ToString("MenuItemName"),
                Function = new Function
                {
                    Id = reader.ToShort("FunctionId"),
                    Name = reader.ToString("FunctionName")
                },
                Site = new Site
                {
                    Id = reader.ToByte("SiteId"),
                    Name = reader.ToString("SiteName")
                },
                Description = reader.ToString("Description"),
                Parent = reader.IsDBNull("ParentId") ? null : new MenuItem
                {
                    Id = reader.ToInt("ParentId"),
                    Name = reader.ToString("ParentName")
                },
                Policy = reader.ToInt("Policy"),
                Url = reader.ToString("URL"),
                DisplayOrder = reader.ToShort("DisplayOrder"),
                IsActive = reader.ToBool("IsActive"),
                Creator = reader.IsDBNull("CreatorId") ? null : new User
                {
                    Id = reader.ToInt("CreatorId"),
                    Name = reader.ToString("CreatorUsername")
                },
                CreatedDate = reader.ToDateTime("CreatedDate"),
                Modifier = reader.IsDBNull("ModifierId") ? null : new User
                {
                    Id = reader.ToInt("ModifierId"),
                    Name = reader.ToString("ModifierUsername")
                },
                ModifiedDate = reader.ToDateTime("ModifiedDate"),
                Environment = reader.ToString("Environment")
            };
        }

        return null;
    }

    public  int Count(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        return (int)db.ExecuteScalar(criteria);
    }

    public  IEnumerable<IMenuItem> Fetch(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var reader = db.ExecuteReader(criteria);
        var menuItemList = new List<MenuItem>();

        while (reader.Read())
        {
            menuItemList.Add(new MenuItem
            {
                Id = reader.ToInt("MenuItemId"),
                Name = reader.ToString("MenuItemName"),
                Function = new Function
                {
                    Id = reader.ToShort("FunctionId"),
                    Name = reader.ToString("FunctionName")
                },
                Site = new Site
                {
                    Id = reader.ToByte("SiteId"),
                    Name = reader.ToString("SiteName")
                },
                Description = reader.ToString("Description"),
                Parent = reader.IsDBNull("ParentId") ? null : new MenuItem
                {
                    Id = reader.ToInt("ParentId"),
                    Name = reader.ToString("ParentName")
                },
                Policy = reader.ToInt("Policy"),
                Url = reader.ToString("URL"),
                DisplayOrder = reader.ToShort("DisplayOrder"),
                IsActive = reader.ToBool("IsActive"),
                Creator = reader.IsDBNull("CreatorId") ? null : new User
                {
                    Id = reader.ToInt("CreatorId"),
                    Name = reader.ToString("CreatorUsername")
                },
                CreatedDate = reader.ToDateTime("CreatedDate"),
                Modifier = reader.IsDBNull("ModifierId") ? null : new User
                {
                    Id = reader.ToInt("ModifierId"),
                    Name = reader.ToString("ModifierUsername")
                },
                ModifiedDate = reader.ToDateTime("ModifiedDate"),
                Environment = reader.ToString("Environment")
            });
        }

        return menuItemList.OrderBy(m => m.Name).ToList();
    }

    public ICommandResult Insert(InsertMenuItemCriteria criteria, ICriteria logCriteria)
    {
        var hashtable = HashtableExtensions.ToHashtable(criteria, logCriteria, new ResultCriteria());

        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        db.ExecuteNonQuery(hashtable);

        criteria.MenuItemId = Convert.ToInt32(db.GetParameterValue("MenuItemId"));

        return GetCommandResult(hashtable);
    }

    public ICommandResult Update(ICriteria criteria, ICriteria logCriteria)
    {
        var hashtable = HashtableExtensions.ToHashtable(criteria, logCriteria, new ResultCriteria());

        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        db.ExecuteNonQuery(hashtable);

        return GetCommandResult(hashtable);
    }

    public ICommandResult Delete(ICriteria criteria, ICriteria logCriteria)
    {
        throw new NotImplementedException();
    }
}
