echo .NET SDK version:
dotnet --version
dotnet publish ConsoleApp2C.csproj --nologo --no-restore --runtime win-x64 --configuration Release   -p:PublishSingleFile=true -p:SelfContained=true -p:PublishReadyToRun=false  -p:PublishTrimmed=false --output ./SelfContained_SingleFile_win-x64
