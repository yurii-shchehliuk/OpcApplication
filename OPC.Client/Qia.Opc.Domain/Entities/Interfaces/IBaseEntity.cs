namespace Qia.Opc.Domain.Entities.Interfaces;

public interface IBaseEntity
{
    string Guid { get; set; }
    DateTime CreatedAt { get; set; }
    //public DateTime? UpdatedAt { get; set; }
}
