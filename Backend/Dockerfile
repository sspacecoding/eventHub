FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies (cache layer)
COPY ["SpaceEventHub.API/SpaceEventHub.API.csproj", "SpaceEventHub.API/"]
RUN dotnet restore "SpaceEventHub.API/SpaceEventHub.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/SpaceEventHub.API"
RUN dotnet build "SpaceEventHub.API.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "SpaceEventHub.API.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-restore

FROM base AS final
WORKDIR /app

# Install wget for healthcheck (before switching user)
RUN apt-get update && \
    apt-get install -y wget && \
    rm -rf /var/lib/apt/lists/*

# Create non-root user for security
RUN useradd -m -u 1000 appuser

COPY --from=publish /app/publish .
RUN chown -R appuser:appuser /app

USER appuser

ENTRYPOINT ["dotnet", "SpaceEventHub.API.dll"]
