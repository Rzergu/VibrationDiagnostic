namespace VibroDiagnostic.Core.Interfaces;

public interface IRepository
{
    string Name { get; }
    Task<int> SaveToDatabase();
    void Stop();
}