# Steward Frontend Functions
1. View Agent Status
    - View all registered agents
    - See online/offline status
    - See last connection time
    - enable / disable agent
    - delete agent
2. Refresh Agents
    - populating new agents and resources
    - Show time since last refresh
3. CRUD Wards
    - See what resources agents control
    - See what devices agents control
    - See what users agents control
    - Group the above into something that can be controlled
    - optionally copy
    - optionally enable a toggle so all users share policy limits (ie shared console)
4. CRUD Policies
    - enable/disable
    - optionally copy
    - sudo override
5. Request and Approve Overrides
    - optionally push notification
    - optionally logins

## Definition of a Ward
### Purpose

A Ward is a reusable management surface that groups users, devices, and resources together so that policies can be applied consistently.

A Ward does not define behavior itself. Instead, it defines the scope over which one or more policies are evaluated.

Wards exist to answer the question:

> **"Who and what should these policies apply to?"**

### Metadata
* Name
* Description
* Tags
* Created Date
* Modified Date

### Members

A Ward may contain any combination of the following:

#### Users

Users represent the people whose allowances and policy state are tracked by Steward.

Daily allowances, override requests, and other per-user policy state are maintained independently for each user, regardless of how many devices they use.

Users are owned and managed by Steward.

#### Devices

Devices represent the endpoints on which agents can enforce policy.

A device may be associated with one or more users and may appear in multiple wards.

Devices are discovered and advertised by agents but organized into wards by Steward.

#### Resources

Resources identify the applications, services, or capabilities that policies govern.

Examples include:

* Steam
* Discord
* YouTube
* Internet Access
* Game Console Power

Resources are advertised by agents and selected by administrators when defining a ward.

### Policy Assignment

One or more policies may be assigned to a Ward.

Each assigned policy is evaluated against every applicable user within the ward.

The resulting enforcement actions are translated into device- and resource-specific commands for the appropriate agents.

### Ownership

| Object              | Source of Truth |
| ------------------- | --------------- |
| Wards               | Steward         |
| Users               | Steward         |
| Device Membership   | Steward         |
| Policies            | Steward         |
| Devices             | Agent Discovery |
| Resources           | Agent Discovery |


## Definition of a Policy
### Metadata
- Control Group
- Name
- Tags
- Is Active
- Created Date
- Modified Date

### Active Schedule

### Access Rules
- Total Daily Time Allowance
- Maximum Session Length
- Daily Unlocks

### Override Requests
Defines normal user-requested exceptions.
- Allowed: Y/N
- Requirements
    - Delay (escalating?)
    - Typing random text
    - User Approval

### Override Allowance
Defines the maximum exception a user may request.
- Additional Total Time
- Maximum Requestable Session Length (defaults to the same value from the access rules, but can be overridden)
- Additional Unlocks

### Open Questions
- What happens if two policies are active on the same control group at the same time?

## Administrative Overrides
Manual exceptions granted outside normal policy enforcement.