assemble:
	@msbuild Analytics/Analytics.csproj /t:restore
	@msbuild Analytics/Analytics.csproj /p:Configuration=Release
	@msbuild Analytics.Net35/Analytics.Net35.csproj /t:restore
	@msbuild Analytics.Net35/Analytics.Net35.csproj /p:Configuration=Release

test: assemble
	@nunit-console Test/bin/Debug/Test.dll

PHONY: assemble test
