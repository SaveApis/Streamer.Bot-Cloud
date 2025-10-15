FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM registry.digitalocean.com/saveapis/streamer-bot-base:latest AS publish
WORKDIR /src
COPY . .
WORKDIR /src/src/Software/Middleware
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Middleware.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Middleware.dll"]
