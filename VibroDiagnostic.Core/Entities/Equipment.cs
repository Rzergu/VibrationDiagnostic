namespace VibroDiagnostic.Core.Entities;

public class Equipment
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Sensor> Sensors { get; set; }
}