@echo off
title Building

dotnet --info >nul 2>&1
if %errorlevel% neq 0 (
    echo .NET SDK not found. Installing via winget...
    winget install --id Microsoft.DotNet.SDK.8 --silent --accept-source-agreements --accept-eula
    if %errorlevel% neq 0 (
        echo Failed to install .NET SDK. Please install it manually and try again.
        pause
        exit /b
    )
)

:: build
echo .NET SDK is installed. Proceeding to publish the project...
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true peredoz.sln

pause
exit