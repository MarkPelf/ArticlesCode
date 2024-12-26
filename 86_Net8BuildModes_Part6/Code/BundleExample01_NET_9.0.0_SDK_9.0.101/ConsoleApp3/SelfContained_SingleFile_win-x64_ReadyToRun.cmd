echo .NET SDK version:
dotnet --version
dotnet publish ConsoleApp3.csproj --nologo --no-restore --runtime win-x64 --configuration Release   -p:PublishSingleFile=true -p:SelfContained=true -p:PublishTrimmed=false -p:PublishReadyToRun=true   --output ./SelfContained_SingleFile_win-x64_ReadyToRun
