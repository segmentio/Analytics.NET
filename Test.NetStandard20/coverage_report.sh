#!/bin/sh
set -ex

dotnet restore
dotnet build
minicover instrument --workdir ../ --assemblies Test.NetStandard20/**/bin/**/*.dll --sources Analytics/**/*.cs --tests Test.NetStandard20/**/*.cs
minicover reset
dotnet test --no-build
minicover opencoverreport --workdir ../ --threshold 80
