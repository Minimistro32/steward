# Steward Architecture Notes

## Overview

Steward is a modular screen time management system designed for a homelab environment.

The goal is to have a central server that manages screen time policies while supporting many different enforcement mechanisms without the server needing to know implementation details.

Examples of planned enforcers:

* Cold Turkey Blocker integration
* Pi-hole DNS blocking
* ESP32-controlled physical devices (such as a PS5 relay)
* Future clients such as mobile apps, CLI tools, or additional automation systems

The core idea:

> The server decides the desired state. Clients know how to enforce that state.

---

# High-Level Architecture

```
                 Web UI
                   |
                   |
              Steward Server
                   |
        ------------------------
        |          |           |
        |          |           |
   Cold Turkey   Pi-hole    ESP32
    Agent        Agent      Agent
```

The server is the source of truth.

Agents are responsible for translating high-level commands into device-specific actions.

---

# Major Components

## Steward Server

Responsibilities:

* Store system state
* Evaluate screen time policies
* Manage users and devices
* Track agents
* Receive capability advertisements
* Send desired states to agents
* Handle override requests
* Provide API access
* Host the web UI in production

The server should not directly know how a specific integration works.

Example:

The server should understand:

```
Block YouTube
```

It should not understand:

```
Create Pi-hole regex entry
```

or:

```
Call Cold Turkey executable with these arguments
```

Those belong to agents.

---

## Agents / Enforcers

Agents are independent clients that connect to Steward.

Responsibilities:

* Advertise capabilities
* Maintain connection to server
* Receive desired state
* Enforce changes locally
* Report status

Examples:

### Pi-hole Agent

Can advertise:

```
Resource:
    YouTube

Methods:
    DNS blocking
```

The agent understands how to modify Pi-hole configuration.

---

### Cold Turkey Agent

Can advertise:

```
Resource:
    YouTube

Methods:
    Application blocking
```

The agent understands how to interact with Cold Turkey.

---

### ESP32 Agent

Can advertise:

```
Resource:
    PS5 Power

Methods:
    Physical relay control
```

The agent understands hardware control.

---

# Capability Discovery

Agents should advertise what they can control.

The server should not assume integrations exist.

Example:

Agent connects:

```
Hello, I am Pi-hole Living Room.

I can manage:

- YouTube
- Twitch
- Reddit

through DNS blocking.
```

The server stores this information.

The user can then create logical groupings.

---

# Resource Concept

A resource represents something that can have screen time managed.

Examples:

```
YouTube
Steam
PS5
Internet
Gaming Computer
```

A resource is not necessarily tied to one implementation.

Example:

YouTube may exist as:

```
Pi-hole:
    DNS block

Cold Turkey:
    Application block
```

The user may decide those represent the same logical resource.

The exact terminology is still under consideration.

Possible names:

* Resource
* Capability
* Managed Resource
* Target
* Controlled Resource

---

# Policies

Policies define the rules for controlling resources.

Possible controls:

## Access Control

* Scheduled block windows
* Daily time allowance
* Number of unlocks per day
* Maximum session length

## Friction Controls

* Unlock delay
* Increasing delay over time
* Required typing phrase

## Override Controls

* Manual unblock request
* Temporary bypass
* Allowance extension

Example:

```
User:
    Child

Resource:
    Gaming

Policy:
    No gaming after 8 PM

Override:
    Parent approved 30 minutes
```

---

# Desired State Model

The server should push intent, not implementation.

Example:

Server sends:

```
Resource:
    YouTube

Desired state:
    Blocked

Reason:
    Daily allowance exhausted
```

The agent decides:

```
How do I accomplish this?
```

---

# Current Technology Choices

## Backend

Chosen:

* .NET 10
* ASP.NET Core
* Minimal APIs

Reason:

* Good Linux support
* Strong ecosystem
* Good background service support
* Good fit for agent communication

---

## Frontend

Planned:

* Svelte
* TypeScript
* Vite

The frontend will be a separate project.

Development:

```
Svelte Dev Server
        |
        |
ASP.NET API
```

Production:

```
ASP.NET Server

    /
    -> Svelte static files

    /api
    -> API endpoints

    /hub
    -> realtime communication
```

---

## Database

Planned:

* PostgreSQL

Development:

* PostgreSQL runs in Docker

Reason:

Docker manages infrastructure dependencies.

The .NET application will use:

* Npgsql (PostgreSQL driver)
* Entity Framework Core (object-relational mapping)

---

# Current Repository Structure

Initial structure:

```
Steward/

├── src/
│   └── Steward.Server/
│
├── frontend/
│
├── docs/
│
└── infrastructure/
```

The project is intentionally starting simple.

Additional projects should only be created when boundaries become clear.

---

# Current Backend Strategy

Start with one ASP.NET project.

Do not prematurely split into:

* Domain project
* Application project
* Infrastructure project

Those may happen later if needed.

The current goal is to discover the domain model.

---

# Development Milestones

## Milestone 1: Server Foundation

Goal:

A running API.

Tasks:

* Create ASP.NET Core project
* Create health endpoint
* Establish development workflow

Status:

In progress.

---

## Milestone 2: Agent Registration

Goal:

A fake agent can connect.

Tasks:

* Define agent model
* Agent registration endpoint
* Capability advertisement
* Heartbeat tracking

---

## Milestone 3: Persistence

Goal:

Store system state.

Tasks:

* Add PostgreSQL
* Add EF Core
* Create migrations
* Persist agents/resources

---

## Milestone 4: Real Agent

Goal:

One real enforcement client.

Possible first targets:

* Mock agent
* Pi-hole agent
* Cold Turkey agent

---

# Open Questions

## Naming

Need final names for:

* Agent capability advertisements
* Resources
* Logical groupings
* Policies

---

## Communication

Need to decide:

* REST only?
* WebSockets?
* SignalR?
* MQTT?
* Combination?

Likely direction:

* REST for configuration
* SignalR/WebSockets for live agent communication

---

## Security

Future considerations:

* Agent authentication
* User authentication
* Authorization
* Audit logs

---

# Current Philosophy

Avoid over-engineering early.

The most important design problem is not the database or UI.

The most important problem is defining:

1. What agents can advertise
2. How the server expresses desired state
3. How policies become enforcement decisions

Everything else builds on those concepts.
