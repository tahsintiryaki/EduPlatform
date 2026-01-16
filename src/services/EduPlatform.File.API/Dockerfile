FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/services/EduPlatform.File.API/EduPlatform.File.API.csproj", "src/services/EduPlatform.File.API/"]
COPY ["src/shared/EduPlatform.Shared/EduPlatform.Shared.csproj", "src/shared/EduPlatform.Shared/"]
COPY ["src/shared/EduPlatform.Bus/EduPlatform.Bus.csproj", "src/shared/EduPlatform.Bus/"]
RUN dotnet restore "src/services/EduPlatform.File.API/EduPlatform.File.API.csproj"
COPY . .
WORKDIR "/src/src/services/EduPlatform.File.API"
RUN dotnet build "./EduPlatform.File.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EduPlatform.File.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EduPlatform.File.API.dll"]
