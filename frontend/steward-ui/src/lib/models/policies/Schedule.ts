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