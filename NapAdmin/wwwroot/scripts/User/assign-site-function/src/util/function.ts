import { Site, SiteLevel, SiteLevelValue } from '../model/site';
import { Function, FunctionPolicy, FunctionPolicyValue } from '../model/function';
import MessageBox from './message-box';
import configs from '../configs/configs';

export default class FunctionUtil {
    public static readonly WinLossByProductsWarningMessage = 'You must select ONLY ONE function ';

    public static SetCurrentFunctionPolicy(site: Site, functionId: number, rootFuncUserPolicy: boolean, isRead: boolean) {
        const currentFunc = site.Functions.find(f => f.Id === functionId);

        if (isRead) {
            if (currentFunc.HasUserHostPolicy && !currentFunc.DisableHost) {
                currentFunc.HasUserHostPolicy = rootFuncUserPolicy;
            }
        } else {
            if (!currentFunc.HasUserReadPolicy && !currentFunc.DisableRead) {
                currentFunc.HasUserReadPolicy = rootFuncUserPolicy;
            }
        }
    }

    public static SetChildrenFunctionPolicy(site: Site, functionId: number, rootFuncUserPolicy: boolean, isRead: boolean) {
        const childrenFunc = site.Functions.filter(f => f.Parent !== null && f.Parent.Id === functionId);

        for (const func of childrenFunc) {
            if (
                rootFuncUserPolicy &&
                configs.WinLossByProductsFunctionNames.some(name => this.CheckFunctionIsWinLossByProducts(func, name))
            ) {
                continue;
            }

            if (isRead && !func.DisableRead) {
                func.HasUserReadPolicy = rootFuncUserPolicy;
            } else if (!func.DisableHost) {
                func.HasUserHostPolicy = rootFuncUserPolicy;
            }

            this.SetCurrentFunctionPolicy(site, func.Id, rootFuncUserPolicy, isRead);
            this.SetChildrenFunctionPolicy(site, func.Id, rootFuncUserPolicy, isRead);
        }
    }

    public static SetParentFunctionPolicy(site: Site, functionId: number, rootFuncUserPolicy: boolean, isRead: boolean) {
        let parentFunc = site.Functions.find(f => f.Id === functionId).Parent;

        if (parentFunc !== null) {
            let existChildrenFuncHasUserRead = false;
            const childrenFunc = site.Functions.filter(f => f.Parent !== null && f.Parent.Id === parentFunc.Id);

            for (const func of childrenFunc) {
                if ((isRead && func.HasUserReadPolicy) || (!isRead && func.HasUserHostPolicy)) {
                    existChildrenFuncHasUserRead = true;
                }
            }

            parentFunc = site.Functions.find(f => f.Id === parentFunc.Id);

            if (isRead) {
                parentFunc.HasUserReadPolicy = existChildrenFuncHasUserRead ? true : rootFuncUserPolicy;
            } else {
                parentFunc.HasUserHostPolicy = existChildrenFuncHasUserRead ? true : rootFuncUserPolicy;
            }

            this.SetCurrentFunctionPolicy(site, parentFunc.Id, rootFuncUserPolicy, isRead);
            this.SetParentFunctionPolicy(site, parentFunc.Id, rootFuncUserPolicy, isRead);
        }
    }

    public static SetAllFunctionsReadPolicy(site: Site, rootPolicy: boolean) {
        site.Functions.forEach(f =>
            (rootPolicy && configs.WinLossByProductsFunctionNames.some(name => this.CheckFunctionIsWinLossByProducts(f, name))) ||
            f.DisableRead
                ? (f.HasUserReadPolicy = f.HasUserReadPolicy)
                : (f.HasUserReadPolicy = rootPolicy)
        );

        site.HasAllFunctionUserReadPolicy = rootPolicy;
    }

    public static SetAllFunctionsHostPolicy(site: Site, rootPolicy: boolean) {
        site.Functions.forEach(f => (f.DisableHost ? (f.HasUserHostPolicy = f.HasUserHostPolicy) : (f.HasUserHostPolicy = rootPolicy)));
        site.HasAllFunctionUserHostPolicy = rootPolicy;
    }

