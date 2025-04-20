using System.Diagnostics;

namespace VibroDiagnostic.Core.Services;

public class MyTaskTimer :IDisposable
  {
    private readonly 
    #nullable disable
    string _owner;
    private TimeSpan _interval;
    private readonly Func<Task> _doProcess;
    private readonly CancellationTokenSource _token = new CancellationTokenSource();
    private Task _process;

    public bool IsTelemetryActive { get; set; } = true;

    public MyTaskTimer(string owner, TimeSpan interval, Func<Task> doProcess)
    {
      this._owner = owner;
      this._interval = interval;
      this._doProcess = doProcess;
    }

    public MyTaskTimer(Type owner, TimeSpan interval, Func<Task> doProcess)
      : this(owner.Name, interval, doProcess)
    {
    }

    public static MyTaskTimer Create<T>(TimeSpan interval, Func<Task> doProcess)
    {
      return new MyTaskTimer(typeof (T), interval, doProcess);
    }

    public void ChangeInterval(TimeSpan interval) => this._interval = interval;

    public void Start()
    {
      this._process = Task.Run(new Func<Task>(this.DoProcessInt), this._token.Token);
    }

    private async Task DoProcessInt()
    {
      try
      {
        while (!this._token.IsCancellationRequested)
        {
            try
            {
              await this._doProcess();
            }
            catch (Exception ex)
            {
            }
          await Task.Delay(this._interval, this._token.Token);
        }
      }
      catch (Exception ex)
      {
      }
    }

    public void Stop()
    {
      this._token.Cancel();
      this._process?.Wait();
    }

    public void Dispose() => this.Stop();

    public MyTaskTimer DisableTelemetry()
    {
      this.IsTelemetryActive = false;
      return this;
    }
  }