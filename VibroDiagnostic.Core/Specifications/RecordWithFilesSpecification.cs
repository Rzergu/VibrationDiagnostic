using System.Linq.Expressions;
using VibroDiagnostic.Core.Entities;

namespace VibroDiagnostic.Core.Specifications;

public class SensorWithFilesSpecification : BaseSpecification<Sensor>
{
    public SensorWithFilesSpecification(int id) : base(x => x.Id == id)
    {
        AddInclude(x => x.Files);
    }
}