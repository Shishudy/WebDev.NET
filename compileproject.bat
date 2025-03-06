@echo off
setlocal

:: Define MSBuild path manually
set MSBUILD="C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"

:: Define solution file paths
set SOLUTION1="1_WebAPI\WebAPI.sln"
set SOLUTION2="4_MPA\UserMPA\UserMPA.sln"

:: Define the output directories for the executables
set PROJECT1_OUTPUT_DIR=""
set PROJECT2_OUTPUT_DIR=""

:: Define project output locations
set APP1="%PROJECT1_OUTPUT_DIR%\WebAPI.exe"
set APP2="%PROJECT2_OUTPUT_DIR%\UserMPA.exe"

:: Define build configuration and platform
set CONFIGURATION=Release
set PLATFORM=Any CPU

:: Build first solution
echo Building %SOLUTION1%...
%MSBUILD% %SOLUTION1% /p:Configuration=%CONFIGURATION% /p:Platform="%PLATFORM%" /p:OutputPath=%PROJECT1_OUTPUT_DIR%
if %ERRORLEVEL% NEQ 0 (
    echo Failed to build %SOLUTION1%
    exit /b 1
)

:: Build second solution
echo Building %SOLUTION2%...
%MSBUILD% %SOLUTION2% /p:Configuration=%CONFIGURATION% /p:Platform="%PLATFORM%" /p:OutputPath=%PROJECT2_OUTPUT_DIR%
if %ERRORLEVEL% NEQ 0 (
    echo Failed to build %SOLUTION2%
    exit /b 1
)

:: Run the compiled programs
echo Running %APP1%...
start "" %APP1%

echo Running %APP2%...
start "" %APP2%

echo Build and execution completed!
exit /b 0
