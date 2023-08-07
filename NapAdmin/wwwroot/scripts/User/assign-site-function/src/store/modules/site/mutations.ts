import Vuex from 'vuex';
import * as Type from '../../types';
import { Site, SiteLevel, SiteLevelValue } from '../../../model/site';
import { Function, FunctionPolicy, FunctionPolicyValue } from '../../../model/function';
import { SiteState } from './state';
import DateTimeUtil from '../../../util/datetime';
import FunctionUtil from '../../../util/function';
import * as _ from 'lodash';
import configs from '../../../configs/configs';

export const mutations: Vuex.MutationTree<SiteState> = {
    [Type.Mutation.SetRootRoute](state, rootRoute: string) {
        state.RootRoute = rootRoute;
    },

    [Type.Mutation.SetAllUserSites](state, sites: Array<Site>) {
        sites.forEach(s => (s.showFunctionList = false));
        state.All = sites;
    },

    [Type.Mutation.SetCurrentSiteId](state, currentSiteId: number) {
        state.CurrentSiteId = currentSiteId;
    },

    [Type.Mutation.SetLoginUserHasAssignSiteHostPermissionSites](state, sites: Array<Site>) {
        state.LoginUserHasAssignSiteHostPermissionSites = sites;
    },

    [Type.Mutation.SetViewedUserId](state, viewUserId: string) {
        state.ViewedUserId = viewUserId;
    },

    [Type.Mutation.SetLoginUserHasAssignSitePermission](state, permission: boolean) {
        state.LoginUserHasAssignSitePermission = permission;
    },

    [Type.Mutation.SetLoginUserHasAssignSiteReadPermission](state, permission: boolean) {
        state.LoginUserHasAssignSiteReadPermission = permission;
    },

    [Type.Mutation.SetLoginUserId](state, loginUserId: string) {
        state.LoginUserId = loginUserId;
    },

    [Type.Mutation.SetLoginUserIsPortalAdmin](state, loginUserIsPortalAdmin) {
        state.LoginUserIsPortalAdmin = loginUserIsPortalAdmin;
    },

    [Type.Mutation.SetFunctionsToSite](state, { siteId, functions, loginUserAssignFunctionPermission }) {
        const siteIndex = state.All.findIndex(s => s.id === siteId);
        const site: Site = state.All[siteIndex];
        const isSelfEdit = state.LoginUserId === state.ViewedUserId;
        site.functions = functions;
        site.loginUserAssignFunctionPermission = loginUserAssignFunctionPermission;
        FunctionUtil.InitFunctionPolicies(site);
        FunctionUtil.DisableFunctionPolicies(
            site,
            state.LoginUserIsPortalAdmin,
            state.LoginUserAssignFunctionPermissionInAdminSite,
            isSelfEdit
        );

        state.All.splice(siteIndex, 1, site);
    },

    [Type.Mutation.SetSelectAllUserFunctionsForAllSites](state) {
        state.SelectAllReadFunctionsForAllSites = state.All.every(
            site => site.hasAllFunctionUserReadPolicy || site.disableCheckBoxAllFunctionReadPolicy
        );
        state.SelectAllHostFunctionsForAllSites = state.All.every(
            site => site.hasAllFunctionUserHostPolicy || site.disableCheckBoxAllFunctionHostPolicy
        );
    },

    [Type.Mutation.UpdateSelectAllUserFunctionForAllSites](state, { policy, isRead }) {
        if (isRead) {
            state.SelectAllReadFunctionsForAllSites = policy;

            if (policy === false) {
                state.SelectAllHostFunctionsForAllSites = policy;
            }
        } else {
            state.SelectAllHostFunctionsForAllSites = policy;

            if (policy === true) {
                state.SelectAllReadFunctionsForAllSites = policy;
            }
        }
    },

    [Type.Mutation.SetViewedUserFunctionPolicies](state, { siteId, functionPolicies, groupFunctionPolicies }) {
        const siteIndex = state.All.findIndex(s => s.id === siteId);
        const site: Site = state.All[siteIndex];

        site.viewedUserFunctionPolicies = functionPolicies;
        site.viewedUserGroupFunctionPolicies = groupFunctionPolicies;

        state.All.splice(siteIndex, 1, site);
    },

    [Type.Mutation.SetLoginUserFunctionPolicies](state, { siteId, functionPolicies }) {
        const siteIndex = state.All.findIndex(s => s.id === siteId);
        const site: Site = state.All[siteIndex];

        site.loginUserFunctionPolicies = functionPolicies;

        state.All.splice(siteIndex, 1, site);
    },

    [Type.Mutation.SetSiteLevelToSite](state, payLoad: { siteId: number; siteLevel: SiteLevel }) {
        const siteIndex = state.All.findIndex(s => s.id === payLoad.siteId);
        const site = state.All[siteIndex];

        if (payLoad.siteLevel !== undefined) {
            if (payLoad.siteLevel.isNeverExpired) {
                payLoad.siteLevel.expiredDate = null;
            } else {
                payLoad.siteLevel.expiredDate = DateTimeUtil.ParseAndFormat(payLoad.siteLevel.expiredDate, 'DD MMM YYYY');
            }
        }

        site.siteLevel = payLoad.siteLevel;
        state.All.splice(siteIndex, 1, site);
    },

    [Type.Mutation.SetLoginUserSiteLevels](state, siteLevels: Array<SiteLevel>) {
        state.LoginUserSiteLevels = _.cloneDeep(siteLevels);
    },

    [Type.Mutation.SetViewedUserSiteLevels](state, siteLevels: Array<SiteLevel>) {
        state.ViewedUserSiteLevels = _.cloneDeep(siteLevels);
    },

    [Type.Mutation.SetAllFunctionsUserPolicyForSite](state, payLoad: { siteId: number; rootPolicy: boolean; isRead: boolean }) {
        const siteIndex = state.All.findIndex(s => s.id === payLoad.siteId);
        const site = state.All[siteIndex];

        if (payLoad.isRead && !site.disableCheckBoxAllFunctionReadPolicy) {
            FunctionUtil.SetAllFunctionsReadPolicy(site, payLoad.rootPolicy);

            if (payLoad.rootPolicy === false) {
                FunctionUtil.SetAllFunctionsHostPolicy(site, payLoad.rootPolicy);
            }
        } else if (!site.disableCheckBoxAllFunctionHostPolicy) {
            FunctionUtil.SetAllFunctionsHostPolicy(site, payLoad.rootPolicy);

            if (!site.hasAllFunctionUserReadPolicy) {
                FunctionUtil.SetAllFunctionsReadPolicy(site, payLoad.rootPolicy);
            }
        }

        site.hasFunctionsChange = true;
        state.All.splice(siteIndex, 1, site);
        state.SelectAllReadFunctionsForAllSites = state.All.filter(s => !s.disableCheckBoxAllFunctionReadPolicy).every(
            s => s.hasAllFunctionUserReadPolicy
        );
        state.SelectAllHostFunctionsForAllSites = state.All.filter(s => !s.disableCheckBoxAllFunctionHostPolicy).every(
            s => s.hasAllFunctionUserHostPolicy
        );
    },

    [Type.Mutation.SetFunctionUserPolicy](
        state,
        payLoad: { siteId: number; functionId: number; rootFuncUserPolicy: boolean; isRead: boolean }
    ) {
        const siteIndex = state.All.findIndex(s => s.id === payLoad.siteId);
        const site = state.All[siteIndex];

        if (
            payLoad.rootFuncUserPolicy &&
            configs.WinLossByProductsFunctionNames.some(name =>
                FunctionUtil.CheckExistFunctionPermissionInWinLossByProducts(site, payLoad.functionId, name)
            )
        ) {
            const func = site.functions.find(f => f.id === payLoad.functionId);
            func.hasUserReadPolicy = !func.hasUserReadPolicy;
            state.All.splice(siteIndex, 1, site);
            return;
        }

        FunctionUtil.SetCurrentFunctionPolicy(site, payLoad.functionId, payLoad.rootFuncUserPolicy, payLoad.isRead);
        FunctionUtil.SetChildrenFunctionPolicy(site, payLoad.functionId, payLoad.rootFuncUserPolicy, payLoad.isRead);
        FunctionUtil.SetParentFunctionPolicy(site, payLoad.functionId, payLoad.rootFuncUserPolicy, payLoad.isRead);
        FunctionUtil.SetSiteFunctionPolicy(site);

        site.hasFunctionsChange = true;
        state.All.splice(siteIndex, 1, site);

        state.SelectAllReadFunctionsForAllSites = state.All.filter(s => !s.disableCheckBoxAllFunctionReadPolicy).every(
            s => s.hasAllFunctionUserReadPolicy
        );
        state.SelectAllHostFunctionsForAllSites = state.All.filter(s => !s.disableCheckBoxAllFunctionHostPolicy).every(
            s => s.hasAllFunctionUserHostPolicy
        );
    },

    [Type.Mutation.ChangeFunctionPolicy](state, payLoad: { siteId: number; func: Function; isRead: boolean }) {
        const siteIndex = state.All.findIndex(s => s.id === payLoad.siteId);
        const site = state.All[siteIndex];

        const currentFunc = site.functions.find(f => f.id === payLoad.func.id);

        if (payLoad.isRead) {
            currentFunc.hasUserReadPolicy = !currentFunc.hasUserReadPolicy;
        } else {
            currentFunc.hasUserHostPolicy = !currentFunc.hasUserHostPolicy;
        }

        state.All.splice(siteIndex, 1, site);
    },

    [Type.Mutation.SetLoginUserAssignFunctionPermissionInAdminSite](state, permission: number) {
        state.LoginUserAssignFunctionPermissionInAdminSite = permission;
    },

    [Type.Mutation.SetLoginUserAssignSitePermissionForSite](state, payload: { siteId: number; permission: number }) {
        const site = state.All.find(s => s.id === payload.siteId);
        site.loginUserAssignSitePermission = payload.permission;
    },

    [Type.Mutation.ShowHideSiteFunctions](state, { siteId, isShow }) {
        const siteIndex = state.All.findIndex(s => s.id === siteId);
        const site = state.All[siteIndex];
        site.showFunctionList = isShow;

        state.All.splice(siteIndex, 1, site);
    },

    [Type.Mutation.EnableEditSite](state, siteId: number) {
        const siteIndex = state.All.findIndex(s => s.id === siteId);
        const site = state.All[siteIndex];
        site.enableEdit = !site.enableEdit;

        state.All.splice(siteIndex, 1, site);
    },

    [Type.Mutation.EnableAssignSite](state, siteId: number) {
        const siteIndex = state.All.findIndex(s => s.id === siteId);
        const site = state.All[siteIndex];
        site.enableAssign = true;
        site.enableEdit = true;
        site.siteLevel = {
            level: SiteLevelValue.User,
            isActive: true,
            isNeverExpired: true,
            siteId: site.id,
            siteName: site.name
        };

        state.All.splice(siteIndex, 1, site);
    },

    [Type.Mutation.DoneAssignSite](state, siteId: number) {
        const siteIndex = state.All.findIndex(s => s.id === siteId);
        const site = state.All[siteIndex];
        site.enableAssign = false;
        site.enableEdit = false;

        const siteLevel = _.cloneDeep(site.siteLevel);
        state.ViewedUserSiteLevels.push(siteLevel);

        state.All.splice(siteIndex, 1, site);
    },

    [Type.Mutation.CancelAssignSite](state, siteId: number) {
        const siteIndex = state.All.findIndex(s => s.id === siteId);
        const site = state.All[siteIndex];

        site.enableAssign = false;
        site.enableEdit = false;
        site.siteLevel = undefined;

        state.All.splice(siteIndex, 1, site);
    },

    [Type.Mutation.ChangeSiteStatus](state, payload: { siteId: number; rootStatus: boolean }) {
        const siteIndex = state.All.findIndex(s => s.id === payload.siteId);
        const site = state.All[siteIndex];
        site.siteLevel.isActive = payload.rootStatus;
        state.ViewedUserSiteLevels.find(sl => sl.siteId === payload.siteId).isActive = payload.rootStatus;

        state.All.splice(siteIndex, 1, site);
    },

    [Type.Mutation.UpdateSiteLevel](state, payload: { siteId: number; siteLevel: SiteLevel }) {
        const currentSiteLevel = state.ViewedUserSiteLevels.find(sl => sl.siteId === payload.siteId);
        _.assign(currentSiteLevel, payload.siteLevel);
    }
};
