# Screens
My instinct is that Steward's UI has three major jobs:

1. Observe reality — what exists and what is happening.
2. Define intent — what should happen.
3. Resolve exceptions — what happens when the system needs human input.

With that framing, here are the screens I would brainstorm.

<hr>

## Priority
1. Home (w/ status widgets)
2. Control Groups  (w/ creation)
3. Policies / Rules (w/ creation)
4. Request / Approvals

<hr>

## 1. Dashboard / Home
**Purpose:** "What is the current state of my Steward system?"

This is the first screen you see.

Possible widgets:

- Agent health
    - 5/5 agents online
    - 1 offline
- Active policies
    - "3 active rules"
- Current enforcement
    - Alice's laptop: YouTube blocked
    - Bob's tablet: 30 minutes remaining
- Pending requests
    - "Alice requested 20 more minutes"
- Recent events
    - "Pi-hole blocked YouTube"
    - "Cold Turkey disconnected"

Questions this screen answers:

> Is everything working?




Great. I think the next step is to think less about implementation and more like we are designing the "app" a parent/admin would use.

My instinct is that Steward's UI has **three major jobs**:

1. **Observe reality** — what exists and what is happening.
2. **Define intent** — what should happen.
3. **Resolve exceptions** — what happens when the system needs human input.

With that framing, here are the screens I would brainstorm.

---

# 2. Agents

**Purpose:** "What enforcement components are connected?"

This is probably the first screen needed from our MQTT work.

List:

```
Pi-hole Home
Status: Online
Last Seen: 30 seconds ago

Capabilities:
- DNS blocking
- Internet filtering

Resources:
- YouTube
- TikTok
```

Actions:

* Refresh agents
* View agent details
* Disable/enable agent? (maybe later)

---

# 3. Agent Details

**Purpose:** "Tell me everything about this enforcer."

Example:

```
Pi-hole Home

Status:
Online

Instance:
container-abc123

Resources:
- YouTube
  - block
  - unblock

- Internet
  - block
  - unblock

Recent Commands:
- Block YouTube
  Completed
- Unblock YouTube
  Failed
```

This is useful for debugging.

---

# 4. Resources

**Purpose:** "What things can Steward control?"

This is a catalog.

Example:

```
Resources

Internet Access
Provided by:
Pi-hole Home

Actions:
Block
Unblock


YouTube
Provided by:
Pi-hole Home

Actions:
Block
Unblock
```

Question answered:

> What knobs do I actually have?

---

# 5. Users

**Purpose:** "Who is being managed?"

Example:

```
Alice

Devices:
- Alice Laptop
- iPad

Control Groups:
- Alice Internet
- Homework Time

Active Rules:
- No YouTube after 8 PM
```

---

# 6. Devices

**Purpose:** "What hardware exists?"

Example:

```
Alice Laptop

Assigned User:
Alice

Resources:
- Applications
- Websites

Agents:
- Cold Turkey
```

Potentially combined with Users at first.

---

# 7. Control Groups

**Purpose:** "What targets can I apply rules to?"

This is probably one of the core screens.

Example:

```
Homework Devices

Users:
Alice

Devices:
Laptop

Resources:
Chrome
Internet

Rules:
School hours
```

Actions:

* Create group
* Edit group
* Preview affected things

---

# 8. Policies / Rules

**Purpose:** "What restrictions exist?"

List:

```
Rules

Bedtime Internet
Target:
Alice Devices

Schedule:
9 PM - 7 AM

Status:
Active


Gaming Limit
Target:
Gaming PCs

Allowance:
2 hours/day
```

---

# 9. Rule Editor

**Purpose:** "Create or modify a rule."

Wizard style might work well:

Step 1:
What does this apply to?

→ Select Control Group

Step 2:
What access rule?

→ Schedule
→ Time allowance
→ Session limits

Step 3:
What friction?

→ Delay
→ Approval

Step 4:
Save

---

# 10. Requests / Approvals

**Purpose:** "Handle exceptions."

Example:

```
Alice requested:

20 additional minutes
for YouTube

Reason:
Homework finished

[Approve]
[Deny]
```

---

# 11. Activity / History

**Purpose:** "What happened?"

Timeline:

```
8:00 PM
Bedtime rule activated

8:01 PM
Pi-hole blocked YouTube

8:15 PM
Alice requested access

8:17 PM
Request approved
```

This will become extremely valuable once the system is used.

---

# 12. Settings

Probably later:

* MQTT connection
* Server configuration
* Authentication
* User accounts
* Backup/restore


