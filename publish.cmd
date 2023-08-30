@echo off
set VER=1.1.4
dotnet test -c Release -f net6.0
if errorlevel 1 goto :error
pushd Universe.ChromeAndDriverInstaller
rd /q /s bin\Release
msbuild /t:Restore,Rebuild /v:m /p:Configuration=Release /p:PackageVersion=%VER% /p:Version=%VER%
popd

:error
echo ERROR
exit 0

:exit
exit 0