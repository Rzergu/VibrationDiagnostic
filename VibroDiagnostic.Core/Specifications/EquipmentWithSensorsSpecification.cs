using VibroDiagnostic.Core.Entities;

namespace VibroDiagnostic.Core.Specifications;

public class EquipmentWithSensorsSpecification : BaseSpecification<Equipment>
{
    public EquipmentWithSensorsSpecification(int id) : base(item => item.Id == id)
    {
        AddInclude(x => x.Sensors);
    }
}