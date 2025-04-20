using VibroDiagnostic.Core.Entities;

namespace VibroDiagnostic.ViewModels.Files;

public class SensorFileViewModel
{
    public int Id { get; set; }
	
    public string Name { get; set; }
    public string? FileName { get; set; }

    public IFormFile? File { get; set; } = null;
    
    public int SensorId { get; set; }
    
    public bool IsLatest { get; set; }

}