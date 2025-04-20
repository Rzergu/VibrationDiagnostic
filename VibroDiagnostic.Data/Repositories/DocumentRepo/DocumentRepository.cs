using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using VibroDiagnostic.Core.Interfaces;

namespace VibroDiagnostic.Data.Repositories.DocumentRepo;

public class DocumentRepository : IDocumentRepository
{
    private MongoClient client;
    private IMongoDatabase database;
    public DocumentRepository()
    {  
        client = new MongoClient("mongodb+srv://lieznovskiy:hY5C8pU6aFFFYe9R@cluster0.3nevvxb.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0");
        database = client.GetDatabase("Himera");
    }

    public async Task DownloadFileToStreamAsync(string id, Stream stream)
    {
        IGridFSBucket gridFS = new GridFSBucket(database);
        await gridFS.DownloadToStreamAsync(new ObjectId(id), stream);
    }

    public async Task<string> UploadFileFromStreamAsync(string fileName, byte[] content)
    {
        IGridFSBucket gridFS = new GridFSBucket(database);
        using (var stream = new MemoryStream(content))
        {
            var objId = await gridFS.UploadFromStreamAsync(fileName, stream);
            return objId.ToString();
        }
    }
}