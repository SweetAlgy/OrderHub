# =============================================================================
# Stage 1: Build
# =============================================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy solution file
COPY OrderHub.slnx ./

# Copy project files (separate layer for better caching of restore)
COPY src/OrderHub.Api/OrderHub.Api.csproj               src/OrderHub.Api/
COPY src/OrderHub.Application/OrderHub.Application.csproj src/OrderHub.Application/
COPY src/OrderHub.Domain/OrderHub.Domain.csproj           src/OrderHub.Domain/
COPY src/OrderHub.Infrastructure/OrderHub.Infrastructure.csproj src/OrderHub.Infrastructure/
COPY src/OrderHub.Shared/OrderHub.Shared.csproj           src/OrderHub.Shared/

# Restore only the API and its dependencies
RUN dotnet restore src/OrderHub.Api/OrderHub.Api.csproj

# Copy full source
COPY src/ src/


# Publish in Release mode
RUN dotnet publish src/OrderHub.Api/OrderHub.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# =============================================================================
# Stage 2: Runtime
# =============================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

USER $APP_UID

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "OrderHub.Api.dll"]
