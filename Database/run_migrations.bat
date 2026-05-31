@echo off
REM Auto-run PostgreSQL migrations for Thuegi v2
REM No manual interaction needed

echo ========================================
echo Thuegi v2 - Database Migration
echo ========================================
echo.

set PGPASSWORD=123456
set PGHOST=localhost
set PGUSER=postgres
set PGDATABASE=thuegi

echo [1/3] Checking database connection...
psql -U %PGUSER% -h %PGHOST% -d %PGDATABASE% -c "SELECT version();" >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Cannot connect to database. Please check PostgreSQL is running.
    pause
    exit /b 1
)
echo [OK] Connected to database: %PGDATABASE%
echo.

echo [2/3] Running migration 001_initial_schema.sql...
psql -U %PGUSER% -h %PGHOST% -d %PGDATABASE% -f "Migrations/001_initial_schema.sql" -q
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Migration 001 failed!
    pause
    exit /b 1
)
echo [OK] Migration 001 completed
echo.

echo [3/3] Running migration 002_advanced_features.sql...
psql -U %PGUSER% -h %PGHOST% -d %PGDATABASE% -f "Migrations/002_advanced_features.sql" -q
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Migration 002 failed!
    pause
    exit /b 1
)
echo [OK] Migration 002 completed
echo.

echo ========================================
echo [SUCCESS] All migrations completed!
echo ========================================
echo.
echo Verifying schema...
psql -U %PGUSER% -h %PGHOST% -d %PGDATABASE% -c "\dt"
echo.
pause
