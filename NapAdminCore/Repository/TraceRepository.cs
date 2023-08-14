
namespace NapAdminCore.Repository;

using System.Linq;
using Criteria.Trace;
using Fanex.Data.Repository;
using Models.ValueObject;
public interface ITraceRepository : IDynamicRepository
{
    IQueryable<LogExecSp> SearchLogExecSp(GetAllLogCriteria criteria);

    IQueryable<LogUserLogin> SearchLogUserLogin(GetAllLogUserLoginCriteria criteria);

    LogExecSp GetById(ICriteria criteria);
}
public class TraceRepository : DynamicRepository, ITraceRepository
{
    private readonly IDynamicRepository dynamicRepository;

    public TraceRepository(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository;
    }


    public IQueryable<LogExecSp> SearchLogExecSp(GetAllLogCriteria criteria)
    {
        return dynamicRepository.Fetch<LogExecSp>(criteria).AsQueryable();
    }

    public LogExecSp GetById(ICriteria criteria)
    {
        return dynamicRepository.Get<LogExecSp>(criteria);
    }

    public IQueryable<LogUserLogin> SearchLogUserLogin(GetAllLogUserLoginCriteria criteria)
    {
        return dynamicRepository.Fetch<LogUserLogin>(criteria).AsQueryable();
    }
}
