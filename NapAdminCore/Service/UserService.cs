namespace NapAdminCore.Service;

using System.Collections;
using Fanex.Data.Repository;
using Fanex.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Criteria.User;
using Models.ValueObject;
using Repository;
using ONEbook.OTP.Services;
using Criteria;
using Criteria.OTP;

public interface IUserService
{
    string SyncUserToOWP(int userId);

    bool IgnoredOTP(int userId, string system);

    bool VerifyOTP(int userId, string system, string otp);

    User GetById(int id);

    User GetByUsername(string username);

    bool CheckUsernameExist(string username);

    IList<UserPolicy> GetByFunctionAndSite(int functionId, int siteId);

    IList<User> GetByGroupAndSite(int groupId = 0, int siteId = 0, bool? isActive = null);

    IList<UserLevel> GetAllByUsernameGroupSite(GetAllByUsernameGroupSiteUserCriteria criteria);

    IList<UserLevel> SearchUser(SearchUserCriteria criteria);

    IList<UserLevel> SearchUserExcludeFunction(SearchUserExcludeFunctionCriteria criteria);

    IList<User> SearchByUsername(SearchByUsernameUserCriteria criteria);

    int GetUserSiteLevel(int userId, int siteId);

    int GetUserFunctionPolicy(int userId, int functionId);

    int GetUserSiteFunctionKeyPolicy(int userId, int siteId, string functionKey);

    Hashtable GetUserListSiteFunctionKeyPolicy(int userId, string functionKey, List<SelectListItem> listSite);

    CommandResult Insert(InsertCustCommand criteria);

    CommandResult AssignSite(AssignSiteUserCommand criteria);

    CommandResult UpdateSite(UpdateSiteToUserCommand criteria);

    CommandResult AssignFunction(AssignFunctionUserCommand criteria);

    CommandResult InsertOrUpdate(UpdateCommandBase criteria);
}

public class UserService(ISimpleRepository repository, IDynamicRepository dynamicRepository, LoggedUserContext loggedUserContext)
    : IUserService
{
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

    public IList<User> GetByGroupAndSite(int groupId = 0, int siteId = 0, bool? isActive = null)
    {
        var criteria = new GetByGroupAndSiteUserCriteria
        {
            GroupId = groupId,
            SiteId = siteId,
            IsActive = isActive
        };

        return dynamicRepository.Fetch<User>(criteria).OrderBy(u => u.Username).ToList();
    }

    public IList<UserLevel> GetAllByUsernameGroupSite(GetAllByUsernameGroupSiteUserCriteria criteria)
    {
        return dynamicRepository.Fetch<UserLevel>(criteria).ToList();
    }

    public IList<UserLevel> SearchUser(SearchUserCriteria criteria)
    {
        return dynamicRepository.Fetch<UserLevel>(criteria).ToList();
    }

    public IList<UserLevel> SearchUserExcludeFunction(SearchUserExcludeFunctionCriteria criteria)
    {
        return dynamicRepository.Fetch<UserLevel>(criteria).ToList();
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

    public CommandResult Insert(InsertCustCommand criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        return repository.Execute(criteria, logCriteria);
    }


    public CommandResult AssignSite(AssignSiteUserCommand criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        return repository.Execute(criteria, logCriteria);
    }

    public CommandResult UpdateSite(UpdateSiteToUserCommand criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        return repository.Execute(criteria, logCriteria);
    }

    public CommandResult AssignFunction(AssignFunctionUserCommand criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        return repository.Execute(criteria, logCriteria);
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

    public IList<User> SearchByUsername(SearchByUsernameUserCriteria criteria)
    {
        return dynamicRepository.Fetch<User>(criteria).ToList();
    }

    public CommandResult InsertOrUpdate(UpdateCommandBase criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        return repository.Execute(criteria, logCriteria);
    }

    public IList<UserPolicy> GetByFunctionAndSite(int functionId, int siteId)
    {
        var criteria = new GetByFunctionAndSiteUserCriteria
        {
            FunctionId = functionId,
            SiteId = siteId
        };

        return dynamicRepository.Fetch<UserPolicy>(criteria).OrderBy(u => u.Username).ToList();
    }

    public Hashtable GetUserListSiteFunctionKeyPolicy(int userId, string functionKey, List<SelectListItem> listSite)
    {
        var criteria = new GetUserListSiteFunctionKeyPolicyUserCriteria
        {
            UserId = userId,
            ListSite = listSite,
            FunctionKey = functionKey
        };

        var models = dynamicRepository.Fetch<SitePolicy>(criteria);

        var hashTable = new Hashtable();
        foreach (var model in models)
        {
            if (!hashTable.ContainsKey(model.SiteId))
            {
                hashTable.Add(model.SiteId, model.Policy);
            }
        }
        
        return hashTable;
    }

    public string SyncUserToOWP(int userId)
    {

        var command = new InsertUserToOWPCommand(userId);
        dynamicRepository.Execute(command);

        if (command.ErrCode > 0)
        {
            var message = $"SyncUserToOWP error: UserId: {userId}; Message: {command.ErrMsg}";

            Logger.Log.Error(message);
            return message;
        }

        return AdminConstant.Ok;
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
