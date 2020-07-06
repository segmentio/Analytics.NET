#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:2.1-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["Samples/Sloth.Enterprise/Sloth.Enterprise.csproj", "Samples/Sloth.Enterprise/"]
COPY ["Samples/Sloth.Common/Sloth.Common.csproj", "Samples/Sloth.Common/"]
RUN dotnet restore "Samples/Sloth.Enterprise/Sloth.Enterprise.csproj"
COPY . .
WORKDIR "/src/Samples/Sloth.Enterprise"
RUN dotnet build "Sloth.Enterprise.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sloth.Enterprise.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sloth.Enterprise.dll"]