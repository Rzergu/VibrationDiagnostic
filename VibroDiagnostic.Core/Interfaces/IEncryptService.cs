namespace VibroDiagnostic.Core.Interfaces;

public interface IEncryptService
{
    public byte[] EncryptFile(byte[] content);
}