FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["OpcServer/OpcServer.csproj", "OpcServer/"]
COPY ["OpcServer/Config/BatchPlantServer.Config.xml", "OpcServer/Config/BatchPlantServer.Config.xml"]
RUN dotnet restore "OpcServer/OpcServer.csproj"
COPY . .
WORKDIR "/src/OpcServer"
RUN dotnet build "OpcServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpcServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpcServer.dll"]
