# Steward UI User Flows

The Steward UI is the human control surface for configuring, observing, and managing the Steward system.

The UI should focus on helping a user answer:

- What is connected?
- What can be controlled?
- What is currently being enforced?
- What rules are active?
- How do I change behavior?

---

# Core Flows

## 1. View Agent Status

Goal:
Understand whether Steward's enforcement components are connected and healthy.

User should be able to:

- View all registered agents
- See online/offline status
- See last connection time
- See what capabilities each agent provides
- Identify unhealthy or missing agents

Examples:

- Pi-hole agent: Online
- Cold Turkey agent: Offline
- ESP32 bedroom display: Online

---

## 2. Refresh Agents

Goal:
Force Steward to rediscover available agents and capabilities.

User should be able to:

- Trigger an agent refresh
- See refresh progress
- See newly discovered agents
- See updated resources

Use cases:

- Installing a new enforcer
- Recovering after a server restart
- Updating agent capabilities

---

## 3. View Available Resources

Goal:
Understand what things can currently be controlled.

User should be able to:

- Browse resources discovered from agents
- See which agent provides each resource
- See available actions

Examples:

- YouTube
  - Provided by Pi-hole
  - Actions: Block, Unblock

- Gaming PC
  - Provided by Cold Turkey
  - Actions: Block, Unblock

---

## 4. Manage Control Groups

Goal:
Create reusable targets for screen time rules.

A control group combines:

- Users
- Devices
- Resources

User should be able to:

- Create a control group
- Add/remove members
- Preview what the group affects

Examples:

"Child Internet"

Includes:
- User: Alice
- Resources: Internet access
- Devices: All devices

"Gaming"

Includes:
- User: Bob
- Resource: Gaming PC

---

## 5. Configure Screen Time Rules

Goal:
Define when and how control groups are enforced.

User should be able to:

- Create rules
- Apply rules to control groups
- Enable/disable rules
- View active rules

Supported controls:

Access:
- Scheduled block windows
- Daily time allowance
- Maximum session length
- Number of unlocks per day

Friction:
- Unlock delays
- Increasing delays
- Required confirmation challenges

Overrides:
- Access requests
- Temporary bypasses
- Allowance extensions

---

## 6. Understand Current Enforcement

Goal:
Know whether configured rules are actually being applied.

User should be able to:

- View active policies
- See which agents are enforcing them
- See enforcement status
- Identify failures

Example:

Rule:
"Block YouTube after 8 PM"

Status:
- Policy active
- Pi-hole enforcing
- Cold Turkey not applicable

---

# Initial UI Priority

The first version should support:

1. Agent status
2. Resource discovery
3. Control group management
4. Basic rule creation
5. Enforcement visibility

Advanced features like approvals and adaptive friction can come later.