import { Site } from './site';

export interface Function {
    id?: number;
    name?: string;
    key?: string;
    parentId?: number;
    parentName?: string;
    siteId?: number;
    siteName?: string;
    hasUserReadPolicy?: boolean;
    hasUserHostPolicy?: boolean;
    hasGroupReadPolicy?: boolean;
    hasGroupHostPolicy?: boolean;
    crud?: number;
    disableRead?: boolean;
    disableHost?: boolean;
    isHidden?: boolean;
}

export interface FunctionPolicy {
    id?: number;
    name?: string;
    key?: string;
    parentId?: number;
    parentName?: string;
    siteId?: number;
    siteName?: string;
    policy?: number;
}

export enum FunctionPolicyValue {
    None = 0,
    Read = 1,
    Host = 2
}
