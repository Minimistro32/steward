export interface Schedule {
    days: DayOfWeek[];
    startTime: string;   // "" means beginning of day
    endTime: string;     // "" means end of day
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