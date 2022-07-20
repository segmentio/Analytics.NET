# Releasing

Pre-requisits: 
Make sure to have .net installed ( download from [here](https://dotnet.microsoft.com/en-us/download/dotnet) )


1.  Change the version in `Analytics/Analytics.csproj`.
2.  Update the version in `Analytics/Analytics.cs`.
3.  `dotnet pack -o . -c Release Analytics/Analytics.csproj` to verify the build.
4.  `git commit -am "Release X.Y.Z."` (where X.Y.Z is the new version)
5.  `git tag -a X.Y.Z -m "Version X.Y.Z"` (where X.Y.Z is the new version)
6.  `dotnet pack -o . -c Release Analytics/Analytics.csproj` to build.
7.  `dotnet nuget push Analytics.{X.Y.Z}.nupkg -s https://www.nuget.org/api/v2/package -k <NUGET_API_KEY>`
8.  `git push origin master` to push the last commit 
9.  `git push --tags` to push the release tag
