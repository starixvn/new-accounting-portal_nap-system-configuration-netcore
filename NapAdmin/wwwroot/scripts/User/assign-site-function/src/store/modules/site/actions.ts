import Vuex from 'vuex';
import { Function, FunctionPolicyValue } from '../../../model/function';
import { Site } from '../../../model/site';
import { SiteState } from './state';
import * as Type from '../../types';
import SiteService from '../../../service/site-service';
import FunctionService from '../../../service/function-service';
import MessageBox from '../../../util/message-box';
import DateTimeUtil from '../../../util/datetime';
import * as _ from 'lodash';

export const actions: Vuex.ActionTree<SiteState, SiteState> = {
    [Type.Action.GetAllUserSites]({ commit, state }) {
        SiteService.GetSites(state.RootRoute, state.ViewedUserId, sites => {
            commit(Type.Mutation.SetAllUserSites, sites);

            SiteService.GetUserSitesLevels(state.RootRoute, state.ViewedUserId, siteLevels => {
                for (const site of sites) {
                    const siteId = site.id;
                    const siteLevel = siteLevels.find(sl => sl.siteId === siteId);

                    commit(Type.Mutation.SetSiteLevelToSite, { siteId, siteLevel });
                    commit(Type.Mutation.SetViewedUserSiteLevels, siteLevels);
                    GetFunctionsForSite(commit, state, siteId);
                }
            });
        });
    },

    [Type.Action.GetLoginUserHasAssignSiteHostPermissionSites]({ commit, state }) {
        SiteService.GetLoginUserHasAssignSiteHostPermissionSites(state.RootRoute, sites => {
            commit(Type.Mutation.SetLoginUserHasAssignSiteHostPermissionSites, sites);
        });
    },

    [Type.Action.GetLoginUserSiteLevels]({ commit, state }) {
        SiteService.GetUserSitesLevels(state.RootRoute, state.LoginUserId, siteLevels => {
            commit(Type.Mutation.SetLoginUserSiteLevels, siteLevels);
        });
    },

    [Type.Action.ResetSiteLevel]({ commit, state }, siteId: number) {
        ResetSiteLevel(commit, state, siteId);
    },

    [Type.Action.UpdateSiteStatus]({ commit, state }, { siteId, siteStatus }) {
        const site = state.All.find(s => s.id === siteId);

        _.assign(site.siteLevel, state.ViewedUserSiteLevels.find(sl => sl.siteId === siteId));
        site.siteLevel.isActive = siteStatus;

        if (!DateTimeUtil.IsValid(site.siteLevel.expiredDate)) {
            site.siteLevel.expiredDate = new Date().toDateString();
            site.siteLevel.isNeverExpired = true;
        }

        SiteService.UpdateSiteToUser(state.RootRoute, state.ViewedUserId, site, (result) => {
            if (result === 'OK') {
                commit(Type.Mutation.ChangeSiteStatus, { siteId: siteId, rootStatus: site.siteLevel.isActive });
                MessageBox.ShowUpdateSuccessfully();
            } else {
                MessageBox.ShowError();
            }
        });
    },

    [Type.Action.UpdateSite]({ commit, state }, { site, isEdit }) {
        UpdateSite(commit, state, site, isEdit);
    },

    [Type.Action.UpdateFunctionsForSite]({ commit, state }, site: Site) {
        if (site.hasFunctionsChange) {
            const funcHost: Array<number> = [];
            const funcRead: Array<number> = [];

            for (const func of site.functions) {
                if (func.hasUserReadPolicy && (!func.hasUserHostPolicy || (func.hasUserHostPolicy && func.crud <= 0))) {
                    funcRead.push(func.id);
                }

                if (func.hasUserHostPolicy && func.crud > 0) {
                    funcHost.push(func.id);
                }
            }

            const data = [{
                funcHost: funcHost,
                funcRead: funcRead,
                siteId: site.id,
                userId: state.ViewedUserId
            }];

            FunctionService.UpdateFunctions(state.RootRoute, data, (result) => {
                if (result === 'OK') {
                    MessageBox.ShowUpdateSuccessfully();
                    GetFunctionsForSite(commit, state, site.id);
                } else {
                    MessageBox.ShowError();
                }
            });
        } else {
            MessageBox.ShowUpdateSuccessfully('There is no changes to be saved');
        }
    },

    [Type.Action.AssignSite]({ commit, state }, site) {
        SiteService.AssignSiteToUser(state.RootRoute, state.ViewedUserId, site, (assignResult) => {
            if (assignResult === 'OK') {
                UpdateSite(commit, state, site, false);
            } else {
                commit(Type.Mutation.CancelAssignSite, this.Site.Id);
                MessageBox.ShowError('Assign error');
            }
        });
    },

    [Type.Action.SetAllFunctionsUserPolicyForAllSites]({ commit, state }, payLoad: { rootPolicy: boolean, isRead: boolean }) {
        for (const site of state.All) {
            commit(Type.Mutation.SetAllFunctionsUserPolicyForSite,
                { siteId: site.id, rootPolicy: payLoad.rootPolicy, isRead: payLoad.isRead });
        }
    }
};

const GetFunctionsForSite = (commit: Vuex.Commit, state: SiteState, siteId: number) => {
    FunctionService.GetFunctions(
        state.RootRoute,
        state.ViewedUserId,
        siteId,
        ({
            functions,
            functionPolicies,
            groupFunctionPolicies,
            loginUserFunctionPolicies,
            loginUserAssignFunctionPermission,
            loginUserAssignSitePermission
        }) => {
            commit(Type.Mutation.SetLoginUserFunctionPolicies, { siteId, functionPolicies: loginUserFunctionPolicies });
            commit(Type.Mutation.SetLoginUserAssignSitePermissionForSite, { siteId, permission: loginUserAssignSitePermission });
            commit(Type.Mutation.SetViewedUserFunctionPolicies, { siteId, functionPolicies, groupFunctionPolicies });
            commit(Type.Mutation.SetFunctionsToSite, { siteId, functions, loginUserAssignFunctionPermission });
            commit(Type.Mutation.SetSelectAllUserFunctionsForAllSites);
        });
};

const ResetSiteLevel = (commit: Vuex.Commit, state: SiteState, siteId: number) => {
    const siteLevel = _.cloneDeep(state.ViewedUserSiteLevels.find(sl => sl.siteId === siteId));
    commit(Type.Mutation.SetSiteLevelToSite, { siteId, siteLevel });
    commit(Type.Mutation.EnableEditSite, siteId);
};

const UpdateSite = (commit: Vuex.Commit, state: SiteState, site: Site, isEdit: boolean) => {
    if (!DateTimeUtil.IsValid(site.siteLevel.expiredDate)) {
        site.siteLevel.expiredDate = new Date().toDateString();
        site.siteLevel.isNeverExpired = true;
    }

    SiteService.UpdateSiteToUser(state.RootRoute, state.ViewedUserId, site, (result) => {
        if (result === 'OK') {
            MessageBox.ShowUpdateSuccessfully();

            GetFunctionsForSite(commit, state, site.id);

            if (isEdit) {
                commit(Type.Mutation.EnableEditSite, site.id);
                commit(Type.Mutation.UpdateSiteLevel, { siteId: site.id, siteLevel: site.siteLevel });
            } else {
                commit(Type.Mutation.DoneAssignSite, site.id);
            }
        } else {
            MessageBox.ShowError();
            ResetSiteLevel(commit, state, site.id);
        }
    });
};
