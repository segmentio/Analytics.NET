Releasing
=========

 1. Change the version in `Analytics.Extensions.Microsoft.DependencyInjection\Analytics.Extensions.Microsoft.DependencyInjection.csproj`.
 2. `dotnet pack -o . -c Release Analytics.Extensions.Microsoft.DependencyInjection\Analytics.Extensions.Microsoft.DependencyInjection.csproj` to verify the build.
 3. `git commit -am "Release DI X.Y.Z."` (where X.Y.Z is the new version)
 4. `git tag -a DI-X.Y.Z -m "Version DI X.Y.Z"` (where X.Y.Z is the new version)
 5. `dotnet pack -o . -c Release Analytics.Extensions.Microsoft.DependencyInjection\Analytics.Extensions.Microsoft.DependencyInjection.csproj` to build.
 6. `nuget push Analytics.Extensions.Microsoft.DependencyInjection.{X.Y.Z}.nupkg -Source https://www.nuget.org/api/v2/package`
