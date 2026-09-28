# Authentication and Users: MVP Design

Status: account storage, terminal setup, sign-in, sessions, and role-based page/API
access and admin-managed account creation/editing implemented. Email recovery
remains unimplemented.

## Goals

Differentiate users and give them the appropriate interface with a simple,
mobile-friendly sign-in flow that children ages 6–8 and older can use.
Keep the MVP small: names, numeric PINs, and two account types.
Transparency is intentional: every signed-in user can see everyone's request
activity.

Steward is intended for local networks, not public internet exposure. Agents
are decentralized and enforce policy independently.

## Account Types

The account types are **Admin** and **Member**.

- **Admin:** Manages people, settings, and request approvals.
- **Member:** Requests access and sees everyone's request activity.

| Capability | Admin | Member |
| --- | --- | --- |
| Available pages | All pages | Requests only |
| View request activity | All users' activity | All users' activity |
| Approve or reject requests | Yes, subject to request rules | Never |
| Manage users and settings | Yes | No |
| Reset a member account's PIN | Yes | No |
| Recover own forgotten PIN | Email reset flow | Ask an admin |

Names identify accounts in the UI. Avatars are out of scope.
Admin accounts require both an email address and a PIN. Member accounts are
managed by admins and may have neither an email address nor a PIN.

## First Installation

There is no automatically seeded admin or default credential. The
first-installation flow uses `dotnet run --project src/Steward.Server -- setup`:

1. Create the database and apply migrations.
2. Prompt for the first admin’s name, email, and numeric PIN. Hide PIN entry and
   ask for confirmation.
3. Validate the input, hash the PIN, and save the admin account.
4. Start Steward and sign in through the frontend using that account.

For Docker, setup should run interactively against the same database volume as
the application. Normal startup should remain noninteractive; before setup is
complete, it should print setup instructions and exit. Startup applies pending migrations before checking for an admin. Setup does not
start the HTTP server or MQTT services. Redirected input is refused to keep PIN
entry interactive.

Setup never overwrites existing accounts. A persistent setup-completed marker
is saved in the same transaction as the admin; removing admins does not reopen
setup. If no admin remains afterward, startup requires restoring an admin from
a database backup. After installation, admins manage accounts in the frontend.

## PIN Storage and Database Rules

An admin must have a nonblank email and PIN hash. Members may leave their PIN
unset, represented by a null `PinHash`, not a hash of an empty string. For a
member with an unset PIN, only an empty PIN submission should succeed. For any
account with a configured PIN, the correct PIN is required; an empty submission
must fail. Admins can manage member PINs, including clearing them. An admin’s
PIN cannot be cleared while the account remains an admin.

The database checks admin email and hash presence, as well as valid account
types. Email format, numeric PIN validation, and creation of a valid hash belong
in terminal setup and future account-management code. A member can be promoted only
when both required admin fields are present.

Terminal setup uses ASP.NET Core’s `PasswordHasher<UserEntity>`; future PIN
editing should use the same hasher.
It generates a fresh random salt for each PIN update and stores the salt, hash,
and hashing parameters together in `PinHash`; no separate salt column is needed.
A configured PIN must be verified by the hasher, including its rehash-needed result.

The initial migration creates the current schema without seeded users. Terminal
setup creates the first admin explicitly.

## Sign-In and Sessions

1. The user selects their name and enters their numeric PIN (or leaves it empty
   for a member whose PIN is unset).
2. Steward starts a signed-in session.
3. The user can perform permitted actions without entering their PIN for each
   action. Member accounts land on the Requests page.
4. The user can sign out explicitly. Sessions also expire, after which the user
   must sign in again before continuing, using an empty PIN if theirs is unset.

This is a temporary signed-in session, not an indefinitely remembered login or
a permanently paired browser. The intended interaction is similar to the
user's description of Pi-hole sign-in, with a PIN and account selection.

Use a mobile-friendly numeric input/keypad. Show the current user's name and an
accessible sign-out action so shared-device users can switch accounts.

Sessions expire eight hours after sign-in without sliding renewal. Cookies are
HTTP-only, SameSite Strict, and secure when served over HTTPS. Tickets live in
server memory: logout revokes them, and restarting Steward signs everyone out.
Changes to account type or PIN, and account deletion, invalidate existing sessions.
The frontend checks the session at launch and every minute, and clears it on API
401 responses. Members always land on Requests, with no sidebar and the logo next
to “Steward / Giving you room to grow.” Admins retain the full navigation.

The public account picker exposes only IDs and names. All other data APIs require
a session; management APIs require Admin. Members can request and complete access
only for themselves and cannot approve or reject requests. The Requests page uses the signed-in user for both account types, without a user
selector. Approval identity always comes from the session. All signed-in users see the shared activity timeline.

Login allows ten attempts per minute per client IP. Cookie-authenticated mutations
require `X-Steward-Request: 1`, with CORS restricted to configured development
origins. The frontend uses `/api`; Vite proxies it to the local backend during
development. Production hosting must route `/api` to Steward on the same origin.
Terminal setup currently requires a six-digit PIN.

## Requests and Activity

Member accounts use their signed-in identity to request access. They cannot
choose another user to act as through the existing Requests-page user selector.

All signed-in accounts see the shared request activity timeline, including
other users' requests and outcomes. Do not filter member accounts' activity
view to only their own requests.

Member accounts never see approve or reject buttons, including on other users'
pending requests. Admin accounts can respond where the request rules permit it.
Admin status does not by itself remove the existing restriction against
approving one's own request.

## User Management

Admins create users at `#/users/new` and edit them at `#/users/:id`, using the
same form. It includes name, type, email, and a six-digit PIN with confirmation.
On edit, an empty PIN preserves the existing hash. Members have an explicit
clear-PIN option; admins must retain a PIN and email. Device assignments are
managed on the Users page and are preserved when account details are saved.
The last admin cannot be demoted or deleted. Changing your own PIN or type
returns you to sign-in. PINs and hashes are never returned by the user API.

## PIN Recovery

- An admin resets a member account's PIN when needed.
- An admin who forgets their own PIN uses an email reset flow.
- Member accounts do not have an email recovery flow.

The email delivery setup and reset-link behavior still need implementation
design. Browser pairing and passkeys are not prerequisites for this MVP.

## Implementation Boundaries

The server must enforce the account permissions as well as the UI. Member
accounts must not be able to approve/reject requests or modify administrative
settings by calling an API directly.

Use the signed-in session to determine the acting user instead of trusting an
acting-user ID supplied by the browser. Replace the activity widget's temporary
hardcoded approver identity with the signed-in admin identity. Request data can
still identify the requester separately from the person taking an action.

Keep authentication implementation details separate from the product flow;
use standard session and credential handling when implementing it.

## Out of Scope for MVP

- Passkeys.
- Avatars.
- QR codes or device-pairing flows.
- Permanently remembered child sessions.
- Additional account roles or per-person approver assignments.

## Remaining Decisions

- Email delivery configuration.
