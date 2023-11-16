# SensorMonitoring

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
