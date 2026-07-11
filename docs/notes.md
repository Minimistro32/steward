ScreenTime Server Notes
- PolicyEngine (WebAPI / DB / Server)
- Web UI Observer (Client)
- ESP32 Enforcer (Client)
- Cold Turkey Blocker Enforcer (Client)
- PI Hole DNS Blocking Enforcer (Client)

Concepts:
- Users (A person whose screen time is managed by the system)
- Devices (A physical or logical thing that a user interacts with)
- Resource (tied to and advertised by an enforcer, represents something that can have its screen time managed)
- Control Group (a boolean combination of users, devices, and resources that can be controlled in a policy and relayed to enforcers)
- Rule (a screen time enforcement rule applied to a Control Group)
 
<hr>

Here are the access control levers you can pull, all of which I believe can work simultaneously (if desired) without conflicts. Scheduled block windows, daily time allowance, number of unlocks a day, maximum session length.

Here are all the friction control levers you can pull. Unlock delays, gradual wait escalation, typing out a phrase.

Here are all the override levers you can pull. Unblock requests (manual approval by parent/account owner), temporary bypasses (grants access for X minutes), allowance extensions (request 10 more minutes or 1 more unlock).