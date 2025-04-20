namespace VibroDiagnostic.Core.Interfaces;

public interface IDocumentRepository
{
    public Task DownloadFileToStreamAsync(string id, Stream stream);
    public Task<string> UploadFileFromStreamAsync(string fileName, byte[] stream);
}