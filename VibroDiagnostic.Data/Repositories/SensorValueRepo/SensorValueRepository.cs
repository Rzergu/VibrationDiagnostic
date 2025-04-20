using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using VibroDiagnostic.Core.Entities;
using VibroDiagnostic.Core.Interfaces;
using VibroDiagnostic.Data;

namespace HimeraRadio.Data.Repositories.SensorValueRepo;

public class SensorValueRepository : IRepository
{
    private readonly object _gate = new();
    private readonly Dictionary<int, List<FrequencyDataSensorValue>> _sensorDataToSave = new();
    private readonly IDocumentRepository _documentRepository;
    private DateTime _lastUpdate = DateTime.UtcNow;
    private readonly IServiceScopeFactory _scopeFactory;

    public SensorValueRepository(IDocumentRepository documentRepository, IServiceScopeFactory scopeFactory)
    {
        _documentRepository = documentRepository;
        _scopeFactory = scopeFactory;
    }

    public string Name { get; } = nameof(SensorValueRepository);
    public async Task<int> SaveToDatabase()
    {
        if (DateTime.UtcNow - _lastUpdate < TimeSpan.FromMinutes(1))
        {
            return 0;
        }
        Dictionary<int, List<FrequencyDataSensorValue>> list = new();
        
        lock (_gate)
        {
            list =_sensorDataToSave;
        }

        foreach (var sensorData in list)
        {
            var fileName = $"{sensorData.Key}_{_lastUpdate:yyyyMMddHHmm}-{DateTime.UtcNow:yyyyMMddHHmm}";
            var jsonData = JsonConvert.SerializeObject(sensorData.Value);
            var fileID = await  _documentRepository.UploadFileFromStreamAsync(fileName, new UTF8Encoding().GetBytes(jsonData));
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IAsyncRepository<SensorFile>>();
            var res = await db.AddAsync(new SensorFile()
            {
                Id = 0,
                Name = fileName,
                FileName = $"{fileName}.json",
                SensorId = sensorData.Key,
                IsLatest = true,
                Path = fileID
            });
        }
        _lastUpdate = DateTime.UtcNow;
        lock (_gate)
        {
            _sensorDataToSave.Clear();
        }
        return 0;
    }
    
    public async Task Add(FrequencyDataSensorValue value)
    {
        System.Collections.Generic.List<FrequencyDataSensorValue> sensorData;
        lock (_gate)
        {
            if (!_sensorDataToSave.TryGetValue(value.SensorId, out sensorData))
            {
                sensorData = new List<FrequencyDataSensorValue>();
                sensorData.Add(value);
                _sensorDataToSave[value.SensorId] = sensorData;
            }
            else
            {
                _sensorDataToSave[value.SensorId].Add(value);
            }
        }
    }

    public void Stop()
    {
    }
}