# Steward

Steward is an open-source screen time management and policy enforcement platform.

It provides a central system for managing users, devices, and resources while allowing different enforcement methods to be integrated through modular agents.

## Why Steward?

Screen time management tools are often fragmented. One tool might manage a desktop, another might control a network, and another might handle a game console. Steward aims to provide a single place to define policies and let different systems enforce them.

The goal is not to replace every existing tool, but to connect them into one flexible system.

## How It Works

Steward uses a server-and-agent architecture:

* **Steward Server** manages policies, users, devices, and assignments.
* **Agents** connect to Steward and provide enforcement capabilities for specific platforms or services.

Examples of possible agents:

* Desktop application blockers
* Network-level controls
* Smart device controllers
* Custom hardware integrations

Agents handle the details of enforcement, allowing the server to remain platform-independent.

## Project Status

Steward is currently under active development. The core architecture is being built and APIs are still evolving.

It started as a personal project to solve a problem in my own home, but it is being developed openly in case it can be useful to others as well.

## Local setup

With the .NET 10 SDK installed, run these commands from the repository root:

```sh
dotnet run --project src/Steward.Server -- setup
dotnet run --project src/Steward.Server
```

Setup applies database migrations and asks for your admin name, email, and a
six-digit PIN (entered twice, hidden). It stores a salted hash and never prints
the PIN. Setup cannot be repeated once completed or if an admin already exists.
Normal startup applies migrations and exits with instructions if setup is needed.

Use the same connection string for setup and normal startup. The default local
database is `src/Steward.Server/steward.db` when using the commands above.
For a published build, run `dotnet Steward.Server.dll setup` from its directory.
Container deployments need an interactive terminal and the same persistent
database volume for setup and startup; the current Compose file only runs MQTT.

Email is collected for future recovery; setup does not verify or send email.
Web sign-in, sessions, and email recovery are not implemented yet. See
[the authentication design](docs/authentication.md) for account rules.

## Contributing

Contributions, ideas, and discussions are welcome. See [CONTRIBUTING.md](CONTRIBUTING.md) for more information.

## License

Steward is licensed under the GNU Affero General Public License v3.0 (AGPLv3).


    Steward - An open-source screen time management and policy enforcement platform.
    Copyright (C) 2026 Tyson Freeze

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.