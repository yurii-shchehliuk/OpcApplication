cd ../OPC.Client/QIA.Opc.API
:: dotnet publish -c Release -r win-x64 --configfile "./Packages.config"
dotnet publish -c Release -r win-x64 -p:PublishReadyToRun=true --self-contained true

cd bin/Release/net6.0/win-x64
powershell Compress-Archive -LiteralPath './publish' -DestinationPath './_publish.zip'  -Force
