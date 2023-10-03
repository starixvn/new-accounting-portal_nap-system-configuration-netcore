import { Site, SiteLevel, SiteLevelValue } from '../model/site';
import { Function, FunctionPolicy, FunctionPolicyValue } from '../model/function';
import MessageBox from './message-box';
import configs from '../configs/configs';

export default class FunctionUtil {
    public static readonly WinLossByProductsWarningMessage = 'You must select ONLY ONE function ';

    public static SetCurrentFunctionPolicy(site: Site, functionId: number, rootFuncUserPolicy: boolean, isRead: boolean) {
        const currentFunc = site.functions.find(f => f.id === functionId);

        if (isRead) {
            if (currentFunc.hasUserHostPolicy && !currentFunc.disableHost) {
                currentFunc.hasUserHostPolicy = rootFuncUserPolicy;
            }
        } else {
            if (!currentFunc.hasUserReadPolicy && !currentFunc.disableRead) {
                currentFunc.hasUserReadPolicy = rootFuncUserPolicy;
            }
        }
    }

    public static SetChildrenFunctionPolicy(site: Site, functionId: number, rootFuncUserPolicy: boolean, isRead: boolean) {
        const childrenFunc = site.functions.filter(f => f.parentId === functionId);

        for (const func of childrenFunc) {
            if (
                rootFuncUserPolicy &&
                configs.WinLossByProductsFunctionNames.some(name => this.CheckFunctionIsWinLossByProducts(func, name))
            ) {
                continue;
            }

            if (isRead && !func.disableRead) {
                func.hasUserReadPolicy = rootFuncUserPolicy;
            } else if (!func.disableHost) {
                func.hasUserHostPolicy = rootFuncUserPolicy;
            }

            this.SetCurrentFunctionPolicy(site, func.id, rootFuncUserPolicy, isRead);
            this.SetChildrenFunctionPolicy(site, func.id, rootFuncUserPolicy, isRead);
        }
    }

    public static SetParentFunctionPolicy(site: Site, functionId: number, rootFuncUserPolicy: boolean, isRead: boolean) {
        const parentFuncId = site.functions.find(f => f.id === functionId).parentId;

        if (parentFuncId) {
            let existChildrenFuncHasUserRead = false;
            const childrenFunc = site.functions.filter(f => f.parentId === parentFuncId);

            for (const func of childrenFunc) {
                if ((isRead && func.hasUserReadPolicy) || (!isRead && func.hasUserHostPolicy)) {
                    existChildrenFuncHasUserRead = true;
                }
            }

            const parentFunc = site.functions.find(f => f.id === parentFuncId);

            if (isRead) {
                parentFunc.hasUserReadPolicy = existChildrenFuncHasUserRead ? true : rootFuncUserPolicy;
            } else {
                parentFunc.hasUserHostPolicy = existChildrenFuncHasUserRead ? true : rootFuncUserPolicy;
            }

            this.SetCurrentFunctionPolicy(site, parentFunc.id, rootFuncUserPolicy, isRead);
            this.SetParentFunctionPolicy(site, parentFunc.id, rootFuncUserPolicy, isRead);
        }
    }

    public static SetAllFunctionsReadPolicy(site: Site, rootPolicy: boolean) {
        site.functions.forEach(f =>
            (rootPolicy && configs.WinLossByProductsFunctionNames.some(name => this.CheckFunctionIsWinLossByProducts(f, name))) ||
            f.disableRead
                ? (f.hasUserReadPolicy = f.hasUserReadPolicy)
                : (f.hasUserReadPolicy = rootPolicy)
        );

        site.hasAllFunctionUserReadPolicy = rootPolicy;
    }

    public static SetAllFunctionsHostPolicy(site: Site, rootPolicy: boolean) {
        site.functions.forEach(f => (f.disableHost ? (f.hasUserHostPolicy = f.hasUserHostPolicy) : (f.hasUserHostPolicy = rootPolicy)));
        site.hasAllFunctionUserHostPolicy = rootPolicy;
    }

    public static SetSiteFunctionPolicy(site: Site) {
        site.hasAllFunctionUserReadPolicy = site.functions.every(f => f.hasUserReadPolicy);
        site.hasAllFunctionUserHostPolicy = site.functions.filter(f => f.crud > 0).every(f => f.hasUserHostPolicy);
    }

