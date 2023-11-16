
namespace NapAdminCore.Repository;

using System.Collections.Generic;
using System.Linq;
using Fanex.Data.Repository;
using Models.ValueObject;

public interface IFunctionRepository: IBaseRepository
{
    IEnumerable<FunctionPolicy> FetchFunctionPolicy(ICriteria criteria);
}

public class FunctionRepository : BaseRepository, IFunctionRepository
{
    private readonly IDynamicRepository dynamicRepository;

    public FunctionRepository(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository;
    }

    
    public IEnumerable<FunctionPolicy> FetchFunctionPolicy(ICriteria criteria)
    {
        var policies = dynamicRepository.Fetch<FunctionPolicy>(criteria);
        return policies.OrderBy(f => f.Name).ToList();
    }
}
