FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ui/EduPlatform.Web/EduPlatform.Web.csproj", "src/ui/EduPlatform.Web/"]
RUN dotnet restore "src/ui/EduPlatform.Web/EduPlatform.Web.csproj"
COPY . .
WORKDIR "/src/src/ui/EduPlatform.Web"
RUN dotnet build "./EduPlatform.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EduPlatform.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EduPlatform.Web.dll"]
