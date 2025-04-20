using HimeraRadio.SignalRHub;
using VibroDiagnostic.Core.Entities;
using VibroDiagnostic.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using VibroDiagnostic.Attributes;
using VibroDiagnostic.Interfaces;
using VibroDiagnostic.ViewModels;
using VibroDiagnostic.ViewModels.FrequencySensorData;

namespace VibroDiagnostic.Controllers;

[ApiController]
[Microsoft.AspNetCore.Mvc.Route("[controller]")]
public class SensorsController : ControllerBase
{
    private readonly ILogger<SensorsController> _logger;
    private readonly ISensorsViewModelService _sensorsService;
    private IHubContext<ChartHub> _hub;

    public SensorsController(ILogger<SensorsController> logger, 
        ISensorsViewModelService SensorsService,
        IHubContext<ChartHub> hub)
    {
        _logger = logger;
        _sensorsService = SensorsService;
        _hub = hub;
    }
    [Microsoft.AspNetCore.Mvc.HttpGet]
    public async Task<IActionResult> GetSensorsAsync()
    {
        _logger.LogTrace("GetSensorsAsync");
        var Sensors = await _sensorsService.GetAllSensorsAsync();
        return Ok(Sensors.SensorsItem);
    }
    [Microsoft.AspNetCore.Mvc.HttpGet("{id:int}")]
    public async Task<IActionResult> GetSensorByIdAsync(int id)
    {
        _logger.LogTrace("GetSensorByIdAsync");
        var Sensor = await _sensorsService.GetSensorByIdAsync(id);
        return Ok(Sensor);
    }
    [Microsoft.AspNetCore.Mvc.HttpPost]
    public async Task<IActionResult> UpsertSensor([FromForm] SensorViewModel sensor)
    {
        _logger.LogTrace("UpsertSensor");
        var res = await _sensorsService.UpsertAsync(sensor);
        return Ok(res);
    }
    [Microsoft.AspNetCore.Mvc.HttpPost("delete")]
    public async Task<IActionResult> DeleteSensor([Microsoft.AspNetCore.Mvc.FromBody] int sensorId)
    {
        _logger.LogTrace("DeleteSensor");
        var res = await _sensorsService.DeleteSensorAsync(sensorId);
        return Ok(res);
    }
    [Microsoft.AspNetCore.Mvc.HttpPost("sensorData/add")]
    public async Task<IActionResult> AddSensorData([FromBody]FrequencySensorDataIndexViewModel sensorData)
    {
        await _sensorsService.AddSensorsValues(sensorData);
        await _hub.Clients.Groups(sensorData.SensorId.ToString()).SendAsync("transferfreqdata", sensorData);
        return Ok();
    }
}

