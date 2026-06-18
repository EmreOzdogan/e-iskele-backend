# ============================================================
# Stage 1: Build
# Official .NET 9 SDK — compile and publish the application
# ============================================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Copy solution file
COPY EIskele.sln .

# Copy all csproj files first — Docker layer caching trick:
# restore only re-runs when .csproj files change, not source code
COPY src/EIskele.Shared/EIskele.Shared.csproj           src/EIskele.Shared/
COPY src/EIskele.Domain/EIskele.Domain.csproj           src/EIskele.Domain/
COPY src/EIskele.Application/EIskele.Application.csproj src/EIskele.Application/
COPY src/EIskele.Persistence/EIskele.Persistence.csproj src/EIskele.Persistence/
COPY src/EIskele.Infrastructure/EIskele.Infrastructure.csproj src/EIskele.Infrastructure/
COPY src/EIskele.Api/EIskele.Api.csproj                 src/EIskele.Api/

# Restore NuGet packages
RUN dotnet restore src/EIskele.Api/EIskele.Api.csproj

# Copy the rest of the source code
COPY src/ src/

# Publish in Release mode — output to /app/publish
RUN dotnet publish src/EIskele.Api/EIskele.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ============================================================
# Stage 2: Runtime
# Minimal ASP.NET Core 9 runtime image — no SDK, much smaller
# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Install wget for HEALTHCHECK
RUN apt-get update && apt-get install -y wget && rm -rf /var/lib/apt/lists/*

# Create a non-root user for security
RUN addgroup --system --gid 1001 appgroup && \
    adduser --system --uid 1001 --ingroup appgroup --no-create-home appuser

# Copy published output from build stage
COPY --from=build /app/publish .

# Create directory for local file uploads (if using local storage)
RUN mkdir -p /app/uploads && chown appuser:appgroup /app/uploads

# Switch to non-root user
USER appuser

# ASP.NET Core listens on port 8080 by default in .NET 8+
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Health check — Dokploy / container orchestrators can use this
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD wget -qO- http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "EIskele.Api.dll"]
