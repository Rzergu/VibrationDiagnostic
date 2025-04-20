using System.Reflection;
using AutoMapper;
using HimeraRadio.Data.Repositories.SensorValueRepo;
using HimeraRadio.SignalRHub;
using VibroDiagnostic.Core;
using VibroDiagnostic.Core.Entities;
using VibroDiagnostic.Core.Interfaces;
using VibroDiagnostic.Core.Services;
using VibroDiagnostic.Data;
using VibroDiagnostic.Data.Contexts;
using VibroDiagnostic.Data.Repositories;
using VibroDiagnostic.Data.Repositories.DocumentRepo;
using VibroDiagnostic.Data.Services;
using VibroDiagnostic.Helpers;
using VibroDiagnostic.Interfaces;
using VibroDiagnostic.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.AspNetCore;
using Quartz.Impl.AdoJobStore;
using VibroDiagnostic;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddDbContext<SensorContext>(c =>
            c.UseNpgsql("Server=db;Port=5432;Database=postgres;User Id=postgres;Password=MyPassword123;", option =>
            {
                option.MigrationsAssembly("VibroDiagnostic");
            }));
        builder.Services.AddQuartz( q => {
            q.UseMicrosoftDependencyInjectionJobFactory();
        });
        builder.Services.AddSignalR();
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        builder.Services.AddAutoMapper(Assembly.Load("VibroDiagnostic.Core"));
        builder.Services.AddAutoMapper(Assembly.Load("VibroDiagnostic.Data"));
        builder.Services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>));
        builder.Services.AddScoped<IFilesViewModelService,FilesViewModelService>();
        builder.Services.AddScoped<IEquipmentViewModelService,EquipmentViewModelService>();
        builder.Services.AddSingleton<IDocumentRepository,DocumentRepository>();
        builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddSingleton<IEncryptService,EncryptService>();
        builder.Services.AddSingleton<RepositoryManager>();
        builder.Services.AddSingleton<SensorValueRepository>();
        builder.Services.AddSingleton<IRepository, SensorValueRepository>(provider => provider.GetService<SensorValueRepository>()!);
        builder.Services.AddHostedService<ApplicationLifetimeManager>();
        builder.Services.AddScoped<ISensorsViewModelService, SensorsViewModelService>();
            
        builder.Services.AddCors(o => o.AddPolicy("NUXT", builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        }));
        builder.Services.AddQuartzServer(opt =>
        {
            opt.WaitForJobsToComplete = true;
        });
        builder.Services.AddCors(c =>
        {
            c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin());
        });
        var app = builder.Build();
        app.UseStaticFiles();
        if (builder.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseHsts();
        }
        else
        {
            app.UseHsts();
        }
        app.UseHttpsRedirection();
        //app.UseMiddleware<JwtMiddleware>();
        app.UseCors("NUXT");
        app.UseAuthorization();
        app.MapControllers();
        app.MapHub<ChartHub>("/sensorData");
        app.UseCors(options => options.AllowAnyOrigin());
        Task.Delay(10000).Wait();

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<SensorContext>();
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
        }
        app.Run();
    }
}