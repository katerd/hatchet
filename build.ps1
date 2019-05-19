param($PackageVersion)

dotnet test
dotnet pack src/Hatchet/Hatchet.csproj -c Release -o _package -p:PackageVersion=$PackageVersion
