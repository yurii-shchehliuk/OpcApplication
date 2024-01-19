cd ../ClientApp
powershell ng build
powershell Compress-Archive -LiteralPath './dist' -DestinationPath './_dist.zip'  -Force
