# ---------- build ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Сначала только csproj — слой с restore переиспользуется, пока не изменились зависимости.
COPY MetricsService.Api/MetricsService.Api.csproj MetricsService.Api/
COPY MetricsService.Application/MetricsService.Application.csproj MetricsService.Application/
COPY MetricsService.Domain/MetricsService.Domain.csproj MetricsService.Domain/
COPY MetricsService.Repository/MetricsService.Repository.csproj MetricsService.Repository/
RUN dotnet restore MetricsService.Api/MetricsService.Api.csproj

COPY MetricsService.Api/ MetricsService.Api/
COPY MetricsService.Application/ MetricsService.Application/
COPY MetricsService.Domain/ MetricsService.Domain/
COPY MetricsService.Repository/ MetricsService.Repository/
RUN dotnet publish MetricsService.Api/MetricsService.Api.csproj -c Release -o /app --no-restore

# ---------- runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app .

EXPOSE 8080
ENTRYPOINT ["dotnet", "MetricsService.Api.dll"]
