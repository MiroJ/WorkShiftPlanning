@echo off
echo ========================================
echo   Work Shift Planning System
echo   Frontend-Only Mode
echo ========================================
echo.

cd /d "%~dp0ClientApp"

echo Installing dependencies...
call npm install
if errorlevel 1 (
    echo Error installing dependencies!
    pause
    exit /b 1
)

echo.
echo Starting Angular development server...
echo.
echo The application will be available at:
echo   http://localhost:4200
echo.
echo Press Ctrl+C to stop the server
echo.

call npm start
