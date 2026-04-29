# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Sai.DealAssistant.WebApi/Sai.DealAssistant.WebApi.csproj", "Sai.DealAssistant.WebApi/"]
COPY ["Sai.DealAssistant.Application/Sai.DealAssistant.Application.csproj", "Sai.DealAssistant.Application/"]
COPY ["Sai.DealAssistant.Common/Sai.DealAssistant.Common.csproj", "Sai.DealAssistant.Common/"]
COPY ["Sai.DealAssistant.Domain/Sai.DealAssistant.Domain.csproj", "Sai.DealAssistant.Domain/"]
COPY ["Sai.DealAssistant.Infrastructure/Sai.DealAssistant.Infrastructure.csproj", "Sai.DealAssistant.Infrastructure/"]
RUN dotnet restore "./Sai.DealAssistant.WebApi/Sai.DealAssistant.WebApi.csproj"
COPY . .
WORKDIR "/src/Sai.DealAssistant.WebApi"
RUN dotnet build "./Sai.DealAssistant.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Sai.DealAssistant.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Install Playwright dependencies and PowerShell
# Register Microsoft package repository so the `powershell` package can be installed on Debian/Bookworm
RUN apt-get update && \
    apt-get install -y wget gnupg ca-certificates apt-transport-https && \
    wget -q https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O /tmp/packages-microsoft-prod.deb && \
    dpkg -i /tmp/packages-microsoft-prod.deb && \
    rm -f /tmp/packages-microsoft-prod.deb && \
    apt-get update && \
    apt-get install -y --no-install-recommends \
      libnss3 libatk-bridge2.0-0 libgtk-3-0 libxss1 libasound2 libgbm1 libxshmfence1 \
      powershell && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=publish /app/publish .

# Install Playwright browsers
RUN pwsh /app/bin/Release/net8.0/playwright.ps1 install

ENTRYPOINT ["dotnet", "Sai.DealAssistant.WebApi.dll"]