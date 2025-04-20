namespace VibroDiagnostic;

public class ApplicationLifetimeManagerBase : IHostedService
{
    private readonly IHostApplicationLifetime _appLifetime;

    public ApplicationLifetimeManagerBase(IHostApplicationLifetime appLifetime)
    {
        this._appLifetime = appLifetime;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        this._appLifetime.ApplicationStarted.Register(new Action(this.OnStarted));
        this._appLifetime.ApplicationStopping.Register(new Action(this.OnStopping));
        this._appLifetime.ApplicationStopped.Register(new Action(this.OnStopped));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    protected virtual void OnStarted()
    {
    }

    protected virtual void OnStopping()
    {
    }

    protected virtual void OnStopped()
    {
    }
}