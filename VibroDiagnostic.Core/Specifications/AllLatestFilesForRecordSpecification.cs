using System.Linq.Expressions;
using VibroDiagnostic.Core.Entities;

namespace VibroDiagnostic.Core.Specifications;

public class AllLatestFilesForSensorSpecification : BaseSpecification<SensorFile>
{
    public AllLatestFilesForSensorSpecification(int SensorId) : base( e => true)
    {
        AddInclude(e => e.Sensor);
        ApplyFilter(e => e.Sensor.Id == SensorId && e.IsLatest);
    }
}