param($PackageVersion)

dotnet pack src/Hatchet/Hatchet.csproj -o _package -p:PackageVersion=$PackageVersion
