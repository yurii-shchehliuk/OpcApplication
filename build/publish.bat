cd ../OPC.Client/QIA.Plugin.OpcClient
dotnet publish -c Debug -r win-x64 --configfile "./Packages.config"
:: dotnet publish -c Release -r win-x64 --configfile "./Packages.config" -p:PublishReadyToRun=true
cd bin/Debug/net6.0/win-x64
powershell Compress-Archive -LiteralPath './publish' -DestinationPath './_publish.zip'  -Force