    public static InitFunctionPolicies(site: Site) {
        let allFuncHasHostPolicy = true;
        let allFuncHasReadPolicy = true;
        let allFuncHasGroupHostPolicy = true;
        let allFuncHasGroupReadPolicy = true;

        for (const func of site.functions) {
            const policy = site.viewedUserFunctionPolicies.find(fp => fp.id === func.id);
            const groupPolicy = site.viewedUserGroupFunctionPolicies.find(fp => fp.id === func.id);

            if (policy !== undefined) {
                func.hasUserReadPolicy = policy.policy >= FunctionPolicyValue.Read;
                func.hasUserHostPolicy = policy.policy >= FunctionPolicyValue.Host;
            }

            if (groupPolicy !== undefined) {
                func.hasGroupReadPolicy = groupPolicy.policy >= FunctionPolicyValue.Read;
                func.hasGroupHostPolicy = groupPolicy.policy >= FunctionPolicyValue.Host;
            }

            if (func.hasUserHostPolicy === undefined || (!func.hasUserHostPolicy && func.crud > 0)) {
                allFuncHasHostPolicy = false;
            }

            if (func.hasUserReadPolicy === undefined || !func.hasUserReadPolicy) {
                allFuncHasReadPolicy = false;
            }

            if (func.hasGroupHostPolicy === undefined || (!func.hasGroupHostPolicy && func.crud > 0)) {
                allFuncHasGroupHostPolicy = false;
            }

            if (allFuncHasGroupReadPolicy === undefined || !func.hasGroupReadPolicy) {
                allFuncHasGroupReadPolicy = false;
            }
        }

        site.hasAllFunctionUserHostPolicy = allFuncHasHostPolicy;
        site.hasAllFunctionUserReadPolicy = allFuncHasReadPolicy || allFuncHasHostPolicy;
        site.hasAllFunctionGroupHostPolicy = allFuncHasGroupHostPolicy;
        site.hasAllFunctionGroupReadPolicy = allFuncHasGroupReadPolicy || allFuncHasGroupHostPolicy;
    }

    public static DisableFunctionPolicies(
        site: Site,
        loginUserIsPortalAdmin: boolean,
        loginUserAssignFunctionPermissionInAdminSite: number,
        isSelfEdit: boolean
    ) {
        for (const func of site.functions) {
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
            func.disableRead = isDisabledRead;
            func.disableHost = isDisabledHost;
            func.isHidden =
                !loginUserIsPortalAdmin &&
                (site.loginUserAssignFunctionPermission < FunctionPolicyValue.Read ||
                    loginUserAssignFunctionPermissionInAdminSite < FunctionPolicyValue.Read);
        }

        site.disableCheckBoxAllFunctionReadPolicy = site.functions.every(f => f.disableRead);
        site.disableCheckBoxAllFunctionHostPolicy = site.functions.every(f => f.disableHost);
        site.hideCheckBoxAllFunctionPolicy = site.functions.every(f => f.isHidden);
    }

    public static CheckDisableFunctionPolicy(
        site: Site,
        func: Function,
        isRead: boolean,
        loginUserIsPortalAdmin: boolean,
        loginUserAssignFunctionPermissionInAdminSite: number,
        isSelfEdit: boolean
    ) {
        const viewedUserPolicy = site.viewedUserFunctionPolicies.find(fp => fp.id === func.id);
        const loginUserPolicy = site.loginUserFunctionPolicies.find(fp => fp.id === func.id);

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
        const func = site.functions.find(f => f.id === currentFunctionId);
        let existPermission = false;

        if (this.HasNameOrParentNameIsWinLossByProducts(func, winLossFunctionName)) {
            existPermission = site.functions.some(
                f =>
                    ((f.id !== currentFunctionId && this.HasNameOrParentNameIsWinLossByProducts(f, winLossFunctionName)) ||
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
        return func.name === winLossFunctionName;
    }

    private static HasParentNameIsWinLossByProducts(func: Function, winLossFunctionName: string) {
        return func.parentName === winLossFunctionName;
    }

    private static HasReadPolicyIncludeGroup(func: Function) {
        return func.hasUserReadPolicy || func.hasGroupReadPolicy;
    }

    private static ViewedUserSiteLevelIsAdmin(site: Site) {
        return site.siteLevel !== undefined && site.siteLevel.level >= SiteLevelValue.SiteAdmin;
    }

    private static LoginUserPolicyIsSmallerThanViewedUserPolicy(viewdUserPolicy: FunctionPolicy, loginUserPolicy: FunctionPolicy) {
        return viewdUserPolicy !== undefined && loginUserPolicy.policy < viewdUserPolicy.policy;
    }

    private static LoginUserHasNoPolicy(loginUserPolicy: FunctionPolicy, isRead: boolean) {
        return loginUserPolicy === undefined || loginUserPolicy.policy < (isRead ? FunctionPolicyValue.Read : FunctionPolicyValue.Host);
    }

    private static LoginUserHasAssignFunctionReadPermissionInCurrentSiteAndAdminSite(
        site: Site,
        loginUserAssignFunctionPermissionInAdminSite: number
    ): boolean {
        return (
            site.loginUserAssignFunctionPermission <= FunctionPolicyValue.Read ||
            loginUserAssignFunctionPermissionInAdminSite <= FunctionPolicyValue.Read
        );
    }
}
