FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

RUN mkdir -p /data/sqlite && chown -R $APP_UID:$APP_UID /data/sqlite
USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Shorty.Management.Worker/Shorty.Management.Worker.csproj", "Shorty.Management.Worker/"]
COPY ["Shorty.Management.Infrastructure/Shorty.Management.Infrastructure.csproj", "Shorty.Management.Infrastructure/"]
COPY ["Shorty.Management.Domain/Shorty.Management.Domain.csproj", "Shorty.Management.Domain/"]

RUN dotnet restore "Shorty.Management.Worker/Shorty.Management.Worker.csproj"
COPY . .
WORKDIR "Shorty.Management.Worker"
RUN dotnet build "Shorty.Management.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Shorty.Management.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Shorty.Management.Worker.dll"]