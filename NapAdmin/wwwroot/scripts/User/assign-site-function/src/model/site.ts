import { Function, FunctionPolicy } from './function';

export interface Site {
    description?: string;
    environment?: string;
    isActive?: boolean;
    numOfUser?: number;
    secretAccessKey?: string;
    siteKey?: string;
    id?: number;
    name?: string;
    showFunctionList?: boolean;
    hasAllFunctionUserReadPolicy?: boolean;
    hasAllFunctionUserHostPolicy?: boolean;
    hasAllFunctionGroupReadPolicy?: boolean;
    hasAllFunctionGroupHostPolicy?: boolean;
    disableCheckBoxAllFunctionReadPolicy?: boolean;
    disableCheckBoxAllFunctionHostPolicy?: boolean;
    hideCheckBoxAllFunctionPolicy?: boolean;
    functions?: Array<Function>;
    viewedUserFunctionPolicies?: Array<FunctionPolicy>;
    viewedUserGroupFunctionPolicies?: Array<FunctionPolicy>;
    loginUserFunctionPolicies?: Array<FunctionPolicy>;
    loginUserAssignFunctionPermission?: number;
    loginUserAssignSitePermission?: number;
    siteLevel?: SiteLevel;
    enableEdit?: boolean;
    enableAssign?: boolean;
    hasFunctionsChange?: boolean;
}

export interface SiteLevel {
    level?: number;
    isActive?: boolean;
    isNeverExpired?: boolean;
    expiredDate?: string;
    siteId?: number;
    siteName?: string;
}

export enum SiteLevelValue {
    User = 0,
    SiteAdmin = 50,
    PortalAdmin = 100
}
