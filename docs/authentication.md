# Authentication and Users: MVP Design

Status: agreed product direction; not yet implemented.

## Goals

Differentiate users and give them the appropriate interface with a simple,
mobile-friendly sign-in flow that children ages 6–8 and older can use.
Keep the MVP small: names, numeric PINs, and two account types.
Transparency is intentional: every signed-in user can see everyone's request
activity.

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
Admin accounts require an email address for PIN recovery; member accounts do
not need an email address.

## Sign-In and Sessions

1. The user selects their name and enters their numeric PIN.
2. Steward starts a signed-in session.
3. The user can perform permitted actions without entering their PIN for each
   action. Member accounts land on the Requests page.
4. The user can sign out explicitly. Sessions also expire, after which the user
   must enter their PIN again before continuing.

This is a temporary signed-in session, not an indefinitely remembered login or
a permanently paired browser. The intended interaction is similar to the
user's description of Pi-hole sign-in, with a PIN and account selection.

Use a mobile-friendly numeric input/keypad. Show the current user's name and an
accessible sign-out action so shared-device users can switch accounts.

The exact session duration and whether expiry is based on inactivity, elapsed
time since sign-in, or both remain to be decided. Six-digit PINs are the proposed
starting point; finalize PIN length before implementation.

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

- Session timeout duration and expiry behavior.
- Final PIN length.
- Initial admin creation and email configuration.
- Whether admins can submit requests on behalf of other users.
