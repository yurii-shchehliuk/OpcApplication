cd ../ClientApp
powershell ng build --configuration=iis
powershell Compress-Archive -LiteralPath './dist' -DestinationPath './_dist.zip'  -Force
