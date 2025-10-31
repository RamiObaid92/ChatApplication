# ChatApplication

Realtime Chat Application using WebSocket (SignalR), built primarily with C#/.NET and a small web UI layer.

- Repository: [RamiObaid92/ChatApplication](https://github.com/RamiObaid92/ChatApplication)
- Solution: [ChatApplication.slnx](https://github.com/RamiObaid92/ChatApplication/blob/main/ChatApplication.slnx)

## Overview

This project demonstrates how to build a real‑time chat experience over WebSockets using ASP.NET Core SignalR. It includes a server project that hosts the SignalR hub and a basic web interface to connect and exchange messages.

Core capabilities:
- Real‑time, low‑latency messaging via SignalR hubs
- Web client for sending/receiving messages
- Extensible architecture for features like rooms, presence, persistence, and authentication

## Tech Stack

Languages detected in the repository:
- C#: ~82%
- TypeScript: ~15%
- JavaScript, CSS, HTML: ~3% combined

Key technologies:
- ASP.NET Core + SignalR for real‑time communication
- Web client built with TypeScript/JavaScript, HTML, and CSS

## Repository Structure

- [.gitattributes](https://github.com/RamiObaid92/ChatApplication/blob/main/.gitattributes)
- [.gitignore](https://github.com/RamiObaid92/ChatApplication/blob/main/.gitignore)
- [ChatApplication.Server/](https://github.com/RamiObaid92/ChatApplication/tree/main/ChatApplication.Server) — ASP.NET Core server and SignalR hub(s)
- [ChatApplication.Tests/](https://github.com/RamiObaid92/ChatApplication/tree/main/ChatApplication.Tests) — Test project
- [ChatApplication.slnx](https://github.com/RamiObaid92/ChatApplication/blob/main/ChatApplication.slnx) — Solution file
- [README.md](https://github.com/RamiObaid92/ChatApplication/blob/main/README.md)

Tip: In the server project, look for the Program/Startup configuration that maps the SignalR hub, and a Hub class (e.g., ChatHub) that defines the real‑time methods for sending/receiving messages.

## Getting Started

### Prerequisites
- .NET SDK (use the version targeted by the server project; you can check with `dotnet --info`)
- A modern web browser

### Clone and Run

```bash
# Clone
git clone https://github.com/RamiObaid92/ChatApplication.git
cd ChatApplication

# Restore and build
dotnet restore
dotnet build --configuration Release

# Run the server
dotnet run --project ChatApplication.Server
```

When the server starts, the console output will show the HTTP/HTTPS URLs (e.g., http://localhost:5xxx). Open the URL in your browser to use the web client.

### Development mode

```bash
# Rebuild and run with hot reload where supported
dotnet watch --project ChatApplication.Server run
```

### Tests

```bash
dotnet test
```

## Configuration

Typical ASP.NET Core settings (e.g., logging, CORS, allowed hosts) are managed via `appsettings.json` and environment variables. If you need to change ports, CORS policies, or hub routes, update the server configuration (Program/Startup) accordingly.

## How It Works

- The server hosts a SignalR hub that manages connections and relays messages between clients.
- The web client connects to the hub over WebSockets (or falls back to another transport supported by SignalR if needed).
- Clients can send messages to the hub, which broadcasts or routes them to other connected clients.

To extend the app:
- Add groups/rooms to isolate conversations
- Implement user presence and typing indicators
- Persist messages in a database
- Add authentication/authorization for private chats

## Troubleshooting

- If the client cannot connect to the hub, verify:
  - The hub route is correctly mapped in the server
  - The browser is connecting to the same base URL and hub path
  - CORS is configured to allow your client origin (if serving from a different host/port)
- Check console logs in both the browser and the server for detailed errors.

## Roadmap Ideas

- Rooms and private messaging
- Message persistence with a database
- Authentication/authorization (e.g., JWT or cookies)
- Presence, typing indicators, and delivery/read receipts
- Basic UI improvements and accessibility

## Contributing

Issues and PRs are welcome. If you plan a larger contribution, consider opening an issue first to discuss the approach.

## License

This repository does not currently include a license file. If you plan to use this code beyond personal experimentation, please contact the repository owner for licensing terms.

## Acknowledgements

- Microsoft SignalR documentation: https://learn.microsoft.com/aspnet/core/signalr
