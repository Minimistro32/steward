# Contributing to Steward

Thanks for your interest in Steward! 

I started Steward as a personal project to solve a problem I had managing screen time across my own devices. I built it as a solution for me, but I'm happy if it is useful to others too.

Contributions are welcome, but I have no expectations. Whether you want to improve the project, build an integration, or simply use Steward for your own setup, I appreciate your interest.

## Reporting Issues

If you find a bug or have an idea, open an issue with enough detail to understand the problem:

* What happened?
* What did you expect to happen?
* How can it be reproduced?
* Any relevant logs or screenshots

## Code Contributions

For larger changes, it is helpful to open an issue first so we can discuss the approach.

When submitting code:

* Keep changes focused
* Follow the existing style and architecture
* Update documentation when behavior changes
* Include tests where practical

## Agent Development

Agents are a core part of Steward's design. The goal is to allow Steward to manage many different devices and enforcement systems without the server needing to know the implementation details.

New agents should generally:

* Communicate with Steward through the defined agent interface
* Handle platform-specific enforcement logic themselves
* Keep the server and other agents unaware of implementation details

Examples of possible agents include:

* Desktop screen time integrations
* Network-level controls
* Smart device controllers
* Parental control integrations
* Custom hardware enforcement

If you are building a new agent or integration, feel free to open a discussion about the design.

## Licensing

Steward is licensed under the GNU Affero General Public License v3.0 (AGPLv3).

By submitting a contribution to Steward, you agree that your contribution may be distributed under the project's AGPLv3 license.

Contributions remain attributed to their original authors. The Steward project may include submitted contributions as part of the project under the terms of the AGPLv3 license.

## Development Questions

If you are unsure where a change belongs or want to discuss an idea, open an issue or discussion. Early feedback is welcome.

Thanks for helping improve Steward!
