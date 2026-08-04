export interface Schedule {
    days: DayOfWeek[];
    startTime: string;   // "" means beginning of day
    endTime: string;     // "" means end of day
}

export namespace Schedule {
    export function formatTimeRange(schedule: Schedule): string {
        const hasStart = schedule.startTime !== "";
        const hasEnd = schedule.endTime !== "";

        return hasStart || hasEnd
            ? `${formatTime(schedule.startTime || "00:00")} \u2013 ${formatTime(schedule.endTime || "24:00")}`
            : "";

    }

    function formatTime(time: string): string {
        if (time === "24:00") return "Midnight";

        let [hour, minute] = time.split(":").map(Number);

        const suffix = hour >= 12 ? "PM" : "AM";

        hour %= 12;

        if (hour === 0) hour = 12;

        return minute === 0
            ? `${hour}${suffix}`
            : `${hour}:${minute.toString().padStart(2, "0")}${suffix}`;
    }
}

export enum DayOfWeek {
    Sunday = "Sunday",
    Monday = "Monday",
    Tuesday = "Tuesday",
    Wednesday = "Wednesday",
    Thursday = "Thursday",
    Friday = "Friday",
    Saturday = "Saturday",
}

export namespace DayOfWeek {
    const numbers: Record<DayOfWeek, number> = {
        [DayOfWeek.Sunday]: 0,
        [DayOfWeek.Monday]: 1,
        [DayOfWeek.Tuesday]: 2,
        [DayOfWeek.Wednesday]: 3,
        [DayOfWeek.Thursday]: 4,
        [DayOfWeek.Friday]: 5,
        [DayOfWeek.Saturday]: 6,
    };

    export function toNumber(day: DayOfWeek): number {
        return numbers[day];
    }

    export const all: DayOfWeek[] = [
        DayOfWeek.Sunday,
        DayOfWeek.Monday,
        DayOfWeek.Tuesday,
        DayOfWeek.Wednesday,
        DayOfWeek.Thursday,
        DayOfWeek.Friday,
        DayOfWeek.Saturday,
    ] as const;
}