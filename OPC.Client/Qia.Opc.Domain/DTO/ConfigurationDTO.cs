namespace Qia.Opc.Domain.DTO
{
	public class ConfigurationDTO
	{
		public bool CreateFullTree { get; set; } = true;
		public IEnumerable<string> AreasToIgnore { get; set; }
	}
}
