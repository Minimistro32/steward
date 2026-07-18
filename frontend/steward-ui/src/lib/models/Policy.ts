export interface Policy {
    id: string;

    name: string;
    tags: string[];

    wardId: string;

    isActive: boolean;

    schedule: Schedule;

    accessRules: AccessRules;

    overrideRequests: OverrideRequests;

    overrideAllowance: OverrideAllowance;

    createdAt: string;
    modifiedAt: string;
}


export interface Schedule {
    enabled: boolean;

    startTime: string;
    endTime: string;

    days: string[];
}


export interface AccessRules {

    dailyTimeAllowanceMinutes?: number;

    maximumSessionLengthMinutes?: number;

    dailyUnlocks?: number;
}


export interface OverrideRequests {

    allowed: boolean;

    requirement?: OverrideRequirement;
}


export enum OverrideRequirement {
    Delay = "delay",
    RandomText = "random-text",
    UserApproval = "user-approval"
}


export interface OverrideAllowance {

    additionalTimeMinutes?: number;

    maximumRequestableSessionLengthMinutes?: number;

    additionalUnlocks?: number;
}