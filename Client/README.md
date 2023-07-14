# Intro
Purpose of the OpcClient is to pull data from remote OPC server.

## Setup
Depending whether this should be part of QiaService like a plugin or standalone application differs app setup

### • Plugin
App is executed from outside and goes to overriden Init method

### • Standalone.Exe
Run app using .exe file and configure app behaviour in qia.opc folder
Also you can run from IDLE and just set QIA.Plugin.Opc as startup

### • Standalone.Docker
To run app directly set the Docker as launch profile
To run with orchestration set docker-compose as startup. Following this way DB will be automaticaly created.

## Configuratin
In appsettings.json - main app configuration
• OpcUrl
• DB connection string
• AzureEventHub credentials
- advanced configuration:
• SaveToDb - save static values to db
• SaveToAzure - send data to eventhub
• RecreateDb - if we want to drop db and recreate

nodemanager.json - configuration to lookup for particular nodes on the server
• NodeId 
• Name - custom node name
• NodeType - if equals to Subscription will be monitoring value on server by accepting events
• Range - how many nodes to skip and then save to the DB
• Msecs - how often save to db

AppConfig.json - defines minor configuration when searching for nodes
• Name - (Initial/Advanced/Slim) app pulls the last one from settings
• SkipPredefined - skips data like ServerStatus/Aliases
• CreateFullTree - if we want full tree to be created
// this .json will be removed in future

## Logging 
Currently Serilog+Seq is using. Logs are saved locally and also sends to the http://localhost:5341
// will be modificated in future