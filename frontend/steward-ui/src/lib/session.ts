import { writable } from "svelte/store";
export type SessionUser = { id: number; name: string; type: "admin" | "member" };
export const currentUser = writable<SessionUser | null>(null);

let revision = 0;
export const sessionRevision = () => revision;
export const beginSessionChange = () => ++revision;
export function setSession(user: SessionUser | null) {
    revision++;
    currentUser.set(user);
}
