import { client } from "./client";
import { setSession, sessionRevision, beginSessionChange, type SessionUser } from "../session";
export const getLoginUsers = () => client.get<{ id: number; name: string }[]>("/auth/users");
export async function signIn(userId: number, pin: string) {
    const revision = beginSessionChange();
    const user = await client.post<SessionUser>("/auth/login", { userId, pin });
    if (revision === sessionRevision()) setSession(user);
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
