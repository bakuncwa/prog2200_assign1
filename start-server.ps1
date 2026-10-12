# Starts an ncat chat server for the Network Game client to connect to.
# Usage (PowerShell):   .\start-server.ps1            # listens on port 1234
#                       .\start-server.ps1 -Port 5000
# If scripts are blocked:  powershell -ExecutionPolicy Bypass -File .\start-server.ps1

param(
    [int]$Port = 1234
)

# Use ncat from PATH, otherwise fall back to the default Nmap install folders on Windows.
$ncat = (Get-Command ncat -ErrorAction SilentlyContinue).Source
if (-not $ncat) {
    $ncat = @(
        "${env:ProgramFiles(x86)}\Nmap\ncat.exe",
        "$env:ProgramFiles\Nmap\ncat.exe"
    ) | Where-Object { $_ -and (Test-Path $_) } | Select-Object -First 1
}

if (-not $ncat) {
    Write-Error "ncat was not found. Install Nmap (https://nmap.org/download) - ncat is included."
    exit 1
}

Write-Host "Starting chat server on port $Port using $ncat"
Write-Host "Type a message and press Enter to send it to the client. Press Ctrl+C to stop the server."
Write-Host ""

# -l listen, -p port, -t answer telnet negotiation, -v verbose (shows when the client connects)
& $ncat -l -p $Port -t -v
