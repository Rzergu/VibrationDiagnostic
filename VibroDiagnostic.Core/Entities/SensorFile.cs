namespace VibroDiagnostic.Core.Entities;

public class SensorFile
{
    public int Id { get; set; }
    public string Name { get; set; } 
    
    public string FileName { get; set; }
    public string Path { get; set; }
    

    public Sensor Sensor { get; set; }
    public int SensorId { get; set; }
    
    public bool IsLatest { get; set; }
}
