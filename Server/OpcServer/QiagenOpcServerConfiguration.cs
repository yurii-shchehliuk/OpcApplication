using OpcServer.Data;
using System.Runtime.Serialization;


namespace OpcServer
{
    [DataContract(Namespace = Namespaces.BatchPlant)]
    public class QiagenOpcServerConfiguration
    {
        public QiagenOpcServerConfiguration()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes the object during deserialization.
        /// </summary>
        [OnDeserializing()]
        private void Initialize(StreamingContext context)
        {
            Initialize();
        }

        /// <summary>
        /// Sets private members to default values.
        /// </summary>
        private void Initialize()
        {
        }
    }
}
