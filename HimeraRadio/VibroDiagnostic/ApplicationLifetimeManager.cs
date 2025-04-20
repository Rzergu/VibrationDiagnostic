using VibroDiagnostic.Core.Services;

namespace VibroDiagnostic;

public class ApplicationLifetimeManager : ApplicationLifetimeManagerBase
{
    private readonly ILogger<ApplicationLifetimeManager> _logger;
    private readonly RepositoryManager _tapRepositoryManager;
    
    public ApplicationLifetimeManager(IHostApplicationLifetime appLifetime,
        ILogger<ApplicationLifetimeManager> logger, 
        RepositoryManager tapRepositoryManager)
        : base(appLifetime)
    {
        _logger = logger;
        _tapRepositoryManager = tapRepositoryManager;

    }

    protected override void OnStarted()
    {
        _logger.LogInformation("OnStarted has been called.");
        _tapRepositoryManager.Start();
    }

    protected override void OnStopping()
    {
        _logger.LogInformation("OnStopping has been called.");
        _tapRepositoryManager.Stop();
    }

    protected override void OnStopped()
    {
        _logger.LogInformation("OnStopped has been called.");
    }
}