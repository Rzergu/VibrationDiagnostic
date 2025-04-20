using VibroDiagnostic.Core.Entities;
using VibroDiagnostic.ViewModels.Files;

namespace VibroDiagnostic.ViewModels
{
    public class SensorViewModel
    {
        public int Id { get; set; }
		
        public string Title { get; set; }

        public DateTime Date { get; set; }
        
        public int EquipmentId { get; set; }
        
        public List<SensorFileViewModel> Files { get; set; } = new List<SensorFileViewModel>();
        
        public bool IsLatest { get; set; }

    }
}
