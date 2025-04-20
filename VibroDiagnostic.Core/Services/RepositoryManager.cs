using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using VibroDiagnostic.Core.Interfaces;

namespace VibroDiagnostic.Core.Services;

    public class RepositoryManager
    {
        private readonly IRepository[] _repositories;
        
        public bool IsActive { get; private set; }

        private List<TapRepoManagerItem> _managedItems = new();

        public RepositoryManager(IServiceScopeFactory scopeFactory)
        {

            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IRepository>();
            _repositories = new []{ db };

            foreach (var repository in _repositories)
            {
                _managedItems.Add(new TapRepoManagerItem(repository));
            }
        }

        public void Start()
        {
            var repos = "";
            foreach (var item in _managedItems)
            {
                item.Start();
                repos = $"{repos}{item}; ";
            }
            
            IsActive = true;
            
        }

        public void Stop()
        {
            IsActive = false;

            foreach (var repo in _repositories)
            {
                repo.Stop();
            }
            
            Thread.Sleep(10_000);
            
            
            foreach (var item in _managedItems)
            {
                item.Stop();
            }
            
        }
        
        
        public class TapRepoManagerItem
        {
            private const int TimeBetweenSaveInSec = 25;
            
            private readonly IRepository _repository;
            private readonly MyTaskTimer _timer;
            private bool _workInProcess;

            public TapRepoManagerItem(IRepository repository)
            {
                _repository = repository;
                _timer = new MyTaskTimer(nameof(RepositoryManager), TimeSpan.FromMinutes(TimeBetweenSaveInSec), DoTime);
            }

            public void Start()
            {
                _timer.Start();
            }

            public void Stop()
            {
                _repository.Stop();
                

                var index = 0;
                while (_workInProcess && index < 200)
                {
                    index++;
                    Thread.Sleep(1_000);
                }
                
                _timer.Stop();
            }
            
            private async Task DoTime()
            {
                _workInProcess = true;
                
                try
                {
                    var repositorySw = Stopwatch.StartNew();
                    var count = await _repository.SaveToDatabase();
                    repositorySw.Stop();

                }
                catch (Exception ex)
                {
                }
                finally
                {
                    _workInProcess = false;
                }
            }
        }
    }