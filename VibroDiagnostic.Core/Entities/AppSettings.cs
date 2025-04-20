namespace VibroDiagnostic.Core.Entities;

public class AppSettings
{
    public string Secret { get; set; } = string.Empty;
    public string SecretEncryption { get; set; } = string.Empty;
}