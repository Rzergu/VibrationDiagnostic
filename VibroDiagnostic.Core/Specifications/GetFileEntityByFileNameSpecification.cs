using VibroDiagnostic.Core.Entities;

namespace VibroDiagnostic.Core.Specifications;

public class GetFileEntityByFileNameSpecification: BaseSpecification<SensorFile>
{
    public GetFileEntityByFileNameSpecification(string fileSensorName) : base(item => item.Name == fileSensorName)
    {
    }
}