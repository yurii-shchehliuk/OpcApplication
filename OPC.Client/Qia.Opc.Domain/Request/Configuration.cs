namespace QIA.Opc.Domain.Request
{
    public class Configuration
    {
        public bool CreateFullTree { get; set; } = true;
        public IEnumerable<string> AreasToIgnore { get; set; }
    }
}
