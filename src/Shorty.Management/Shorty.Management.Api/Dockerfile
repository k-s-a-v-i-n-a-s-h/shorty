FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

RUN mkdir -p /data/sqlite && chown -R $APP_UID:$APP_UID /data/sqlite
USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Shorty.Management.Api/Shorty.Management.Api.csproj", "Shorty.Management.Api/"]
COPY ["Shorty.Management.Domain/Shorty.Management.Domain.csproj", "Shorty.Management.Domain/"]
COPY ["Shorty.Management.Infrastructure/Shorty.Management.Infrastructure.csproj", "Shorty.Management.Infrastructure/"]

RUN dotnet restore "Shorty.Management.Api/Shorty.Management.Api.csproj"
COPY . .
WORKDIR "Shorty.Management.Api"
RUN dotnet build "Shorty.Management.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS migrationbuilder
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet ef migrations bundle --self-contained -r linux-x64 -o /app/efbundle

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Shorty.Management.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=migrationbuilder /app/efbundle .

USER root
RUN chmod +x /app/efbundle && chown $APP_UID:$APP_UID /app/efbundle
USER $APP_UID

ENTRYPOINT ["dotnet", "Shorty.Management.Api.dll"]