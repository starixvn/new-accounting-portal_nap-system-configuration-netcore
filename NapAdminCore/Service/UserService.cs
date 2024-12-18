namespace NapAdminCore.Service;

using System.Collections;
using Fanex.Data.Repository;
using Fanex.Logging;
using Models;
using Criteria.User;
using Models.ValueObject;
using Repository;
using Criteria;
using Criteria.OTP;
using NapApiCaching;
using Criteria.Misc;
using ONEbook.OTP;

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

    Hashtable GetUserListSiteFunctionKeyPolicy(int userId, string functionKey, List<int> listSite);

    CommandResult Insert(InsertCust0Command command);

    CommandResult AssignSite(AssignSiteUserCommand criteria);

    CommandResult UpdateSite(UpdateSiteToUserCommand criteria);

    CommandResult AssignFunction(AssignFunctionUserCommand criteria);

    CommandResult InsertOrUpdate(UpdateCommandBase criteria);
}

public class UserService(UpdateRepository repository, IDynamicRepository dynamicRepository, LoggedUserContext loggedUserContext)
    : IUserService
{

    public sealed class InsertCustException(int code, string message, Exception e) : Exception(message, e)
    {
        public int ErrorCode { get; } = code;

        public string ErrorMessage { get; } = message;
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

    public CommandResult Insert(InsertCust0Command command)
    {
        var globCommand = InsertStep1GlobDatabase(command);
        if (globCommand.ErrCode != 0)
        {
            return new CommandResult
            {
                Code = globCommand.ErrCode,
                Message = globCommand.ErrMsg
            };
        }

        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        try
        {
            InsertStep2AdminDatabase(command, globCommand.CustId, logCriteria.CurrentUserId);
            InsertStep3DotAdminDatabase(command, globCommand.CustId, logCriteria.CurrentUserId);
            InsertStep4PolDatabase(command, globCommand.CustId, logCriteria);
            command.CustId = globCommand.CustId;
        }
        catch (InsertCustException e)
        {
            InsertStep5RollbackCustDatabase(globCommand.CustId);
            InsertStep6RollbackGlobDatabase(globCommand.CustId);
            return new CommandResult
            {
                Code = e.ErrorCode,
                Message = e.Message
            };
        }
        
        return new CommandResult
        {
            Code = 0,
            Message = string.Empty
        };
    }

    private InsertCust1GlobCommand InsertStep1GlobDatabase(InsertCust0Command command)
    {
        var globCommand = new InsertCust1GlobCommand
        {
            Username = command.Username
        };
        try
        {
            dynamicRepository.Execute(globCommand);
        }
        catch (Exception e)
        {
            globCommand.ErrCode = 1;
            globCommand.ErrMsg = e.Message;
        }

        return globCommand;
    }


    private void InsertStep2AdminDatabase(InsertCust0Command command, int custId, int currentUserId)
    {
        var adminCommand = new InsertCust2AdminCommand
        {
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            UserName = command.Username,
            UserPwd = command.UserPwd,
            CustId = custId,
            CurrentUserId = currentUserId
        };

        try
        {
            dynamicRepository.Execute(adminCommand);
            if (adminCommand.ErrCode > 0)
            {
                throw new InsertCustException(adminCommand.ErrCode, adminCommand.ErrMsg, null);
            }
        }
        catch (Exception e)
        {
            throw new InsertCustException(1, e.Message, e);
        }
    }

    private void InsertStep3DotAdminDatabase(InsertCust0Command command, int custId, int currentUserId)
    {
        var dotAdminCommand = new InsertCust3DotAdminCommand
        {
            CustID = custId,
            UserName = command.Username,
            UserPWD = command.UserPwd,
            Creator = currentUserId
        };

        try
        {
            dynamicRepository.Execute(dotAdminCommand);
        }
        catch (Exception e)
        {
            throw new InsertCustException(1, e.Message, e);
        }
    }

    private void InsertStep4PolDatabase(InsertCust0Command command, int custId, LogExecSpCriteria logExec)
    {
        var adminCommand = new InsertCust4Command
        {
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            Username = command.Username,
            UserPwd = command.UserPwd,
            CustId = custId,
            CurrentUserId = logExec.CurrentUserId,
            CurrentSiteId = logExec.CurrentSiteId,
            GroupId = command.GroupId,
            Language = "en-us",
            LogDate = logExec.LogDate,
            LoginIP = logExec.LoginIp
        };

        try
        {
            dynamicRepository.Execute(adminCommand);
            if (adminCommand.ErrCode != 0)
            {
                throw new InsertCustException(adminCommand.ErrCode, adminCommand.ErrMsg, null);
            }
        }
        catch (Exception e)
        {
            throw new InsertCustException(1, e.Message, e);
        }
    }

    private void InsertStep5RollbackCustDatabase(int custId)
    {
        var command = new InsertCust5RollbackAdminCommand
        {
            CustID = custId
        };

        dynamicRepository.Execute(command);
    }

    private void InsertStep6RollbackGlobDatabase(int custId)
    {
        var command = new InsertCust6RollbackGlobCommand
        {
            CustID = custId
        };

        dynamicRepository.Execute(command);
    }

    public CommandResult AssignSite(AssignSiteUserCommand criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        var result = repository.Execute(criteria, logCriteria);

        DataCachingJobCommands.Instance.AddUserSiteLevelCommand();
        DataCachingJobCommands.Instance.AddUserFunctionPolicyCommand(criteria.SiteId);
        
        return result;
    }

    public CommandResult UpdateSite(UpdateSiteToUserCommand criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        var result = repository.Execute(criteria, logCriteria);

        DataCachingJobCommands.Instance.AddSiteCommand();
        DataCachingJobCommands.Instance.AddUserSiteLevelCommand();
        DataCachingJobCommands.Instance.AddUserFunctionPolicyCommand(criteria.SiteId);

        return result;
    }

    public CommandResult AssignFunction(AssignFunctionUserCommand criteria)
    {
        var logCriteria = loggedUserContext.GetLogExecSpCriteria();

        var result = repository.Execute(criteria, logCriteria);

        DataCachingJobCommands.Instance.AddUserFunctionPolicyCommand(criteria.SiteId);
        return result;
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
        var result = repository.Execute(criteria, logCriteria);

        return result;
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

    public Hashtable GetUserListSiteFunctionKeyPolicy(int userId, string functionKey, List<int> listSite)
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

        var otpService = new OtpServiceImp();
        return otpService.VerifyOtp(userOTP.SecretKey, otp);
    }

    private SetupOTPItem GetUserOTP(int userId, string system)
    {
        var criteria = new SetupOTPGetCriteria
        {
            CustId = userId,
            System = system
        };

        return dynamicRepository.Get<SetupOTPItem>(criteria);
    }
}
