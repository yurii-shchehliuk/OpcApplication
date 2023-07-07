cd ../Client/QIA.Plugin.OpcClient
dotnet publish -c Debug -r win-x64
:: dotnet publish -c Release -r win-x64 -p:PublishReadyToRun=true
cd bin/Debug/net6.0/win-x64
powershell Compress-Archive -LiteralPath './publish' -DestinationPath './_publish.zip'
