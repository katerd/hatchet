msbuild /p:Configuration=Release
.nuget\nuget.exe pack Hatchet.csproj -Version %1 -Prop Configuration=Release
