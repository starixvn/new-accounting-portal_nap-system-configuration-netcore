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
using Criteria.Trace;
using Criteria.User;
using Domain;
using Models.ValueObject;
using Repository;
using ONEbook.OTP.Services;

public interface IUserService
{
    void SyncUserToOWP(int userId);

    bool IgnoredOTP(int userId, string system);

    bool VerifyOTP(int userId, string system, string otp);

    IUser GetById(int id);

    IUser GetByUsername(string username);

    bool CheckUsernameExist(string username);

    IEnumerable<IUser> GetAll();

    IEnumerable<UserPolicy> GetByFunctionAndSite(int functionId, int siteId);

    IEnumerable<IUser> GetBySite(int siteId = 0, bool? isActive = null);

    IEnumerable<IUser> GetByGroupAndSite(int groupId = 0, int siteId = 0, bool? isActive = null);

    IQueryable<UserLevel> GetAllByUsernameGroupSite(GetAllByUsernameGroupSiteUserCriteria criteria);

    IQueryable<UserLevel> SearchUser(SearchUserCriteria criteria);

    IQueryable<UserLevel> SearchUserExcludeFunction(SearchUserExcludeFunctionCriteria criteria);

    IQueryable<IUser> SearchByUsername(SearchByUsernameUserCriteria criteria);

    int GetUserSiteLevel(int userId, int siteId);

    int GetUserFunctionPolicy(int userId, int functionId);

    int GetUserSiteFunctionKeyPolicy(int userId, int siteId, string functionKey);

    bool HasPermissionUserSiteFunctionKey(int userId, int siteId, string functionKey, Constant.Policy policy);

    Hashtable GetUserListSiteFunctionKeyPolicy(int userId, string functionKey, List<SelectListItem> listSite = null);

    ICommandResult ResetPassword(IUser user);

    ICommandResult Insert(IUser user);

    ICommandResult Insert(InsertCustCriteria criteria);

    ICommandResult Update(int userId, string email, string displayName);

    ICommandResult Update(IUser user);

    ICommandResult ChangeUserSiteStatus(int userId, int siteId, bool isActive);

    ICommandResult AssignSite(AssignSiteUserCriteria criteria);

    ICommandResult UpdateSite(UpdateSiteToUserCriteria criteria);

    ICommandResult AssignFunction(AssignFunctionUserCriteria criteria);

    ICommandResult ChangePassword(ICriteria criteria);

    ICommandResult LoginOnUser(string netSessionId, int serverId);

    ICommandResult Logout();

    ICommandResult ReLogin();

    void UserCheckIn(UserCheckInUserCriteria criteria);

    void ReUserCheckIn(UserReCheckInUserCriteria criteria);

    void UserLogOut(LogOutUserCriteria criteria);

    void UserSSOCheckIn(UserSSOCheckInUserCriteria criteria);

    ICommandResult InsertOrUpdate(ICriteria criteria);

    string UserSSOGetTime();
}

public class UserService : BaseService, IUserService
{
    private readonly IUserRepository userRepository;
    private readonly ITraceRepository traceRepository;
    private readonly IDynamicRepository dynamicRepository;

    public UserService(IUserRepository repository, ITraceRepository traceRepository, IDynamicRepository dynamicRepository)
    {
        userRepository = repository;
        this.traceRepository = traceRepository;
        this.dynamicRepository = dynamicRepository;
    }

    public IUser GetById(int id)
    {
        var criteria = new GetByIdUserCriteria
        {
            UserId = id
        };

        return dynamicRepository.Get<IUser>(criteria);
    }

    public IUser GetByUsername(string username)
    {
        var criteria = new GetByUsernameUserCriteria
        {
            Username = username
        };

        return dynamicRepository.Get<IUser>(criteria);
    }

    public bool CheckUsernameExist(string username)
    {
        var user = GetByUsername(username);

        return user?.Id != 0;
    }

    public IEnumerable<IUser> GetAll()
    {
        var criteria = new GetAllUserCriteria();

        return dynamicRepository.Fetch<IUser>(criteria).OrderBy(u => u.Username);
    }

    public IEnumerable<IUser> GetBySite(int siteId = 0, bool? isActive = null)
    {
        var criteria = new GetBySiteUserCriteria
        {
            SiteId = siteId,
            IsActive = isActive
        };

        return dynamicRepository.Fetch<IUser>(criteria).OrderBy(u => u.Username);
    }

