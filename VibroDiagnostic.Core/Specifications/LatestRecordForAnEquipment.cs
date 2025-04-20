using VibroDiagnostic.Core.Entities;

namespace VibroDiagnostic.Core.Specifications;

public class LatestFilesForAnEquipment: BaseSpecification<SensorFile>
{
    public LatestFilesForAnEquipment(int equipmentId) : base(x => true)
    {
        
        AddInclude( x=> x.Sensor);
        AddInclude(x => x.Sensor.Equipment);
        ApplyFilter(x => x.Sensor.Equipment.Id == equipmentId && x.IsLatest);
    }
}