using VibroDiagnostic.Core.Entities;
using VibroDiagnostic.Core.Specifications;
using VibroDiagnostic.Data;
using VibroDiagnostic.Interfaces;
using VibroDiagnostic.ViewModels;
using VibroDiagnostic.ViewModels.Equipment;

namespace VibroDiagnostic.Services;

public class EquipmentViewModelService : IEquipmentViewModelService
{
    private readonly IAsyncRepository<Equipment> _equipmentRepository;
    private readonly ILogger<EquipmentViewModelService> _logger;

    public EquipmentViewModelService(IAsyncRepository<Equipment> equipmentRepository, ILogger<EquipmentViewModelService> logger)
    {
        _logger = logger;
        _equipmentRepository = equipmentRepository;
    }
    public async Task<EquipmentViewModelIndex> GetAllEquipmentsAsync()
    {
        _logger.LogInformation("GetAllSensorsAsync called.");
        var Sensors = await _equipmentRepository.ListAllAsync();
        var vm = new EquipmentViewModelIndex()
        {
            EquipmentIndex = Sensors.Select(x => new EquipmentViewModel()
            {
                ID = x.Id,
                Name = x.Name
            })
        };
        return vm;
    }

    public async  Task<EquipmentViewModel> GetEquipmentByIdAsync(int id)
    {   
        _logger.LogInformation("GetEquipmentByIdAsync called.");
        var Sensor = await _equipmentRepository.GetFirstBySpecificationAsync(new EquipmentWithSensorsSpecification(id));
        var vm = new EquipmentViewModel()
        {
            ID = Sensor.Id,
            Name = Sensor.Name,
            Sensors = Sensor?.Sensors.Select(x => new SensorViewModel()
            {
                Id = x.Id,
                Title = x.Title,
                Date = x.Date,
            }).ToList() ?? new List<SensorViewModel>()

        };
        return vm;
    }

    public async Task<EquipmentViewModel> UpsertAsync(EquipmentViewModel equipment)
    {
        _logger.LogInformation("UpsertAsync called.");
        var equipEntity = new Equipment()
        {
            Name = equipment.Name,
            Id = equipment.ID
            
        };
        if (equipment.ID == 0)
        {
            await _equipmentRepository.AddAsync(equipEntity);
        }
        else
        {
            await _equipmentRepository.UpdateAsync(equipEntity);
        }

        return equipment;
    }
}