FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Software/Bot/Software.Bot.YouTube/Software.Bot.YouTube.csproj", "src/Software/Bot/Software.Bot.YouTube/"]
RUN --mount=type=cache,target=/root/.nuget/packages dotnet restore "src/Software/Bot/Software.Bot.YouTube/Software.Bot.YouTube.csproj"
COPY . .
WORKDIR "/src/src/Software/Bot/Software.Bot.YouTube"
RUN dotnet build "./Software.Bot.YouTube.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Software.Bot.YouTube.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Software.Bot.YouTube.dll"]