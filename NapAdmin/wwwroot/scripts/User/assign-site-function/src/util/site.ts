import { Site, SiteLevelValue, SiteLevel } from '../model/site';

export default class SiteUtil {
    public static CheckForEnablingStatus(
        site: Site,
        loginUserSiteLevels: Array<SiteLevel>,
        loginUserHasAssignSitePermission: boolean,
        loginUserIsPortalAdmin: boolean,
        loginUserHasAssignSiteHostPermissionSites: Array<Site>,
        isSelfEdit: boolean,
        currentSiteId: number): boolean {
        if (isSelfEdit) {
            return loginUserIsPortalAdmin && site.id !== currentSiteId;
        }

        const loginUserCurrentSiteLevel = loginUserSiteLevels.find(sl => sl.siteId === site.id);

        return this.IsPortalAdmin(site, loginUserIsPortalAdmin) ||
            (loginUserHasAssignSitePermission &&
                this.LoginUserHasAssignSiteHostPermissionInSite(site, loginUserHasAssignSiteHostPermissionSites) &&
                this.UserSiteIsActive(loginUserCurrentSiteLevel) &&
                this.LoginUserSiteLevelIsGreaterThanViewedUserSiteLevel(site, loginUserCurrentSiteLevel));
    }

    public static IsPortalAdmin(site: Site, loginUserIsPortalAdmin: boolean): boolean {
        return loginUserIsPortalAdmin &&
            (site.siteLevel === undefined || site.siteLevel.level < SiteLevelValue.PortalAdmin);
    }

    public static LoginUserHasAssignSiteHostPermissionInSite(site: Site, loginUserHasAssignSiteHostPermissionSites: Array<Site>): boolean {
        return loginUserHasAssignSiteHostPermissionSites.some(s => s.id === site.id);
    }

    public static UserSiteIsActive(siteLevel: SiteLevel): boolean {
        return siteLevel !== undefined && siteLevel.isActive;
    }

    public static LoginUserSiteLevelIsGreaterThanViewedUserSiteLevel(
        site: Site,
        loginUserCurrentSiteLevel: SiteLevel): boolean {
        return site.siteLevel === undefined || loginUserCurrentSiteLevel.level > site.siteLevel.level ||
            (loginUserCurrentSiteLevel.level === SiteLevelValue.User && site.siteLevel.level === SiteLevelValue.User);
    }

    public static ShowExpiredDateColumn(viewedUserName: string) {
        return viewedUserName.toLowerCase().startsWith('starix');
    }
}
