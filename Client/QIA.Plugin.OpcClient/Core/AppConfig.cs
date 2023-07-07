namespace QIA.Plugin.OpcClient.Core
{
    public class AppConfig
    {
        //public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "Configuration #";
        public bool SkipPredefined { get; set; } = true;
        public bool CreateFullTree { get; set; } = true;
    }
}
