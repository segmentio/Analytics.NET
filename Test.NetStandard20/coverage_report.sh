#!/bin/sh
set -ex

dotnet restore
dotnet build
dotnet minicover instrument --workdir ../ --assemblies Test.NetStandard20/**/bin/**/*.dll --sources /**/*.cs
dotnet minicover reset
dotnet test --no-build
dotnet minicover opencoverreport --workdir ../
