$ErrorActionPreference = "Stop"
$version = $args[0]
$nugetApiKey = $args[1]

dotnet pack -c Release
cd bin/Release
dotnet nuget push TheTraumer.CommonUtil.$version.nupkg -k $nugetApiKey -s https://api.nuget.org/v3/index.json
cd ../..
