param($PackageVersion)

dotnet test
dotnet pack src/Hatchet/Hatchet.csproj -o _package -p:PackageVersion=$PackageVersion
