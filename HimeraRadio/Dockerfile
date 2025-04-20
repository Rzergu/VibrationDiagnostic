FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
EXPOSE 80
EXPOSE 443
RUN mkdir -p app
WORKDIR /VibroDiagnostic/bin/Release/net8.0/
COPY VibroDiagnostic/bin/Release/net8.0/ app/
ENTRYPOINT ["dotnet", "app/HimeraRadio.dll"]
