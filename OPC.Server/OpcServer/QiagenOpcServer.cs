using Opc.Ua;
using Opc.Ua.Server;
using System.Collections.Generic;

namespace OpcServer
{
    class QiagenOpcServer : StandardServer
    {
        protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server, ApplicationConfiguration configuration)
        {
            Utils.Trace("Creating the Node Managers.");

            List<INodeManager> nodeManagers = new List<INodeManager>();

            // create the custom node managers.
            nodeManagers.Add(new QiagenOpcNodeManager(server, configuration));

            // create master node manager.
            return new MasterNodeManager(server, configuration, null, nodeManagers.ToArray());
        }

        protected override ServerProperties LoadServerProperties()
        {
            ServerProperties properties = new ServerProperties();

            properties.ManufacturerName = "Industry40tv";
            properties.ProductName = "QiagenOpc InformationModel Server";
            properties.ProductUri = "http://opcfoundation.org/QiagenOpc/InformationModelServer/v1.0";
            properties.SoftwareVersion = Utils.GetAssemblySoftwareVersion();
            properties.BuildNumber = Utils.GetAssemblyBuildNumber();
            properties.BuildDate = Utils.GetAssemblyTimestamp();

            // TBD - All applications have software certificates that need to added to the properties.

            return properties;
        }
    }
}
