# Multi-stage Dockerfile for ASP.NET Framework 4.8 application on Windows Server 2022

# Build stage
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2022 AS build
WORKDIR /app

# Install Git
RUN powershell -Command \
    $ErrorActionPreference = 'Stop'; \
    $ProgressPreference = 'SilentlyContinue'; \
    Invoke-WebRequest -Uri 'https://github.com/git-for-windows/git/releases/download/v2.42.0.windows.2/MinGit-2.42.0.2-64-bit.zip' -OutFile 'mingit.zip'; \
    Expand-Archive -Path mingit.zip -DestinationPath C:\git; \
    Remove-Item mingit.zip -Force; \
    $env:PATH += ';C:\git\cmd'; \
    [Environment]::SetEnvironmentVariable('PATH', $env:PATH, [EnvironmentVariableTarget]::Machine); \
    git --version

# Clone the repository
# The repository URL and branch/tag can be passed as build arguments
ARG REPO_URL=https://github.com/yourusername/aspnet-event-management.git
ARG REPO_BRANCH=main
RUN git clone --depth 1 --branch %REPO_BRANCH% %REPO_URL% .

# Restore NuGet packages
RUN nuget restore EventRegistrationSystem.sln

# Build and publish the application
RUN msbuild /p:Configuration=Release /p:DeployOnBuild=true /p:PublishProfile=FolderProfile /p:PublishUrl=./publish /p:WebPublishMethod=FileSystem /p:DeleteExistingFiles=True /p:platform="Any CPU" /p:outdir="./publish" /p:OutputPath="./publish" EventRegistrationSystem.csproj

# Runtime stage
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8-windowsservercore-ltsc2022 AS runtime
WORKDIR /inetpub/wwwroot

# Copy published application from build stage
COPY --from=build /app/publish .

# Set permissions for App_Data directory
RUN IF EXIST "App_Data" (icacls "App_Data" /grant "IIS_IUSRS:(OI)(CI)F")

# Configure the DefaultAppPool to use .NET Framework 4.8
RUN powershell -Command \
    Import-Module WebAdministration; \
    Set-ItemProperty -Path 'IIS:\AppPools\DefaultAppPool' -Name 'managedRuntimeVersion' -Value 'v4.0'; \
    Set-ItemProperty -Path 'IIS:\AppPools\DefaultAppPool' -Name 'enable32BitAppOnWin64' -Value 'False';

# Expose port 80
EXPOSE 80

# Health check
HEALTHCHECK --interval=30s --timeout=10s --retries=3 CMD powershell -command \
    try { $response = Invoke-WebRequest -Uri http://localhost/ -UseBasicParsing; \
    if ($response.StatusCode -eq 200) { return 0 } else { return 1 }; } \
    catch { return 1 }

# The entry point is automatically set by the aspnet base image
