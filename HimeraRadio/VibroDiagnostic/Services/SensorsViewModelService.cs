using HimeraRadio.Data.Repositories.SensorValueRepo;
using VibroDiagnostic.Core.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using MongoDB.Bson;
using Quartz;
using VibroDiagnostic.Core.Entities;
using VibroDiagnostic.Core.Specifications;
using VibroDiagnostic.Data;
using VibroDiagnostic.Interfaces;
using VibroDiagnostic.ViewModels;
using VibroDiagnostic.ViewModels.Files;
using VibroDiagnostic.ViewModels.FrequencySensorData;

namespace VibroDiagnostic.Services
{
    public class SensorsViewModelService : ISensorsViewModelService
    {
        private readonly IAsyncRepository<Sensor> _sensorsRepository;
        private readonly IAsyncRepository<SensorFile> _filesRepository;
        private readonly ILogger<SensorsViewModelService> _logger;
        private readonly SensorValueRepository _sensorValueRepository;
        public SensorsViewModelService(IAsyncRepository<Sensor> sensorsRepository,
            IAsyncRepository<SensorFile> filesRepository,
            ILogger<SensorsViewModelService> logger, 
            SensorValueRepository sensorValueRepository)
        {
            _sensorsRepository = sensorsRepository;
            _filesRepository = filesRepository;
            _logger = logger;
            _sensorValueRepository = sensorValueRepository;
        }
        
        
        public async Task<SensorViewModelIndex> GetAllSensorsAsync()
        {
            _logger.LogInformation("GetAllSensorsAsync called.");
            var Sensors = await _sensorsRepository.ListAllAsync();
            var vm = new SensorViewModelIndex()
            {
                SensorsItem = Sensors.Select(x => new SensorViewModel()
                {
                    Id = x.Id,
                    Title = x.Title,
                    Date = x.Date
                })
            };
            return vm;
        }

        public async Task<SensorViewModel> GetSensorByIdAsync(int SensorId)
        {
            _logger.LogInformation("GetSensorByIdAsync called.");
            var Sensor
                = await _sensorsRepository.GetFirstBySpecificationAsync(new SensorWithFilesSpecification(SensorId));
            return new SensorViewModel()
            {
                Id = Sensor.Id,
                Date = Sensor.Date,
                Title = Sensor.Title,
                Files = Sensor?.Files.Select(y => new SensorFileViewModel()
                {
                    Id = y.Id,
                    Name = y.Name,
                    FileName = y.FileName,
                    IsLatest = y.IsLatest
                }).ToList() ?? new List<SensorFileViewModel>()
            };                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            
        }

        public async Task<SensorViewModel> GetLatestVersionByEquipmentId(int equipmentId)
        {
            _logger.LogInformation("GetLatestVersionByEquipmentId called.");
            var files
                = await _filesRepository.ListAsync(new LatestFilesForAnEquipment(equipmentId));
            return new SensorViewModel()
            {
                Id = 0,
                Date = DateTime.UtcNow,
                Title = string.Empty,
                Files = files.Select(y => new SensorFileViewModel()
                {
                    Id = y.Id,
                    Name = y.Name,
                    SensorId = y.SensorId,
                    FileName = y.FileName,
                    IsLatest = y.IsLatest
                }).ToList()
            };
        }

        public async Task<SensorViewModel> UpsertAsync(SensorViewModel Sensor)
        {
            _logger.LogInformation("UpsertAsync called.");
            var recEntity = new Sensor()
            {
                Title = Sensor.Title,
                Date = DateTime.UtcNow,
                Id = Sensor.Id,
                EquipmentId = Sensor.EquipmentId,
            };
            if (Sensor.Id == 0)
            {
                await _sensorsRepository.AddAsync(recEntity);
            }
            else
            {
                await _sensorsRepository.UpdateAsync(recEntity);
            }


            return Sensor;
        }

        public async Task<bool> DeleteSensorAsync(int SensorId)
        {
            try
            {
                var sensor = await _sensorsRepository.GetByIdAsync(SensorId);
                await _sensorsRepository.DeleteAsync(sensor);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Cannot delete Sensor. {SensorId}");
                return false;
            }
        }

        public async Task AddSensorsValues(FrequencySensorDataIndexViewModel sensorData)
        {
            _logger.LogInformation("AddSensorsValues called.");
            try
            {
                var freqData = new FrequencyDataSensorValue()
                {
                    SensorId = sensorData.SensorId,
                    Date = sensorData.Date,
                    SensorsFrequencyDataPoints = sensorData.SensorsFrequencyDataItems.Select(x =>
                        new FrequencyDataSensorValuePoint()
                        {
                            Frequency = x.Frequency,
                            Power = x.Power
                        })
                };
                await _sensorValueRepository.Add(freqData);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Cannot delete add sensorData. {sensorData.ToJson()}");
            }
        }
    }
}
