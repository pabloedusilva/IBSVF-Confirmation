@echo off
echo ================================
echo   IBSVF Family Day - Build Script
echo ================================
echo.

echo [1/4] Limpando build anterior...
if exist bin rmdir /s /q bin
if exist obj rmdir /s /q obj

echo [2/4] Restaurando dependencias...
dotnet restore

echo [3/4] Compilando aplicacao...
dotnet build --configuration Release

echo [4/4] Testando aplicacao localmente...
echo.
echo Para testar localmente, execute:
echo   dotnet run
echo.
echo Aplicacao estara disponivel em: http://localhost:5000
echo.
echo ================================
echo   Build concluido com sucesso!
echo ================================
pause
