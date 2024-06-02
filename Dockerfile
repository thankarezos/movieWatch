FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /App

ENTRYPOINT ["dotnet", "MetroClimate.Api.dll"]