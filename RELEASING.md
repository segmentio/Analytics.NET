Releasing
========

 1. Change the version in `Analytics.nuspec`.
 2. Change the version in `Analytics/Properties/AssemblyInfo.cs`.
 3. Update the version in `Analytics/Analytics.cs`.
 4. `nuget pack Analytics.nuspec` to verify the build.
 5. `git commit -am "Release X.Y.Z."` (where X.Y.Z is the new version)
 6. `git tag -a X.Y.Z -m "Version X.Y.Z"` (where X.Y.Z is the new version)
 7. `nuget pack Analytics.nuspec` to build.
 8. `nuget push Analytics.{X.Y.Z}.nupkg`
