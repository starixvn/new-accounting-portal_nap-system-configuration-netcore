
namespace NapAdminCore.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using Command;
using Criteria.MenuItem;
using Criteria.Misc;
using Fanex.Data;
using Fanex.Data.Repository;
using Models;
using Helpers;

public interface IMenuItemRepository 
{
    ICommandResult Insert(InsertMenuItemCriteria criteria, ICriteria logCriteria);

    ICommandResult Update(ICriteria criteria, ICriteria logCriteria);
}
public class MenuItemRepository : BaseRepository, IMenuItemRepository
{
    private readonly IDynamicRepository dynamicRepository;

    public MenuItemRepository(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository;
    }

    public  MenuItem Get(ICriteria criteria)
    {
        return dynamicRepository.Get<MenuItem>(criteria);
    }

    public  int Count(ICriteria criteria)
    {
        return dynamicRepository.Get<int>(criteria);
    }

    public  IEnumerable<MenuItem> Fetch(ICriteria criteria)
    {
        var menuItemList = dynamicRepository.Fetch<MenuItem>(criteria);
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
}
