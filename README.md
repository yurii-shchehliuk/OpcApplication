# Dev init
- Start the HMI (ClientApp) application via "ng serve" command 
- Run test server, run OPC.Server.sln with "Sample Server" as startup, and copy the endpoint url from text box
- Run OPC.Client, it will create local DB atomatically

After its done, go to localhost:4200 and create a session (hub source)
Then add node to subscription (ns=2;i=10847 or ns=2;i=10848)