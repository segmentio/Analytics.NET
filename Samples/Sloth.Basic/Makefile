.PHONY: run docker_build docker_run

run:
	dotnet run Sloth.Basic.csproj

docker_build:
	docker build -t 10krps -f Dockerfile ../..

docker_run: docker_build
	docker run -e writeKey -e events 10krps