export interface Schedule {
    enabled: boolean;

    days: DayOfWeek[];

    startTime: string;

    endTime: string;
}

export enum DayOfWeek {
    Sunday = 0,
    Monday = 1,
    Tuesday = 2,
    Wednesday = 3,
    Thursday = 4,
    Friday = 5,
    Saturday = 6
}