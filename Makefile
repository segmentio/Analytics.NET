assemble:
	@xbuild Analytics/Analytics.csproj
	@xbuild Test/Test.csproj

test: assemble
	@nunit-console Test/bin/Debug/Test.dll
