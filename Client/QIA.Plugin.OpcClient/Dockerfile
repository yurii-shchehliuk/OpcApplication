#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
# Copy and restore NuGet packages
COPY ["QIA.Plugin.OpcClient/Packages.config", "QIA.Plugin.OpcClient/"]
COPY ["QIA.Plugin.OpcClient/QIA.Plugin.OpcClient.csproj", "QIA.Plugin.OpcClient/"]

RUN dotnet restore "QIA.Plugin.OpcClient/QIA.Plugin.OpcClient.csproj" --configfile "QIA.Plugin.OpcClient\Packages.config"
COPY . .

WORKDIR "/src/QIA.Plugin.OpcClient"
RUN dotnet build "QIA.Plugin.OpcClient.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "QIA.Plugin.OpcClient.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app


COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QIA.Plugin.OpcClient.dll"]