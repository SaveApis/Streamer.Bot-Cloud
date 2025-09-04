FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Software/Event/Software.Event.Twitch/Software.Event.Twitch.csproj", "src/Software/Event/Software.Event.Twitch/"]
RUN dotnet restore "src/Software/Event/Software.Event.Twitch/Software.Event.Twitch.csproj"
COPY . .
WORKDIR "/src/src/Software/Event/Software.Event.Twitch"
RUN dotnet build "./Software.Event.Twitch.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Software.Event.Twitch.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Software.Event.Twitch.dll"]
