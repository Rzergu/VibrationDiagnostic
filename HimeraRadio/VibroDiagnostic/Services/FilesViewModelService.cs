using VibroDiagnostic.ViewModels;
using VibroDiagnostic.Core.Entities;
using VibroDiagnostic.Core.Interfaces;
using VibroDiagnostic.Core.Specifications;
using VibroDiagnostic.Data;
using VibroDiagnostic.Interfaces;
using VibroDiagnostic.ViewModels.Files;

namespace VibroDiagnostic.Services;

public class FilesViewModelService : IFilesViewModelService
{
    private readonly IAsyncRepository<SensorFile> _filesRepository;
    private readonly ILogger<FilesViewModelService> _logger;
    private readonly IDocumentRepository _documentRepository;
    public FilesViewModelService(IAsyncRepository<SensorFile> filesRepository, IDocumentRepository documentRepository, ILogger<FilesViewModelService> logger)
    {
        _filesRepository = filesRepository;
        _logger = logger;
        _documentRepository = documentRepository;
    }

    public async Task<SensorFileViewModelIndex> GetAllFilesAsync()
    {
        _logger.LogInformation("GetAllFilesAsync called.");
        var filesItem = await _filesRepository.ListAllAsync();
        var vm = new SensorFileViewModelIndex()
        {
            FilesItem = filesItem.Select(x => new SensorFileViewModel()
            {
                Id = x.Id,
                Name = x.Name, 
                SensorId = x.SensorId,
                FileName = x.FileName,
                IsLatest = x.IsLatest,
            })
        };
        return vm;
    }

    public async Task<SensorFileViewModel> SaveFileAsync(SensorFileViewModel SensorFileViewModel,  byte[] fileContent) 
    {
        _logger.LogInformation("SaveFileAsync called.");

       var fileID = await  _documentRepository.UploadFileFromStreamAsync(SensorFileViewModel.FileName, fileContent);
       if (SensorFileViewModel.IsLatest)
       {
           await _filesRepository.UpdateBySpecificationAsync(new AllLatestFilesForSensorSpecification(SensorFileViewModel.SensorId),
               e => e.SetProperty(x => x.IsLatest, false));
       }
       var res = await _filesRepository.AddAsync(new SensorFile()
       {
            Id = 0,
            Name = SensorFileViewModel.Name,
            FileName = SensorFileViewModel.FileName,
            SensorId = SensorFileViewModel.SensorId,
            IsLatest = SensorFileViewModel.IsLatest,
            Path = fileID
       });
       return new SensorFileViewModel()
       {
           Id = res.Id,
           Name = res.Name,
           FileName = res.FileName,
           SensorId = res.SensorId,
           IsLatest = res.IsLatest,
       };
    }

    public async Task<SensorFileViewModel> UpsertFileAsync(SensorFileViewModel SensorFileViewModel)
    {
        _logger.LogInformation("UpsertFileAsync called.");
        if (SensorFileViewModel.IsLatest)
        {
            await _filesRepository.UpdateBySpecificationAsync(new AllLatestFilesForSensorSpecification(SensorFileViewModel.SensorId),
                e => e.SetProperty(x => x.IsLatest, false));
        }

        var Sensor = await _filesRepository.GetByIdAsync(SensorFileViewModel.Id);
        Sensor.Name = SensorFileViewModel.Name;
        Sensor.IsLatest = SensorFileViewModel.IsLatest;
        await _filesRepository.UpdateAsync(Sensor);
        return SensorFileViewModel;
    }

    public async ValueTask<byte[]> DownloadFileById(int id)
    {
        _logger.LogInformation("DownloadFileById called.");
        var SensorFile = await _filesRepository.GetByIdAsync(id);
        await using var stream = new MemoryStream();
        await  _documentRepository.DownloadFileToStreamAsync(SensorFile.Path, stream);
        return stream.GetBuffer();
    }

    public async Task<SensorFileViewModel> GetFileByIdAsync(int id)
    {
        var file = await _filesRepository.GetByIdAsync(id);
        return new SensorFileViewModel()
        {
            Id = file.Id,
            Name = file.Name,
            FileName = file.FileName,
            SensorId = file.SensorId,
            IsLatest = file.IsLatest
        };
    }
    
    public async Task<SensorFileViewModel> GetFileByNameAsync(string name)
    {
        var file = await _filesRepository.GetFirstBySpecificationAsync(new GetFileEntityByFileNameSpecification(name));
        return new SensorFileViewModel()
        {
            Id = file.Id,
            Name = file.Name,
            FileName = file.FileName,
            SensorId = file.SensorId,
            IsLatest = file.IsLatest
        };
    }
    public async Task DeleteFile(int id)
    {
        var file = await _filesRepository.GetByIdAsync(id);
        await _filesRepository.DeleteAsync(file);
    }
}