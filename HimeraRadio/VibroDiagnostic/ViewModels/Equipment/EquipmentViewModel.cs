namespace VibroDiagnostic.ViewModels.Equipment;

public class EquipmentViewModel
{
    public int ID { get; set; }
    public string Name { get; set; }
    public List<SensorViewModel> Sensors { get; set; } = new List<SensorViewModel>();
}