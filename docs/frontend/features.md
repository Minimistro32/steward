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