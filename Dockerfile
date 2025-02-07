# FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
# WORKDIR /app
# EXPOSE 80
# EXPOSE 443

# FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
# WORKDIR /src
# COPY ["src/InvoiceApp.API/InvoiceApp.API.csproj", "src/InvoiceApp.API/"]
# COPY ["src/InvoiceApp.Application/InvoiceApp.Application.csproj", "src/InvoiceApp.Application/"]
# COPY ["src/InvoiceApp.Infrastructure/InvoiceApp.Infrastructure.csproj", "src/InvoiceApp.Infrastructure/"]
# COPY ["src/InvoiceApp.Domain/InvoiceApp.Domain.csproj", "src/InvoiceApp.Domain/"]
# RUN dotnet restore "src/InvoiceApp.API/InvoiceApp.API.csproj"
# COPY . .
# RUN dotnet build "src/InvoiceApp.API/InvoiceApp.API.csproj" -c Release -o /app/build

# FROM build AS publish
# RUN dotnet publish "src/InvoiceApp.API/InvoiceApp.API.csproj" -c Release -o /app/publish

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "InvoiceApp.API.dll"]

# Stage 1: Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Environment configuration
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80;https://+:443
ENV DOTNET_NOLOGO=true
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

# Stage 2: Build environment
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["InvoiceApp.sln", "."]
COPY ["src/InvoiceApp.API/InvoiceApp.API.csproj", "src/InvoiceApp.API/"]
COPY ["src/InvoiceApp.Application/InvoiceApp.Application.csproj", "src/InvoiceApp.Application/"]
COPY ["src/InvoiceApp.Infrastructure/InvoiceApp.Infrastructure.csproj", "src/InvoiceApp.Infrastructure/"]
COPY ["src/InvoiceApp.Domain/InvoiceApp.Domain.csproj", "src/InvoiceApp.Domain/"]

# Restore dependencies
RUN dotnet restore "InvoiceApp.sln"

# Copy all source code
COPY . .

# Build the API project
WORKDIR "/src/src/InvoiceApp.API"
RUN dotnet build "InvoiceApp.API.csproj" -c Release -o /app/build

# Stage 3: Publish
FROM build AS publish
RUN dotnet publish "InvoiceApp.API.csproj" -c Release -o /app/publish \
    --no-restore \
    --no-build \
    -p:ContinuousIntegrationBuild=true

# Final stage
FROM base AS final
WORKDIR /app

# Install required system dependencies
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        curl \
    && rm -rf /var/lib/apt/lists/*

# Copy published files
COPY --from=publish /app/publish .

# Health check verification
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD curl --fail http://localhost/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "InvoiceApp.API.dll"]