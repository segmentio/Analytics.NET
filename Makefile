assemble:
	@msbuild Analytics/Analytics.csproj /t:restore
	@msbuild Analytics/Analytics.csproj /p:Configuration=Release
	@msbuild Analytics.Net35/Analytics.Net35.csproj /t:restore
	@msbuild Analytics.Net35/Analytics.Net35.csproj /p:Configuration=Release
	@msbuild Analytics.Net45/Analytics.Net45.csproj /t:restore
	@msbuild Analytics.Net45/Analytics.Net45.csproj /p:Configuration=Release
	@msbuild Analytics.NetStandard20/Analytics.NetStandard20.csproj /t:restore
	@msbuild Analytics.NetStandard20/Analytics.NetStandard20.csproj /p:Configuration=Release

test: assemble
	@nunit-console Test/bin/Debug/Test.dll

PHONY: assemble test
