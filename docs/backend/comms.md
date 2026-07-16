# Steward Communication Architecture

## Overview

Steward uses a message-based architecture where the Steward Server acts as the central brain and MQTT acts as the communication layer between Steward and enforcement agents.

The key architectural principle:

> Steward decides the desired state. Agents determine how to enforce that state.

Agents should not need to understand policies, users, schedules, or other system concepts. They only need to understand the resources they can manage and how to apply desired states.

---

# High-Level Architecture

```text
                         Web UI
                           |
                    REST API / SignalR
                           |
                           |
                   Steward Server
                           |
                  -----------------
                  |               |
             PostgreSQL       MQTT Client
                                  |
                                  |
                           MQTT Broker
                                  |
        ------------------------------------------------
        |                       |                      |
   ESP32 Agent            Pi-hole Agent        Cold Turkey Agent
```

---

# Component Responsibilities

## Steward Server

The Steward Server is the source of truth and the system brain.

Responsibilities:

* Manage users
* Manage devices
* Manage resources
* Evaluate policies
* Calculate desired states
* Store system state
* Communicate with agents through MQTT
* Provide APIs for the web interface
* Track agent status and reported state

The Steward Server should not contain implementation details for individual enforcement methods.

Example:

Steward understands:

```
YouTube should be blocked.
```

The Pi-hole agent understands:

```
Create DNS block rules.
```

The Cold Turkey agent understands:

```
Apply application blocking rules.
```

---

# MQTT Broker

The MQTT broker is a separate infrastructure component.

Responsibilities:

* Route messages between MQTT clients
* Handle subscriptions
* Manage retained messages
* Provide reliable message delivery

The broker does not understand Steward concepts.

It only moves messages.

Steward should not embed the MQTT broker.

Reasons:

* Keeps responsibilities separated
* Allows MQTT infrastructure to be reused by other systems
* Allows Steward to evolve independently
* Makes the communication layer replaceable

---

# MQTT Clients

Both Steward and all enforcement agents are MQTT clients.

## Steward MQTT Client

The Steward Server uses MQTT to:

* Publish desired states
* Receive agent capability advertisements
* Receive agent status reports
* Receive enforcement results

---

## Agent MQTT Clients

Agents use MQTT to:

* Register themselves
* Advertise capabilities
* Receive desired state changes
* Report actual state
* Report failures or connectivity issues

Examples:

* ESP32 relay controller
* Pi-hole DNS blocker
* Cold Turkey integration

All agents should implement the same communication contract.

---

# Agent Lifecycle

## 1. Agent Connects

An agent connects to the MQTT broker.

Example:

```
ESP32 Agent
    |
    |
MQTT Broker
    |
    |
Steward
```

---

## 2. Agent Advertises Capabilities

Agents announce what they can manage.

Example:

```json
{
  "agent": "Living Room Pi-hole",
  "capabilities": [
    {
      "resource": "YouTube",
      "method": "DNS Blocking"
    }
  ]
}
```

The server stores this information.

The server should not assume what agents are capable of.

---

## 3. Steward Calculates Desired State

Policies are evaluated by Steward.

Example:

Current situation:

```
User:
    Child

Policy:
    Gaming blocked after 8 PM
```

Steward determines:

```
PS5 = Blocked
```

---

## 4. Steward Publishes Desired State

Steward publishes a message through MQTT.

Example:

Topic:

```
steward/resources/ps5/desired-state
```

Payload:

```json
{
  "state": "blocked",
  "reason": "Daily limit exceeded"
}
```

---

## 5. Agent Reconciles State

The agent receives the desired state.

The agent decides how to achieve it.

Example:

ESP32:

```
Desired:
    PS5 blocked

Action:
    Turn relay off
```

Pi-hole:

```
Desired:
    YouTube blocked

Action:
    Update DNS block list
```

---

## 6. Agent Reports Actual State

The agent publishes the result.

Example:

Topic:

```
steward/agents/esp32-001/reported-state
```

Payload:

```json
{
  "resource": "PS5",
  "state": "blocked",
  "success": true
}
```

Steward stores the reported state.

---

# Desired State vs Commands

Steward should avoid thinking in terms of commands.

Avoid:

```
Turn off PS5
```

because commands can be lost if an agent is offline.

Instead:

```
PS5 desired state:
    Blocked
```

The agent should always reconcile its actual state toward the desired state.

This provides resilience.

Example:

```
Agent offline

Steward:
    PS5 should be blocked

Agent reconnects

Agent:
    What is my desired state?

Steward:
    PS5 is blocked

Agent:
    Applying block
```

---

# Web UI Communication

The Web UI is not an MQTT client.

The UI is a user interface, not an enforcement agent.

The preferred communication path:

```
Web UI
   |
   |
REST API
   |
   |
Steward Server
```

The UI uses REST for:

* Viewing data
* Creating policies
* Managing users
* Requesting overrides
* Configuring resources

---

## Optional SignalR Support

SignalR may be added for real-time UI updates.

Example:

```
ESP32 reports offline
        |
        |
       MQTT
        |
        |
Steward Server
        |
        |
     SignalR
        |
        |
     Web UI
```

SignalR is for improving user experience, not for agent communication.

---

# Communication Summary

| Component             | Protocol           | Purpose                        |
| --------------------- | ------------------ | ------------------------------ |
| Web UI → Steward      | REST API           | User actions and configuration |
| Steward → Web UI      | SignalR (optional) | Live updates                   |
| Steward ↔ MQTT Broker | MQTT               | Agent communication            |
| Agents ↔ MQTT Broker  | MQTT               | Capability, state, enforcement |

---

# Design Principles

1. Agents advertise capabilities instead of Steward assuming them.

2. Steward manages desired state, not implementation details.

3. Agents reconcile desired state into real-world actions.

4. MQTT is the single communication protocol between Steward and enforcement agents.

5. The MQTT broker is separate infrastructure.

6. The Web UI interacts with Steward, not directly with agents.
