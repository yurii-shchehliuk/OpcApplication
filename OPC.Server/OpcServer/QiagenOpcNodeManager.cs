using Opc.Ua;
using Opc.Ua.Server;
using OpcServer.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace OpcServer
{
    class QiagenOpcNodeManager : CustomNodeManager2
    {
        public QiagenOpcNodeManager(IServerInternal server, ApplicationConfiguration configuration)
            : base(server, configuration)
        {
            SystemContext.NodeIdFactory = this;

            // set one namespace for the type model and one names for dynamically created nodes.
            string[] namespaceUrls = new string[2];
            namespaceUrls[0] = Data.Namespaces.BatchPlant;
            namespaceUrls[1] = Data.Namespaces.BatchPlant + "/Instance";
            SetNamespaces(namespaceUrls);

            // get the configuration for the node manager.
            m_configuration = configuration.ParseExtension<QiagenOpcServerConfiguration>();

            // use suitable defaults if no configuration exists.
            if (m_configuration == null)
            {
                m_configuration = new QiagenOpcServerConfiguration();
            }

            SimulationVarriableInit();
        }

        protected override NodeStateCollection LoadPredefinedNodes(ISystemContext context)
        {
            NodeStateCollection predefinedNodes = new NodeStateCollection();
            predefinedNodes.LoadFromBinaryResource(context,
                Directory.GetCurrentDirectory() + "/Data/BatchPlant.PredefinedNodes.uanodes",
                typeof(QiagenOpcNodeManager).GetTypeInfo().Assembly,
                true);

            return predefinedNodes;
        }

        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            lock (Lock)
            {
                LoadPredefinedNodes(SystemContext, externalReferences);

                // find the untyped Batch Plant 1 node that was created when the model was loaded.
                BaseObjectState passiveNode = (BaseObjectState)FindPredefinedNode(new NodeId(Data.Objects.BatchPlant1, NamespaceIndexes[0]), typeof(BaseObjectState));

                // convert the untyped node to a typed node that can be manipulated within the server.
                m_QiagenOpc1 = new BatchPlantState(null);
                m_QiagenOpc1.Create(SystemContext, passiveNode);

                // replaces the untyped predefined nodes with their strongly typed versions.
                AddPredefinedNode(SystemContext, m_QiagenOpc1);
                AddPredefinedNode(SystemContext, simulatedVariable);

                m_QiagenOpc1.StartProcess.OnCallMethod = new GenericMethodCalledEventHandler(OnStartProcess);
                m_QiagenOpc1.StopProcess.OnCallMethod = new GenericMethodCalledEventHandler(OnStopProcess);

                m_simulationTimer = new System.Threading.Timer(DoSimulation, null, 1000, 1000);

            }
        }

        // All simulations are made by timer event
        public void DoSimulation(object state)
        {
            //Console.WriteLine($"[{DateTime.UtcNow}]");
            var random = new Random();
            double randomValue = random.Next(15, 40);
            m_QiagenOpc1.Mixer.LoadcellTransmitter.Output.Value = randomValue;
            //Console.WriteLine(nameof(m_QiagenOpc1.Mixer.LoadcellTransmitter) + ": " + randomValue);

            randomValue = random.Next(15, 40);
            m_QiagenOpc1.Mixer.LoadcellTransmitter.ExcitationVoltage.Value = randomValue;
            //Console.WriteLine(nameof(m_QiagenOpc1.Mixer.LoadcellTransmitter.ExcitationVoltage) + ": " + randomValue);
            //Debug.Write("\n" + randomValue.ToString());

            // Generate a random double.
            randomValue = random.Next(15, 40);
            // Assign the random value to the simulated variable.
            simulatedVariable.Value = randomValue;
            simulatedVariable.Timestamp = DateTime.Now;
            simulatedVariable.ClearChangeMasks(SystemContext, false);
            //Console.WriteLine(nameof(simulatedVariable) + ": " + randomValue);
        }
        private void SimulationVarriableInit()
        {
            simulatedVariable = new BaseDataVariableState(null)
            {
                NodeId = new NodeId("SimulatedVariable", NamespaceIndex),
                BrowseName = new QualifiedName("SimulatedVariable", NamespaceIndex),
                DisplayName = new LocalizedText("SimulatedVariable"),
                DataType = DataTypeIds.Double,
                ValueRank = ValueRanks.Scalar,
            };

            // Set initial value.
            simulatedVariable.Value = 0;
            simulatedVariable.Timestamp = DateTime.Now;
            simulatedVariable.Value = StatusCodes.Good;
        }

        private ServiceResult OnStartProcess(ISystemContext context, MethodState method, IList<object> inputArguments,
    IList<object> outputArguments)
        {
            return ServiceResult.Good;
        }

        private ServiceResult OnStopProcess(ISystemContext context, MethodState method, IList<object> inputArguments,
IList<object> outputArguments)
        {

            return ServiceResult.Good;
        }

        private QiagenOpcServerConfiguration m_configuration;
        private static BatchPlantState m_QiagenOpc1;
        private System.Threading.Timer m_simulationTimer;
        private BaseDataVariableState simulatedVariable;



    }
}
