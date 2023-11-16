
namespace NapAdminCore.Repository;

using System.Collections.Generic;
using System.Linq;
using Criteria.User;
using Fanex.Data.Repository;
using Models;
using Models.ValueObject;

public interface IUserRepository: IBaseRepository
{
    IQueryable<User> SearchByUsername(SearchByUsernameUserCriteria criteria);


    IQueryable<UserLevel> Search(SearchUserCriteria criteria);

    IQueryable<UserLevel> GetAllByUsernameGroupSite(GetAllByUsernameGroupSiteUserCriteria criteria);

    IQueryable<UserLevel> SearchUserExcludeFunction(SearchUserExcludeFunctionCriteria criteria);

    IEnumerable<UserPolicy> GetUserPolicy(ICriteria criteria);
}

public class UserRepository : BaseRepository, IUserRepository
{
    public User Get(ICriteria criteria)
    {
        return dynamicRepository.Get<User>(criteria);
    }

    public IEnumerable<User> Fetch(ICriteria criteria)
    {
        return dynamicRepository.Fetch<User>(criteria);
    }

    public IQueryable<User> SearchByUsername(SearchByUsernameUserCriteria criteria)
    {
        return dynamicRepository.Fetch<User>(criteria).AsQueryable();
    }

    public IQueryable<UserLevel> Search(SearchUserCriteria criteria)
    {
        return dynamicRepository.Fetch<UserLevel>(criteria).AsQueryable();
    }

    public IQueryable<UserLevel> GetAllByUsernameGroupSite(GetAllByUsernameGroupSiteUserCriteria criteria)
    {
        return dynamicRepository.Fetch<UserLevel>(criteria).AsQueryable();
    }

    public IQueryable<UserLevel> SearchUserExcludeFunction(SearchUserExcludeFunctionCriteria criteria)
    {
        return dynamicRepository.Fetch<UserLevel>(criteria).AsQueryable();
    }

    public IEnumerable<UserPolicy> GetUserPolicy(ICriteria criteria)
    {
        return dynamicRepository.Fetch<UserPolicy>(criteria).AsQueryable();
    }
}
