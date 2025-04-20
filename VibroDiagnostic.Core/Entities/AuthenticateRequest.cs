using System.ComponentModel;

namespace VibroDiagnostic.Core.Entities;

public class AuthenticateRequest
{
    [DefaultValue("System")]
    public required string Username { get; set; }

    [DefaultValue("System")]
    public required string Password { get; set; }
}