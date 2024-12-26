echo .NET SDK version:
dotnet --version
dotnet publish ConsoleApp5.csproj --nologo --no-restore --runtime win-x64 --configuration Release  -p:SelfContained=true  -p:PublishTrimmed=true --output ./win-x64_Trimmed_AOT
