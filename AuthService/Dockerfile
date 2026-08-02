# ─────────────────────────────────────────────────────────────────────────────
# Stage 1 — Restore dependencies
# Uses the full SDK image so dotnet restore can download NuGet packages.
# ─────────────────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore
WORKDIR /src

# Copy only the project file first — Docker caches this layer as long as
# .csproj hasn't changed, so 'dotnet restore' is skipped on subsequent builds.
COPY ["AuthService/AuthService.csproj", "AuthService/"]
RUN dotnet restore "AuthService/AuthService.csproj"

# ─────────────────────────────────────────────────────────────────────────────
# Stage 2 — Build & Publish
# Copy the full source, build in Release, and publish to /app/publish.
# ─────────────────────────────────────────────────────────────────────────────
FROM restore AS publish
WORKDIR /src

COPY . .
RUN dotnet publish "AuthService/AuthService.csproj" \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

# ─────────────────────────────────────────────────────────────────────────────
# Stage 3 — Runtime (final image)
# Uses the lightweight ASP.NET runtime image — no SDK overhead in production.
# ─────────────────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

# Create a non-root user BEFORE setting WORKDIR
# aspnet:10.0 is Debian-based → use 'useradd', not 'adduser'
RUN useradd --create-home --shell /bin/bash appuser

WORKDIR /app

# Give the app user ownership of the working directory
RUN chown -R appuser:appuser /app

# Switch to non-root user for security
USER appuser

# Copy the published output from the publish stage
COPY --chown=appuser:appuser --from=publish /app/publish .

# HTTP  → 8080
# HTTPS → 8081
EXPOSE 8080
EXPOSE 8081

# Set ASP.NET Core to listen on port 8080 (HTTP) inside the container
ENV ASPNETCORE_HTTP_PORTS=8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "AuthService.dll"]

