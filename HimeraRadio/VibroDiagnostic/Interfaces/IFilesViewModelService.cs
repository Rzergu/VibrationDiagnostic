using VibroDiagnostic.ViewModels.Files;

namespace VibroDiagnostic.Interfaces
{
    public interface IFilesViewModelService
    {
        Task<SensorFileViewModelIndex> GetAllFilesAsync();
        Task<SensorFileViewModel> SaveFileAsync(SensorFileViewModel SensorFileViewModel, byte[] fileContent);
        Task<SensorFileViewModel> UpsertFileAsync(SensorFileViewModel SensorFileViewModel);
        ValueTask<byte[]> DownloadFileById(int id);

        Task<SensorFileViewModel> GetFileByIdAsync(int id);
        
        Task<SensorFileViewModel> GetFileByNameAsync(string name);
        
        Task DeleteFile(int id);

    }
}