using Microsoft.AspNetCore.Mvc;
using VibroDiagnostic.Attributes;
using VibroDiagnostic.Interfaces;
using VibroDiagnostic.ViewModels.Equipment;

namespace VibroDiagnostic.Controllers;

[ApiController]
[Microsoft.AspNetCore.Mvc.Route("[controller]")]
public class EquipmentController : ControllerBase
{
    private readonly ILogger<EquipmentController> _logger;
    private readonly IEquipmentViewModelService _equipmentService;
    private readonly ISensorsViewModelService _SensorsService;

    public EquipmentController(ILogger<EquipmentController> logger, 
        IEquipmentViewModelService equipmentService,
        ISensorsViewModelService SensorsService)
    {
        _logger = logger;
        _equipmentService = equipmentService;
        _SensorsService = SensorsService;
    }
    
    [Microsoft.AspNetCore.Mvc.HttpGet]
    public async Task<IActionResult> GetEquipmentsAsync()
    {
        _logger.LogTrace("GetEquipmentsAsync");
        var files = await _equipmentService.GetAllEquipmentsAsync();
        return Ok(files.EquipmentIndex);
    }
    [Microsoft.AspNetCore.Mvc.HttpGet("{id:int}")]
    public async Task<IActionResult> GetEquipmentById(int id)
    {
        _logger.LogTrace("GetEquipmentById");
        var equipment = await _equipmentService.GetEquipmentByIdAsync(id);
        return Ok(equipment);
    }
    [Microsoft.AspNetCore.Mvc.HttpGet("latest/{id:int}")]
    public async Task<IActionResult> GetLatestSensorVersion(int id)
    {
        _logger.LogTrace("GetLatestVersion");
        var Sensor = await _SensorsService.GetLatestVersionByEquipmentId(id);
        return Ok(Sensor);
    }
    [Microsoft.AspNetCore.Mvc.HttpPost]
    public async Task<IActionResult> UpsertEquipment([FromForm] EquipmentViewModel equipment)
    {
        _logger.LogTrace("UpsertEquipment");
        var res = await _equipmentService.UpsertAsync(equipment);
        return Ok(res);
    }
    
}