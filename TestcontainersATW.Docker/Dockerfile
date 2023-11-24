FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TestcontainersATW.Docker/TestcontainersATW.Docker.csproj", "TestcontainersATW.Docker/"]
RUN dotnet restore "TestcontainersATW.Docker/TestcontainersATW.Docker.csproj"
COPY . .
WORKDIR "/src/TestcontainersATW.Docker"
RUN dotnet build "TestcontainersATW.Docker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TestcontainersATW.Docker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestcontainersATW.Docker.dll"]
