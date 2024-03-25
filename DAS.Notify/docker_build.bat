@echo off
cd /d %~dp0
dotnet publish -c Release -r alpine-x64 --self-contained true /p:PublishTrimmed=true -o ./bin/Release/netcoreapp3.1/publish
docker build -t das.notify -f Dockerfile .
pause