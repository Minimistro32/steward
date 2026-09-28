import { client } from "./client";
import { setSession, sessionRevision, beginSessionChange, type SessionUser } from "../session";
const LAST_USER_KEY = "steward.lastSignedInUserId";
export function getLastSignedInUserId(): string | null {
    try { return localStorage.getItem(LAST_USER_KEY); }
    catch { return null; } // Storage may be disabled by the browser.
}
export const getLoginUsers = () => client.get<{ id: number; name: string }[]>("/auth/users");
export async function signIn(userId: number, pin: string) {
    const revision = beginSessionChange();
    const user = await client.post<SessionUser>("/auth/login", { userId, pin });
    if (revision === sessionRevision()) {
        try { localStorage.setItem(LAST_USER_KEY, String(user.id)); }
        catch { /* Remembering the selection is optional; sign-in still succeeds. */ }
        setSession(user);
    }
}
export async function signOut() {
    const revision = beginSessionChange();
    await client.post<void>("/auth/logout");
    if (revision === sessionRevision()) setSession(null);
}
export async function restoreSession() {
    const revision = sessionRevision();
    const user = await client.get<SessionUser>("/auth/session");
    if (revision === sessionRevision()) setSession(user);
}
