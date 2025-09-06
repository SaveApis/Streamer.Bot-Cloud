FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Software/Event/Software.Event.Discord/Software.Event.Discord.csproj", "src/Software/Event/Software.Event.Discord/"]
RUN --mount=type=cache,target=/root/.nuget/packages dotnet restore "src/Software/Event/Software.Event.Discord/Software.Event.Discord.csproj"
COPY . .
WORKDIR "/src/src/Software/Event/Software.Event.Discord"
RUN dotnet build "./Software.Event.Discord.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Software.Event.Discord.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Software.Event.Discord.dll"]