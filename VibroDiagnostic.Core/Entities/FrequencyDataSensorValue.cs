namespace VibroDiagnostic.Core.Entities;

public class FrequencyDataSensorValue
{
    public int SensorId { get; set; }
    public DateTime Date { get; set; }
    public IEnumerable<FrequencyDataSensorValuePoint> SensorsFrequencyDataPoints { get; set; }
}