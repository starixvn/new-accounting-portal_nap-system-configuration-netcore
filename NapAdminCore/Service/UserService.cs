namespace NapAdminCore.Service;

using System;
using System.Collections;
using DbParams;
using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Command;
using Criteria.User;
using Models.ValueObject;
using Repository;
using ONEbook.OTP.Services;

public interface IUserService
{
    void SyncUserToOWP(int userId);

    bool IgnoredOTP(int userId, string system);

    bool VerifyOTP(int userId, string system, string otp);

    User GetById(int id);

    User GetByUsername(string username);

    bool CheckUsernameExist(string username);

    IEnumerable<UserPolicy> GetByFunctionAndSite(int functionId, int siteId);

    IEnumerable<User> GetByGroupAndSite(int groupId = 0, int siteId = 0, bool? isActive = null);

    IQueryable<UserLevel> GetAllByUsernameGroupSite(GetAllByUsernameGroupSiteUserCriteria criteria);

    IQueryable<UserLevel> SearchUser(SearchUserCriteria criteria);

    IQueryable<UserLevel> SearchUserExcludeFunction(SearchUserExcludeFunctionCriteria criteria);

    IQueryable<User> SearchByUsername(SearchByUsernameUserCriteria criteria);

    int GetUserSiteLevel(int userId, int siteId);

    int GetUserFunctionPolicy(int userId, int functionId);

    int GetUserSiteFunctionKeyPolicy(int userId, int siteId, string functionKey);

    Hashtable GetUserListSiteFunctionKeyPolicy(int userId, string functionKey, List<SelectListItem> listSite);

    ICommandResult Insert(InsertCustCriteria criteria);

    ICommandResult AssignSite(AssignSiteUserCriteria criteria);

    ICommandResult UpdateSite(UpdateSiteToUserCriteria criteria);

    ICommandResult AssignFunction(AssignFunctionUserCriteria criteria);

    ICommandResult InsertOrUpdate(ICriteria criteria);
}

public class UserService : IUserService
{
    private readonly IUserRepository userRepository;
    private readonly IDynamicRepository dynamicRepository;
    private readonly LoggedUserContext loggedUserContext;
    public UserService(IUserRepository repository, IDynamicRepository dynamicRepository, LoggedUserContext loggedUserContext)
    {
        userRepository = repository;
        this.dynamicRepository = dynamicRepository;
        this.loggedUserContext = loggedUserContext;
    }

    public User GetById(int id)
    {
        var criteria = new GetByIdUserCriteria
        {
            UserId = id
        };

        return dynamicRepository.Get<User>(criteria);
    }

    public User GetByUsername(string username)
    {
        var criteria = new GetByUsernameUserCriteria
        {
            Username = username
        };

        return dynamicRepository.Get<User>(criteria);
    }

    public bool CheckUsernameExist(string username)
    {
        var user = GetByUsername(username);

        return user != null && user.Id != 0;
    }

    public IEnumerable<User> GetByGroupAndSite(int groupId = 0, int siteId = 0, bool? isActive = null)
    {
        var criteria = new GetByGroupAndSiteUserCriteria
        {
            GroupId = groupId,
            SiteId = siteId,
            IsActive = isActive
        };

        return dynamicRepository.Fetch<User>(criteria).OrderBy(u => u.Username);
    }

    public IQueryable<UserLevel> GetAllByUsernameGroupSite(GetAllByUsernameGroupSiteUserCriteria criteria)
    {
        return userRepository.GetAllByUsernameGroupSite(criteria);
    }

    public IQueryable<UserLevel> SearchUser(SearchUserCriteria criteria)
    {
        return userRepository.Search(criteria);
    }

    public IQueryable<UserLevel> SearchUserExcludeFunction(SearchUserExcludeFunctionCriteria criteria)
    {
        return userRepository.SearchUserExcludeFunction(criteria);
    }

    public int GetUserSiteLevel(int userId, int siteId)
    {
        var criteria = new GetUserSiteLevelUserCriteria
        {
            UserId = userId,
            SiteId = siteId
        };

        return dynamicRepository.Get<int>(criteria);
    }

    public ICommandResult Insert(InsertCustCriteria criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        return userRepository.Insert(criteria, logCriteria);
    }


    public ICommandResult AssignSite(AssignSiteUserCriteria criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        return userRepository.Update(criteria, logCriteria);
    }

    public ICommandResult UpdateSite(UpdateSiteToUserCriteria criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        return userRepository.Update(criteria, logCriteria);
    }

    public ICommandResult AssignFunction(AssignFunctionUserCriteria criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        return userRepository.Update(criteria, logCriteria);
    }

    public int GetUserFunctionPolicy(int userId, int functionId)
    {
        var criteria = new GetUserFunctionPolicyUserCriteria
        {
            UserId = userId,
            FunctionId = functionId
        };

        return dynamicRepository.Get<int>(criteria);
    }

    public int GetUserSiteFunctionKeyPolicy(int userId, int siteId, string functionKey)
    {
        var criteria = new GetUserSiteFunctionKeyPolicyUserCriteria
        {
            UserId = userId,
            SiteId = siteId,
            FunctionKey = functionKey
        };

        return dynamicRepository.Get<int>(criteria);
    }

    public IQueryable<User> SearchByUsername(SearchByUsernameUserCriteria criteria)
    {
        return userRepository.SearchByUsername(criteria);
    }

    public ICommandResult InsertOrUpdate(ICriteria criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        return userRepository.Update(criteria, logCriteria);
    }

    public IEnumerable<UserPolicy> GetByFunctionAndSite(int functionId, int siteId)
    {
        var criteria = new GetByFunctionAndSiteUserCriteria
        {
            FunctionId = functionId,
            SiteId = siteId
        };

        return userRepository.GetUserPolicy(criteria).OrderBy(u => u.Username);
    }

    public Hashtable GetUserListSiteFunctionKeyPolicy(int userId, string functionKey, List<SelectListItem> listSite)
    {
        var criteria = new GetUserListSiteFunctionKeyPolicyUserCriteria
        {
            UserId = userId,
            ListSite = listSite,
            FunctionKey = functionKey
        };

        return userRepository.GetUserListSiteFunctionKeyPolicy(criteria);
    }

    public void SyncUserToOWP(int userId)
    {

        var command = new InsertUserToOWPCommand(userId);
        dynamicRepository.Execute(command);

        if (command.ErrCode > 0)
        {
            throw new ArgumentException($"SyncUserToOWP error: UserId: {userId}; Message: {command.ErrMsg}");
        }

        Logger.Log.Info($"SyncUserToOWP error: UserId: {userId}");
    }

    public bool IgnoredOTP(int userId, string system)
    {
        var userOTP = GetUserOTP(userId, system);

        return userOTP.IgnoreOTP;
    }

    public bool VerifyOTP(int userId, string system, string otp)
    {
        var userOTP = GetUserOTP(userId, system);

        if (userOTP.IgnoreOTP)
        {
            return true;
        }

        if (string.IsNullOrEmpty(userOTP.SecretKey))
        {
            return false;
        }

        var otpService = new OtpServiceImpl();
        return otpService.VerifyOtp(userOTP.SecretKey, otp);
    }

    private SetupOTPItem GetUserOTP(int userId, string system)
    {
        var criteria = new SetupOTPGetCriteria
        {
            CustId = userId,
            System = system
        };

        var repository = new DynamicRepository();

        return repository.Get<SetupOTPItem>(criteria);
    }
}
