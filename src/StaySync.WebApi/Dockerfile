# --- Build stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
WORKDIR /src

# 1) Copy project files first (for better caching)
COPY src/StaySync.Domain/StaySync.Domain.csproj src/StaySync.Domain/
COPY src/StaySync.Application/StaySync.Application.csproj src/StaySync.Application/
COPY src/StaySync.Infrastructure/StaySync.Infrastructure.csproj src/StaySync.Infrastructure/
COPY src/StaySync.WebApi/StaySync.WebApi.csproj src/StaySync.WebApi/

# 2) Restore only the WebApi project (brings transitive deps)
RUN dotnet restore src/StaySync.WebApi/StaySync.WebApi.csproj

# 3) Copy the rest of the source
COPY src/StaySync.Domain/ src/StaySync.Domain/
COPY src/StaySync.Application/ src/StaySync.Application/
COPY src/StaySync.Infrastructure/ src/StaySync.Infrastructure/
COPY src/StaySync.WebApi/ src/StaySync.WebApi/

# 4) Publish the WebApi
RUN dotnet publish src/StaySync.WebApi/StaySync.WebApi.csproj -c Release -o /app/publish \
    -p:PublishReadyToRun=true -p:PublishSingleFile=false -p:PublishTrimmed=false

# --- Runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080 \
    DOTNET_EnableDiagnostics=0 \
    TZ=Etc/UTC
COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "StaySync.WebApi.dll"]
