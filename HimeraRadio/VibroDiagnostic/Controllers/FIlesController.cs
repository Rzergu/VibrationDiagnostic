using VibroDiagnostic.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using VibroDiagnostic.Attributes;
using VibroDiagnostic.Core.Interfaces;
using VibroDiagnostic.Interfaces;
using VibroDiagnostic.ViewModels.Files;

namespace VibroDiagnostic.Controllers;

[ApiController]
[Microsoft.AspNetCore.Mvc.Route("[controller]")]
public class FilesController : ControllerBase
{

    private readonly ILogger<FilesController> _logger;
    private readonly IFilesViewModelService _filesService;
    private readonly IEncryptService _encryptService;

    public FilesController(
        ILogger<FilesController> logger, 
        IFilesViewModelService filesService, 
        IEncryptService encryptService)
    {
        _logger = logger;
        _filesService = filesService;
        _encryptService = encryptService;
    }
    [Microsoft.AspNetCore.Mvc.HttpGet]
    public async Task<IActionResult> GetFilesAsync()
    {
        _logger.LogTrace("GetFilesAsync");
        var files = await _filesService.GetAllFilesAsync();
        return Ok(files.FilesItem);
    }
    [Microsoft.AspNetCore.Mvc.HttpPost]
    public async Task<IActionResult> AddSensorFile([FromForm] SensorFileViewModel SensorFile)
    {
        _logger.LogTrace("AddSensorFile");
        await using var ms = new MemoryStream((int)SensorFile.File.Length);
        await SensorFile.File.CopyToAsync(ms);
        SensorFile.FileName = SensorFile.File.FileName;
        var viewModel = await _filesService.SaveFileAsync(SensorFile, ms.GetBuffer());
        return Ok(viewModel);
    }
    
    [Microsoft.AspNetCore.Mvc.HttpPost("edit/{id:int}")]
    public async Task<IActionResult> EditSensorFile([FromForm] SensorFileViewModel SensorFile, int id)
    {
        _logger.LogTrace("Edit Sensor File");
        SensorFile.Id = id;
        var viewModel = await _filesService.UpsertFileAsync(SensorFile);
        return Ok(viewModel);
    }
    [Microsoft.AspNetCore.Mvc.HttpGet("{id:int}")]
    public async Task<IActionResult> DownloadFileById(int id)
    {
        _logger.LogTrace("DownloadFileById");
        var SensorFile = await _filesService.GetFileByIdAsync(id);
        var file = await _filesService.DownloadFileById(id);
        var resFile = _encryptService.EncryptFile(file);
        var res = File(resFile, "application/octet-stream", fileDownloadName: SensorFile.FileName);
        return res;
    }
    [Microsoft.AspNetCore.Mvc.HttpGet("fileEntity/{name}")]
    public async Task<IActionResult> GetFileName(string name)
    {
        _logger.LogTrace("GetFileById");
        var SensorFile = await _filesService.GetFileByNameAsync(name);
        return Ok(SensorFile);
    }
    [Microsoft.AspNetCore.Mvc.HttpPost("delete/{id:int}")]
    public async Task<IActionResult> DeleteFile(int id)
    {
        _logger.LogTrace("DeleteFile");
        await _filesService.DeleteFile(id);
        return Ok();
    }
}