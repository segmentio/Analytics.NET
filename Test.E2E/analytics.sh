#!/bin/sh

set -e
echo $@
echo "Starting dotnet run"
dotnet run -- "$@"

echo "Finishing dotnet run"