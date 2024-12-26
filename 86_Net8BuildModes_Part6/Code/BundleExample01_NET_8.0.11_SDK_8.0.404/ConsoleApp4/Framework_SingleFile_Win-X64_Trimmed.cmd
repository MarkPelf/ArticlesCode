echo .NET SDK version:
dotnet --version
dotnet publish ConsoleApp4.csproj --nologo --no-restore --runtime win-x64 --use-current-runtime  --configuration Release   -p:PublishSingleFile=true --no-self-contained -p:PublishTrimmed=true --output ./Framework_SingleFile_Win-X64_Trimmed