    public static SetSiteFunctionPolicy(site: Site) {
        site.HasAllFunctionUserReadPolicy = site.Functions.every(f => f.HasUserReadPolicy);
        site.HasAllFunctionUserHostPolicy = site.Functions.filter(f => f.Crud > 0).every(f => f.HasUserHostPolicy);
    }

    public static InitFunctionPolicies(site: Site) {
        let allFuncHasHostPolicy = true;
        let allFuncHasReadPolicy = true;
        let allFuncHasGroupHostPolicy = true;
        let allFuncHasGroupReadPolicy = true;

        for (const func of site.Functions) {
            const policy = site.ViewedUserFunctionPolicies.find(fp => fp.Function.Id === func.Id);
            const groupPolicy = site.ViewedUserGroupFunctionPolicies.find(fp => fp.Function.Id === func.Id);

            if (policy !== undefined) {
                func.HasUserReadPolicy = policy.Policy >= FunctionPolicyValue.Read;
                func.HasUserHostPolicy = policy.Policy >= FunctionPolicyValue.Host;
            }

            if (groupPolicy !== undefined) {
                func.HasGroupReadPolicy = groupPolicy.Policy >= FunctionPolicyValue.Read;
                func.HasGroupHostPolicy = groupPolicy.Policy >= FunctionPolicyValue.Host;
            }

            if (func.HasUserHostPolicy === undefined || (!func.HasUserHostPolicy && func.Crud > 0)) {
                allFuncHasHostPolicy = false;
            }

            if (func.HasUserReadPolicy === undefined || !func.HasUserReadPolicy) {
                allFuncHasReadPolicy = false;
            }

            if (func.HasGroupHostPolicy === undefined || (!func.HasGroupHostPolicy && func.Crud > 0)) {
                allFuncHasGroupHostPolicy = false;
            }

            if (allFuncHasGroupReadPolicy === undefined || !func.HasGroupReadPolicy) {
                allFuncHasGroupReadPolicy = false;
            }
        }

        site.HasAllFunctionUserHostPolicy = allFuncHasHostPolicy;
        site.HasAllFunctionUserReadPolicy = allFuncHasReadPolicy || allFuncHasHostPolicy;
        site.HasAllFunctionGroupHostPolicy = allFuncHasGroupHostPolicy;
        site.HasAllFunctionGroupReadPolicy = allFuncHasGroupReadPolicy || allFuncHasGroupHostPolicy;
    }

    public static DisableFunctionPolicies(
        site: Site,
        loginUserIsPortalAdmin: boolean,
        loginUserAssignFunctionPermissionInAdminSite: number,
        isSelfEdit: boolean
    ) {
        for (const func of site.Functions) {
            const isDisabledRead = FunctionUtil.CheckDisableFunctionPolicy(
                site,
                func,
                true,
                loginUserIsPortalAdmin,
                loginUserAssignFunctionPermissionInAdminSite,
                isSelfEdit
            );
            const isDisabledHost = FunctionUtil.CheckDisableFunctionPolicy(
                site,
                func,
                false,
                loginUserIsPortalAdmin,
                loginUserAssignFunctionPermissionInAdminSite,
                isSelfEdit
            );
            func.DisableRead = isDisabledRead;
            func.DisableHost = isDisabledHost;
            func.IsHidden =
                !loginUserIsPortalAdmin &&
                (site.LoginUserAssignFunctionPermission < FunctionPolicyValue.Read ||
                    loginUserAssignFunctionPermissionInAdminSite < FunctionPolicyValue.Read);
        }

        site.DisableCheckBoxAllFunctionReadPolicy = site.Functions.every(f => f.DisableRead);
        site.DisableCheckBoxAllFunctionHostPolicy = site.Functions.every(f => f.DisableHost);
        site.HideCheckBoxAllFunctionPolicy = site.Functions.every(f => f.IsHidden);
    }

