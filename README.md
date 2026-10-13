# PROG2200 Assignment 1 - Chat Client

Windows Forms TCP chat with async/await and background threads. Networking encapsulated in class library.
Server: **ncat** (Nmap) or PowerShell test server.

## Quick Start

1. **Install ncat** (included in Nmap): 
   ```powershell
   winget install --id Insecure.Nmap -e
   ```

2. **Clone and build**:
   ```powershell
   git clone https://github.com/bakuncwa/prog2200_assign1.git
   cd prog2200_assign1
   start prog2200_assign1.sln      # Ctrl+Shift+B to build
   ```

3. **Start server**:
   ```powershell
   & "C:\Program Files (x86)\Nmap\ncat.exe" -l -p 1234 -t -v
   # Or: powershell -ExecutionPolicy Bypass -File server.ps1
   ```

4. **Run client**: Press **F5** in Visual Studio

5. **Chat**: 
   - Click **Network → Connect**
   - Type message → Press **Send** or Enter
   - Text auto-clears after sending
   - Type in server terminal to reply

## Architecture

| Component | Purpose |
|-----------|---------|
| **ChatClient.cs** | Async connect, background send/receive threads, events |
| **StateObj.cs** | Per-connection state: socket, buffer, message queue |
| **ChatEvents.cs** | Event delegates and custom EventArgs |
| **MainForm.cs** | UI handlers, event subscriptions, thread-safe Invoke |

## How It Works

- **Async Connection**: ConnectAsync with 5-sec timeout (no UI freeze)
- **Receive Thread**: Reads bytes, raises MessageReceived per line (CR+LF)
- **Send Thread**: BlockingCollection<string> queues messages, raises MessageSent
- **Thread Safety**: Lock guards state; BeginInvoke protects UI thread
- **Line Protocol**: Works with ncat, telnet, any TCP line-based server

## Verified Behavior

- 500 messages sent in order; accented characters intact
- Multi-packet lines merged into single message
- Large lines (3000+ chars) handled correctly
- Clean disconnect/reconnect; crash-proof even if server killed

## Config

Edit `prog2200_assign1/MainForm.cs` to change host/port:
```csharp
private const string ServerHost = "127.0.0.1";
private const int ServerPort = 1234;
```

## Quick Start

1. Install Nmap: winget install --id Insecure.Nmap -e
2. Clone: git clone https://github.com/bakuncwa/prog2200_assign1.git
3. Build: Open prog2200_assign1.sln in Visual Studio, press Ctrl+Shift+B
4. Start server: & 'C:\Program Files (x86)\Nmap\ncat.exe' -l -p 1234 -t -v
5. Run client: Press F5 in Visual Studio

## Features

- Async connection with 5-sec timeout (no UI freeze)
- Receive thread: reads bytes, raises MessageReceived per line
- Send thread: BlockingCollection queues messages, raises MessageSent
- Thread-safe: lock guards state, BeginInvoke protects UI
- Compatible with ncat, telnet, line-based TCP servers (CR+LF)

## Testing

Click Network > Connect, type messages, click Send. Messages auto-clear after sending. Server shows received lines in terminal. Type in server terminal to reply.
