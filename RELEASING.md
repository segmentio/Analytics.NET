Releasing
=========

 1. Change the version in `Analytics.nuspec`.
 2. Update the version in `Analytics/Analytics.cs`.
 3. `nuget pack Analytics.nuspec` to verify the build.
 4. `git commit -am "Release X.Y.Z."` (where X.Y.Z is the new version)
 5. `git tag -a X.Y.Z -m "Version X.Y.Z"` (where X.Y.Z is the new version)
 6. `nuget pack Analytics.nuspec` to build.
 7. `nuget push Analytics.{X.Y.Z}.nupkg`
