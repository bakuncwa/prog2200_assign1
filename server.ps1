# Simple TCP Echo Server
[System.Net.ServicePointManager]::DefaultConnectionLimit = 100

$listener = New-Object System.Net.Sockets.TcpListener([System.Net.IPAddress]::Any, 1234)
$listener.Start()

Write-Host "TCP Server started on port 1234" -ForegroundColor Green
Write-Host "Waiting for connections..." -ForegroundColor Cyan

try {
	while ($true) {
		# Accept client connection with timeout
		$client = $listener.AcceptTcpClient()
		Write-Host "Client connected: $($client.Client.RemoteEndPoint)" -ForegroundColor Green

		# Get network stream
		$stream = $client.GetStream()
		# Don't set timeouts - just wait indefinitely for data

		# Create reader/writer
		$reader = New-Object System.IO.StreamReader($stream)
		$writer = New-Object System.IO.StreamWriter($stream)
		$writer.AutoFlush = $true

		# Echo messages - ReadLine() blocks until a message arrives or connection closes
		try {
			while ($true) {
				$line = $reader.ReadLine()
				if ($line -eq $null) {
					# Null means client disconnected
					break
				}
				Write-Host "Received: $line" -ForegroundColor Yellow
			}
		}
		catch {
			Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
		}

		$client.Close()
		Write-Host "Client disconnected" -ForegroundColor Red
	}
}
catch {
	Write-Host "Server error: $($_.Exception.Message)" -ForegroundColor Red
}
finally {
	$listener.Stop()
}
