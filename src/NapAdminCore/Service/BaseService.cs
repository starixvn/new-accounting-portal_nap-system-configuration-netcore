
namespace NapAdminCore.Service;

using System;
using System.Collections.Generic;
using Criteria.Misc;
using Fanex.Data.Repository;

public interface IBaseService
{
    IServiceContext ServiceContext { get; set; }

    IEnumerable<T> GetData<T>(ICriteria criteria);

    T GetItem<T>(ICriteria criteria) where T : new();

    LogExecSpCriteria GetLogExecSpCriteria();
}
public class BaseService : IBaseService
{
    private readonly IDynamicRepository _dynamicRepository;

    public BaseService(IDynamicRepository dynamicRepository = null)
    {
        _dynamicRepository = dynamicRepository ?? new DynamicRepository();
    }

    public IServiceContext ServiceContext { get; set; }

    public virtual IEnumerable<T> GetData<T>(ICriteria criteria)
    {
        if (criteria.IsValid())
        {
            return _dynamicRepository.Fetch<T>(criteria);
        }

        return new List<T>();
    }

    public virtual T GetItem<T>(ICriteria criteria) where T : new()
    {
        if (criteria.IsValid())
        {
            return _dynamicRepository.Get<T>(criteria);
        }

        return new T();
    }

    public LogExecSpCriteria GetLogExecSpCriteria()
    {
        return new LogExecSpCriteria
        {
            CurrentUserId = ServiceContext.UserId,
            CurrentSiteId = ServiceContext.SiteId,
            LogDate = DateTime.Now,
            LoginIp = ServiceContext.Ip
        };
    }
}
