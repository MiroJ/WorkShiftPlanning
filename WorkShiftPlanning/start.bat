@echo off
echo ========================================
echo   Work Shift Planning System
echo   Starting Application...
echo ========================================
echo.

cd /d "%~dp0"

echo Building Angular application...
cd ClientApp
call npm run build
if errorlevel 1 (
    echo Error building Angular application!
    pause
    exit /b 1
)

cd ..
echo.
echo Starting ASP.NET Core backend...
echo.
echo The application will be available at:
echo   http://localhost:5000
echo.
echo Press Ctrl+C to stop the server
echo.

dotnet run
