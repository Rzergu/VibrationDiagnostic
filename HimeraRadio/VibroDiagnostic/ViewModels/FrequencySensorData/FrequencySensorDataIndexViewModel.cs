namespace VibroDiagnostic.ViewModels.FrequencySensorData;

public class FrequencySensorDataIndexViewModel
{
    public int SensorId { get; set; }
    public DateTime Date { get; set; }
    public IEnumerable<FrequencySensorDataViewModel> SensorsFrequencyDataItems { get; set; }
}