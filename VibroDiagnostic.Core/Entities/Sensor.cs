using System;
namespace VibroDiagnostic.Core.Entities
{
	public class Sensor
	{
		public int Id { get; set; }
		
		public string Title { get; set; }

		public DateTime Date { get; set; }

		public List<SensorFile> Files { get; set; }
		
		public Equipment Equipment { get; set; }
		public int EquipmentId { get; set; }

	}
}