    public static CheckDisableFunctionPolicy(
        site: Site,
        func: Function,
        isRead: boolean,
        loginUserIsPortalAdmin: boolean,
        loginUserAssignFunctionPermissionInAdminSite: number,
        isSelfEdit: boolean
    ) {
        const viewedUserPolicy = site.ViewedUserFunctionPolicies.find(fp => fp.Function.Id === func.Id);
        const loginUserPolicy = site.LoginUserFunctionPolicies.find(fp => fp.Function.Id === func.Id);

        const isDisabled =
            this.ViewedUserSiteLevelIsAdmin(site) ||
            this.LoginUserHasAssignFunctionReadPermissionInCurrentSiteAndAdminSite(site, loginUserAssignFunctionPermissionInAdminSite) ||
            isSelfEdit ||
            (!loginUserIsPortalAdmin &&
                (this.LoginUserHasNoPolicy(loginUserPolicy, isRead) ||
                    this.LoginUserPolicyIsSmallerThanViewedUserPolicy(viewedUserPolicy, loginUserPolicy)));

        return isDisabled;
    }

    public static CheckExistFunctionPermissionInWinLossByProducts(
        site: Site,
        currentFunctionId: number,
        winLossFunctionName: string
    ): boolean {
        const func = site.Functions.find(f => f.Id === currentFunctionId);
        let existPermission = false;

        if (this.HasNameOrParentNameIsWinLossByProducts(func, winLossFunctionName)) {
            existPermission = site.Functions.some(
                f =>
                    ((f.Id !== currentFunctionId && this.HasNameOrParentNameIsWinLossByProducts(f, winLossFunctionName)) ||
                        this.HasNameIsWinLossByProducts(f, winLossFunctionName)) &&
                    this.HasReadPolicyIncludeGroup(f)
            );

            if (existPermission) {
                const message = this.WinLossByProductsWarningMessage + winLossFunctionName;
                MessageBox.ShowError(message);
            }
        }

        return existPermission;
    }

    public static CheckFunctionIsWinLossByProducts(func: Function, winLossFunctionName: string): boolean {
        return this.HasNameOrParentNameIsWinLossByProducts(func, winLossFunctionName);
    }

    private static HasNameOrParentNameIsWinLossByProducts(func: Function, winLossFunctionName: string) {
        return (
            this.HasNameIsWinLossByProducts(func, winLossFunctionName) || this.HasParentNameIsWinLossByProducts(func, winLossFunctionName)
        );
    }

    private static HasNameIsWinLossByProducts(func: Function, winLossFunctionName: string) {
        return func.Name === winLossFunctionName;
    }

    private static HasParentNameIsWinLossByProducts(func: Function, winLossFunctionName: string) {
        return func.Parent !== null && func.Parent !== undefined && func.Parent.Name === winLossFunctionName;
    }

    private static HasReadPolicyIncludeGroup(func: Function) {
        return func.HasUserReadPolicy || func.HasGroupReadPolicy;
    }

    private static ViewedUserSiteLevelIsAdmin(site: Site) {
        return site.SiteLevel !== undefined && site.SiteLevel.Level >= SiteLevelValue.SiteAdmin;
    }

    private static LoginUserPolicyIsSmallerThanViewedUserPolicy(viewdUserPolicy: FunctionPolicy, loginUserPolicy: FunctionPolicy) {
        return viewdUserPolicy !== undefined && loginUserPolicy.Policy < viewdUserPolicy.Policy;
    }

    private static LoginUserHasNoPolicy(loginUserPolicy: FunctionPolicy, isRead: boolean) {
        return loginUserPolicy === undefined || loginUserPolicy.Policy < (isRead ? FunctionPolicyValue.Read : FunctionPolicyValue.Host);
    }

    private static LoginUserHasAssignFunctionReadPermissionInCurrentSiteAndAdminSite(
        site: Site,
        loginUserAssignFunctionPermissionInAdminSite: number
    ): boolean {
        return (
            site.LoginUserAssignFunctionPermission <= FunctionPolicyValue.Read ||
            loginUserAssignFunctionPermissionInAdminSite <= FunctionPolicyValue.Read
        );
    }
}