    public IEnumerable<IUser> GetByGroupAndSite(int groupId = 0, int siteId = 0, bool? isActive = null)
    {
        var criteria = new GetByGroupAndSiteUserCriteria
        {
            GroupId = groupId,
            SiteId = siteId,
            IsActive = isActive
        };

        return dynamicRepository.Fetch<IUser>(criteria).OrderBy(u => u.Username);
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

    public ICommandResult ResetPassword(IUser user)
    {
        throw new NotImplementedException();
    }

    public ICommandResult Insert(IUser user)
    {
        throw new NotImplementedException();
    }

    public ICommandResult Insert(InsertCustCriteria criteria)
    {
        var logCriteria = GetLogExecSpCriteria();

        return userRepository.Insert(criteria, logCriteria);
    }

    public ICommandResult Update(int userId, string email, string displayName)
    {
        throw new NotImplementedException();
    }

    public ICommandResult Update(IUser user)
    {
        throw new NotImplementedException();
    }

    public ICommandResult ChangeUserSiteStatus(int userId, int siteId, bool isActive)
    {
        var criteria = new ChangeUserSiteStatusUserCriteria
        {
            UserId = userId,
            SiteId = siteId,
            IsActive = isActive
        };

        var logCriteria = GetLogExecSpCriteria();

        return userRepository.Update(criteria, logCriteria);
    }

    public ICommandResult AssignSite(AssignSiteUserCriteria criteria)
    {
        var logCriteria = GetLogExecSpCriteria();

        return userRepository.Update(criteria, logCriteria);
    }

    public ICommandResult UpdateSite(UpdateSiteToUserCriteria criteria)
    {
        var logCriteria = GetLogExecSpCriteria();

        return userRepository.Update(criteria, logCriteria);
    }

    public ICommandResult AssignFunction(AssignFunctionUserCriteria criteria)
    {
        var logCriteria = GetLogExecSpCriteria();

        return userRepository.Update(criteria, logCriteria);
    }

    public ICommandResult ChangePassword(ICriteria criteria)
    {
        throw new NotImplementedException();
    }

    public ICommandResult LoginOnUser(string netSessionId, int serverId)
    {
        throw new NotImplementedException();
    }

    public ICommandResult Logout()
    {
        throw new NotImplementedException();
    }

    public ICommandResult ReLogin()
    {
        throw new NotImplementedException();
    }

    public void UserCheckIn(UserCheckInUserCriteria criteria)
    {
        userRepository.UserCheckIn(criteria);

        var logCriteria = new InsertLogUserLoginCriteria
        {
            UserId = criteria.UserId,
            SiteId = criteria.SiteId,
            LogDate = criteria.LoginDate,
            DomainName = criteria.Domain,
            BrowserName = criteria.BrowserName,
            Platform = criteria.Platform,
            Doing = criteria.Doing,
            IP = criteria.Ip,
            CountryCode = criteria.CountryCode,
            Duration = criteria.Duration
        };

        traceRepository.InsertLog(logCriteria);
    }

    public void ReUserCheckIn(UserReCheckInUserCriteria criteria)
    {
        userRepository.UserReCheckIn(criteria);
    }

    public void UserLogOut(LogOutUserCriteria criteria)
    {
        userRepository.UserLogOut(criteria);
    }

    public void UserSSOCheckIn(UserSSOCheckInUserCriteria criteria)
    {
        userRepository.UserSSOCheckIn(criteria);

        var logCriteria = new InsertLogUserLoginCriteria
        {
            UserId = criteria.UserId,
            SiteId = criteria.SiteId,
            LogDate = criteria.LoginDate,
            DomainName = criteria.Domain,
            BrowserName = criteria.BrowserName,
            Platform = criteria.Platform,
            Doing = criteria.Doing,
            IP = criteria.IP,
            CountryCode = criteria.CountryCode,
            Duration = criteria.Duration
        };

        traceRepository.InsertLog(logCriteria);
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

    public IQueryable<IUser> SearchByUsername(SearchByUsernameUserCriteria criteria)
    {
        if (criteria.IsValid())
        {
            return userRepository.SearchByUsername(criteria);
        }

        return null;
    }

    public ICommandResult InsertOrUpdate(ICriteria criteria)
    {
        var logCriteria = GetLogExecSpCriteria();

        if (criteria.IsValid() && logCriteria.IsValid())
        {
            return userRepository.Update(criteria, logCriteria);
        }

        return null;
    }

    public IEnumerable<UserPolicy> GetByFunctionAndSite(int functionId, int siteId)
    {
        var criteria = new GetByFunctionAndSiteUserCriteria
        {
            FunctionId = functionId,
            SiteId = siteId
        };

        if (criteria.IsValid())
        {
            return userRepository.GetUserPolicy(criteria).OrderBy(u => u.User.Username);
        }

        return null;
    }

    public Hashtable GetUserListSiteFunctionKeyPolicy(int userId, string functionKey, List<SelectListItem> listSite = null)
    {
        var criteria = new GetUserListSiteFunctionKeyPolicyUserCriteria
        {
            UserId = userId,
            ListSite = listSite,
            FunctionKey = functionKey
        };

        if (criteria.IsValid())
        {
            return userRepository.GetUserListSiteFunctionKeyPolicy(criteria);
        }

        return null;
    }

    public bool HasPermissionUserSiteFunctionKey(int userId, int siteId, string functionKey, Constant.Policy policy)
    {
        var tempPolicy = GetUserSiteFunctionKeyPolicy(userId, siteId, functionKey);

        if (tempPolicy >= (int)policy)
        {
            return true;
        }

        return false;
    }

    public string UserSSOGetTime()
    {
        var criteria = new UserSSOGetTimeUserCriteria
        {
            Time = DateTime.Now
        };

        return userRepository.UserSSOGetTime(criteria);
    }

    public void SyncUserToOWP(int userId)
    {

        var command = new InsertUserToOWPCommand(userId);

        using (var db = ObjectDbFactory.CreateInstance(command.GetSettingKey()))
        {
            db.ExecuteNonQuery(command);
        }

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
