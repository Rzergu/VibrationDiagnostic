using VibroDiagnostic.Core.Entities;
using VibroDiagnostic.ViewModels;
using VibroDiagnostic.ViewModels.FrequencySensorData;

namespace VibroDiagnostic.Interfaces
{
    public interface ISensorsViewModelService
    {
        Task<SensorViewModelIndex> GetAllSensorsAsync();
        Task<SensorViewModel> GetSensorByIdAsync(int SensorId);
        Task<SensorViewModel> GetLatestVersionByEquipmentId(int equipmentId);
        Task<SensorViewModel> UpsertAsync(SensorViewModel Sensor);
        Task<bool> DeleteSensorAsync(int SensorId);
        Task AddSensorsValues(FrequencySensorDataIndexViewModel sensorData);
    }
}
