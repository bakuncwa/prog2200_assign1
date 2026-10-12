# PROG2200 Assignment 1 – Asynchronous Multi-Threaded Chat Client

A Windows Forms "Network Game" whose chat panel talks to a TCP server asynchronously.
All networking lives in a separate class library; ncat (from Nmap) plays the server.

## Solution layout

```
prog2200_assign1/
├── prog2200_assign1.sln
├── start-server.ps1                    PowerShell helper: starts ncat on port 1234
├── prog2200_assign1/                   Windows Forms client (UI only)
│   ├── Program.cs
│   ├── MainForm.cs                     Menu/button handlers, event subscriptions, Invoke
│   └── MainForm.Designer.cs            Layout: game area, message box, conversation
└── prog2200_assign1_library/           Chat library (all chat functionality)
    ├── ChatClient.cs                   Connect / Send / Disconnect, send + receive threads
    └── ChatEventArgs.cs                Custom EventArgs + ConnectionState enum
```

Both projects target **.NET Framework 4.8** (C# 7.3). The UI project references the library
(Project → Add → Project Reference), so the library is built into `prog2200_assign1_library.dll`,
copied next to `prog2200_assign1.exe` and loaded by the game.

## Requirements

- Windows 10 or 11 (the .NET Framework 4.8 runtime is built in)
- Visual Studio 2022 with the ".NET desktop development" workload (includes the .NET Framework 4.8 targeting pack)
- Optional: the .NET SDK, to build and run from the command line instead of Visual Studio
- Nmap for Windows (https://nmap.org/download), which installs **ncat**
- PowerShell (built into Windows)

## Setup (PowerShell)

### 1. Install the tools

Run in PowerShell. `winget` is built into Windows 10/11. Skip anything you already have.

```powershell
winget install --id Git.Git -e
winget install --id Microsoft.PowerShell -e          # PowerShell 7 (optional - Windows PowerShell 5.1 also works)
winget install --id Insecure.Nmap -e                 # Nmap (with the Zenmap GUI) + ncat

# Visual Studio 2022 Community with the ".NET desktop development" workload (.NET Framework 4.8 support)
winget install --id Microsoft.VisualStudio.2022.Community -e `
    --override "--add Microsoft.VisualStudio.Workload.ManagedDesktop --includeRecommended --passive"

# Optional - only needed for the `dotnet build` / `dotnet run` commands below
winget install --id Microsoft.DotNet.SDK.8 -e
```

Close and reopen PowerShell so the new tools are on your `PATH`, then check them:

```powershell
git --version
dotnet --list-sdks        # only if you installed the optional .NET SDK
ncat --version
```

If `ncat` is not found, add the Nmap folder to `PATH` for the current session:

```powershell
$env:Path += ";${env:ProgramFiles(x86)}\Nmap"
```

### 2. Get the code and build it

```powershell
git clone https://github.com/bakuncwa/prog2200_assign1.git
cd prog2200_assign1
start prog2200_assign1.sln      # opens Visual Studio - then Build -> Build Solution (Ctrl+Shift+B)
# or, with the .NET SDK installed:
dotnet build
```

The game is built to `prog2200_assign1\bin\Debug\net48\prog2200_assign1.exe`, with
`prog2200_assign1_library.dll` beside it.

### 3. Allow scripts (only if `.\start-server.ps1` is blocked)

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass   # current window only
# or permanently for your user:
Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned
Unblock-File .\start-server.ps1                               # clears the "downloaded from the internet" flag
```

### 4. Configuration

The client connects to `127.0.0.1:1234`. To use a different host or port, change the constants
at the top of `prog2200_assign1/MainForm.cs`:

```csharp
private const string ServerHost = "127.0.0.1";
private const int ServerPort = 1234;
```

and start the server on the same port: `.\start-server.ps1 -Port 5000`.

When ncat first starts, Windows Firewall may ask for access. Allow it on **private** networks.

## Running it

1. **Start the server** – in PowerShell, from this folder:
   ```powershell
   .\start-server.ps1
   # same as:  ncat -l -p 1234 -t -v
   # if scripts are blocked:  powershell -ExecutionPolicy Bypass -File .\start-server.ps1
   ```
2. **Start the client** – open `prog2200_assign1.sln` in Visual Studio and press F5
   (or run `dotnet run --project prog2200_assign1`).
3. **Network → Connect** connects to `127.0.0.1:1234`. ncat prints
   `Ncat: Connection from 127.0.0.1...`.
4. Type in *Type your messages here.* and press **Send** or Enter. Type in the ncat window and
   press Enter to reply.
5. **Network → Disconnect** closes the connection. **Game → Exit** or the window's X closes the app.
   You can also stop ncat (Ctrl+C) first: the client reports
   *The server closed the connection* and can connect again later.

### Conversation tags

| Line | Meaning |
|------|---------|
| `>> Hello, Server!` | Sent by this client |
| `<< Why, hello, Client!` | Received from the server - everything after `<< ` is exactly what the server typed |
| `*** Connected to 127.0.0.1:1234 ***` | Status or error message from the client |


## Verified behaviour (library tested against a live TCP server)

- No server running → "no server is listening on that port", Connect re-enabled
- 500 messages queued in under 1 ms; all arrived at the server in order, with accented characters intact
- A server line split across two TCP packets is displayed as one message, including a character split mid-byte
- A 3,000-character line (longer than the 1,024-byte buffer) arrives as one message
- Server closes first → client reports it cleanly; further sends are rejected with a message
- Client disconnects first → server sees a clean close; disconnecting twice is harmless; reconnect works
- Server killed while the client is sending thousands of messages → exactly one disconnect, no crash

## Troubleshooting

| Problem | Fix |
|---------|-----|
| *Could not connect ... no server is listening on that port* | Start the server first (`.\start-server.ps1`) and check the ports match. |
| Port 1234 already in use | `Get-NetTCPConnection -LocalPort 1234` shows the owner; stop it or use another port. |
| ncat exits after the client disconnects | Expected - ncat serves one connection by default. Run `.\start-server.ps1` again before reconnecting. |
| Odd symbols at the start of received text | Start ncat without telnet negotiation: `ncat -l -p 1234 -v`. |
| `ncat` not recognised | See *Install the tools* above, or reopen PowerShell. |
| Running on macOS | The library and `dotnet build` work on macOS (`brew install nmap` for ncat), but the Windows Forms client only runs on Windows. Use a Windows VM to run the GUI. |
