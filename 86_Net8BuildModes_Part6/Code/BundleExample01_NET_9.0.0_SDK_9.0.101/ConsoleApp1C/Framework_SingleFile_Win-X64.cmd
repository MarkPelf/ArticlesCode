echo .NET SDK version:
dotnet --version
dotnet publish ConsoleApp1C.csproj --nologo --no-restore --runtime win-x64 --use-current-runtime  --configuration Release   -p:PublishSingleFile=true --no-self-contained --output ./Framework_SingleFile_Win-X64 