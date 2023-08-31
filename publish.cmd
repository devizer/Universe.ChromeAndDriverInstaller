@echo off
set VER=1.1.11
dotnet test -c Release -f net6.0
if errorlevel 1 goto :error
pushd Universe.ChromeAndDriverInstaller
rd /q /s bin\Release
msbuild /t:Restore,Rebuild /v:m /p:Configuration=Release /p:PackageVersion=%VER% /p:Version=%VER%
popd
goto :exit

:error
echo ERROR
exit 1

:exit
