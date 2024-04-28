# SensorMonitoring

## What?
Sensor Monitoring API and Blazor website for logging and displaying temperature and humidity sensor data.

## Why?
Developed for purpose of monitoring and storing data from temperature and humidity sensors in the house, and allowing display live data as well as trend historical data over time. Main purpose was to track humidty in attic over time, and allow to gauge efforts to reduce humidty and how successful it was.

## Screenshots
![image](https://github.com/barry-code/SensorMonitoring/assets/60239072/936f0d5a-6ffa-49a8-94bd-dcdc3995a9f7)

![image](https://github.com/barry-code/SensorMonitoring/assets/60239072/26ebc481-b2c0-44f0-83a4-20d8193ce1e4)

## Solution Overview
Solution consists of:
- an API which is used for writing/reading sensor readings, storing data in sqlite DB.
- a blazor server app, used for displaying sensor readings data.
- also included for reference, is the script running on arduino which is responsible for reading the physically connected temperature and humidity sensors, and writing said values to the api for storage.

## Third Party Libraries
Mudblazor (used for design of blazor server app)
[Mudblazor Github](https://github.com/MudBlazor/MudBlazor)

Plotly.Blazor (used for charts in the blazor server app)
[Plotly.Blazor Github]([https://github.com/MudBlazor/MudBlazor](https://github.com/LayTec-AG/Plotly.Blazor)

## Deployment Notes
Notes when deploying to raspberry pi:

[Microsoft Arm Deploy Tutorial](https://learn.microsoft.com/en-us/dotnet/iot/deployment#deploying-a-framework-dependent-app)

[Applying Migrations Tutorial](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli)


Replace myUsername with the account name on pi, and myHostname with the hostname of pi, and myHomePath with actual path on rpi. Replace ip and ports with correct ones.


First published api, then copied over to rpi using scp.
>scp -r /publish/* myUsername@myHostname:/myHomePath/MyApps/SensorMonitoring.Api

Created efcore bundler to allow running migrations on rpi. Think I also created db manually using sqlite3 command but that should be unneccessary.
>dotnet ef migrations bundle --project="C:\myWindowsPath\SensorMonitoring\src\Api\Api.csproj" --self-contained -r linux-arm64 --configuration --verbose

Copied bundler over to rpi
>scp -r efbundle myUsername@myHostname:/myHomePath/MyApps/SensorMonitoring.Api/

On rpi, ensured user had rights to run bundler
>chmod +x efbundle

Then ran it (had to pass connection string)
>./efbundle --verbose --no-color --connection="Data Source=/myHomePath/MyApps/Db/SensorMonitoring.db;"

Migrations showed as being generated.

Was then able to run api, passing url so that it ran as ip address instead of localhost, and could be access remotely.
>dotnet SensorMonitoring.Api.dll --urls "http://rpiIpAddress:port"

Also had firewall off on pc when testing, would need to ensure rule added for port.

Created a service to run it using service file which is stored in /etc/systemd/system/
>systemctl status bcode.sensormonitoring.api.service

Same process used for deploying and running the BlazorServerApp.
>scp -r publish myUsername@myHostname:/myHomePath/MyApps/SensorMonitoring.BlazorServerApp

>systemctl status bcode.sensormonitoring.blazorserverapp.service
