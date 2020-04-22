.PHONY: run docker_build docker_run

run:
	dotnet run Sloth.Enterprise.csproj

docker_build:
	docker build -t 100krps -f DockerFile ../..

docker_run: docker_build
	docker run -e writeKey -e events 100krps