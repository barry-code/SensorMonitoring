# Deploy Script
# Run this script to deploy the api/blazorSite to the raspberry pi.
# Note: this script assumes Putty is installed, and Putty path variable set, as there are two exes used by putty (plink.exe + pscp.exe) which are needed here.

param (
    [string]$Target = "all"  # Options: api, blazor, all
)

# SETTINGS
$raspberryPiUser = "b"
$raspberryPiHost = "pidev"
$apiService = "bcode.sensormonitoring.api.service"
$blazorService = "bcode.sensormonitoring.blazorserverapp.service"
$apiRemotePath = "/home/b/MyApps/SensorMonitoring.Api/publish"
$blazorRemotePath = "/home/b/MyApps/SensorMonitoring.BlazorServerApp/publish"
$apiProjectPath = "C:\BCode\SensorMonitoring\Api"
$apiPublishLocal = "C:\temp\SensorMonitorPublish_Api";
$blazorProjectPath = "C:\BCode\SensorMonitoring\BlazorServerApp"
$blazorPublishLocal = "C:\temp\SensorMonitorPublish_Blazor";

# TOOLS
$plink = "plink.exe"
$pscp = "pscp.exe"

# Prompt once for password
if (-not $global:plainPassword) {
    $securePassword = Read-Host -Prompt "Enter password for $raspberryPiUser@$raspberryPiHost" -AsSecureString
    $global:plainPassword = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
        [Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
    )
}

function Run-CheckedCommand {
    param(
        [Parameter(Mandatory=$true)]
        [string]$Description,

        [Parameter(Mandatory=$true)]
        [scriptblock]$Command
    )

    Write-Host "`nStarting: $Description"
    & $Command
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed: $Description" -ForegroundColor Red
        exit 1
    } else {
        Write-Host "Success: $Description" -ForegroundColor Green
    }
}

function Deploy-App {
    param (
        [string]$name,
        [string]$localProjectPath,
        [string]$remotePath,
        [string]$serviceName
    )

    $publishOutput = "$env:TEMP\publish\$name"

    if (Test-Path $publishOutput) {
        Remove-Item -Recurse -Force $publishOutput
    }

    Run-CheckedCommand -Description "Publishing $name project" -Command {
        dotnet publish "$localProjectPath" -c Release -o $publishOutput
    }

    Run-CheckedCommand -Description "Stopping $name service on Pi" -Command {
        & $plink -pw $global:plainPassword -batch "$raspberryPiUser@$raspberryPiHost" "sudo systemctl stop $serviceName"
    }

    Run-CheckedCommand -Description "Copying $name files to Pi" -Command {
        & $pscp -pw $global:plainPassword -r "$publishOutput\*" "${raspberryPiUser}@${raspberryPiHost}:${remotePath}"
    }

    Run-CheckedCommand -Description "Starting $name service on Pi" -Command {
        & $plink -pw $global:plainPassword -batch "$raspberryPiUser@$raspberryPiHost" "sudo systemctl start $serviceName"
    }

    Write-Host "`n$name deployed successfully!`n"
}

if ($Target -eq "api" -or $Target -eq "all") {
    Deploy-App -name "API" -localProjectPath $apiProjectPath -remotePath $apiRemotePath -serviceName $apiService
}
if ($Target -eq "blazor" -or $Target -eq "all") {
    Deploy-App -name "Blazor" -localProjectPath $blazorProjectPath -remotePath $blazorRemotePath -serviceName $blazorService
}

$global:plainPassword = $null