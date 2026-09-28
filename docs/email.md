# Admin PIN recovery email

Steward sends mail through SMTP. It has no paid email dependency. You supply an
existing SMTP mailbox/relay; the application does not create or pay for one.

## Configure SMTP

Set these environment variables for the **server process**, then restart it:

```sh
export Smtp__Enabled=true
export Smtp__Host=smtp.gmail.com
export Smtp__Port=587
export Smtp__UseStartTls=true
export Smtp__Username=your-mailbox@gmail.com
export Smtp__FromAddress=your-mailbox@gmail.com
export Smtp__FromName=Steward
read -rsp 'SMTP app password: ' Smtp__Password; echo
export Smtp__Password
dotnet run --project src/Steward.Server
unset Smtp__Password
```

Use a dedicated mailbox/app password where possible. Keep passwords out of Git,
command-line arguments, and frontend configuration. For a service/container,
provide the same settings through its private environment/secret configuration.
Do not disable TLS certificate validation. This transport uses STARTTLS (usually
port 587), not implicit TLS on port 465 or OAuth-only SMTP accounts.

Gmail can send through `smtp.gmail.com:587` using an app password when the account
supports it and two-step verification is enabled. Account restrictions may prevent
app passwords. See [Google’s SMTP settings](https://support.google.com/mail/answer/7104828)
and [app password instructions](https://support.google.com/accounts/answer/185833).

Alternatively, Brevo currently offers a free plan with 300 daily sends. Complete
its sender verification and use the SMTP credentials from its dashboard. Limits
and account approval are controlled by the provider. See
[Brevo’s plans](https://help.brevo.com/hc/en-us/articles/208589409-About-Brevo-s-pricing-plans).

A self-hosted SMTP relay also works. For a local development mail catcher only,
set `Smtp__UseStartTls=false`, omit username/password, and use its local host/port.
A mail catcher captures mail for testing; it does not deliver to real inboxes.

## User flow

1. On Login, select the admin account and expand **Forgot your PIN?**
2. Enter that account’s email and choose **Email me a new PIN**.
3. Enter the emailed eight-digit PIN within 30 minutes.
4. That PIN becomes the account’s normal PIN after successful sign-in. Changing
   it in Edit User is optional; it does not expire after activation.

Members cannot use email recovery. Admins reset member PINs in Edit User.
Responses do not disclose whether the submitted name/email matched. Sending is
limited to once per account every five minutes and five requests per client IP
per 15 minutes. A newer email replaces the previous pending recovery PIN.

Pending recovery PINs are stored only as salted hashes. The existing PIN and
sessions remain valid until the emailed PIN is used. Activation replaces the old
PIN, clears the pending recovery hash, and invalidates old sessions. Manual PIN,
email, or account-type changes cancel pending recovery PINs.

SMTP failure rolls back the reset; it does not replace the existing PIN. SMTP
acceptance does not guarantee inbox delivery. If mail was accepted but the final
database commit failed, that email’s PIN will not work; request another later.
No PINs or SMTP credentials are returned through the API or intentionally logged.

## Verification

After building the server, run `python3 tests/recovery_smoke.py`. It uses a local
SMTP test peer and temporary database; no messages leave the machine.
