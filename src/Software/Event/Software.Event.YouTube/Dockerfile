FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Software/Event/Software.Event.YouTube/Software.Event.YouTube.csproj", "src/Software/Event/Software.Event.YouTube/"]

RUN --mount=type=secret,id=hangfire_ace_url \
    --mount=type=secret,id=hangfire_ace_username \
    --mount=type=secret,id=hangfire_ace_token \
    --mount=type=secret,id=hangfire_pro_url \
    --mount=type=secret,id=hangfire_pro_username \
    --mount=type=secret,id=hangfire_pro_token \
    dotnet nuget add source "$(cat /run/secrets/hangfire_ace_url)" -n Hangfire.Ace -u "$(cat /run/secrets/hangfire_ace_username)" -p "$(cat /run/secrets/hangfire_ace_token)" --store-password-in-clear-text && \
    dotnet nuget add source "$(cat /run/secrets/hangfire_pro_url)" -n Hangfire.Pro -u "$(cat /run/secrets/hangfire_pro_username)" -p "$(cat /run/secrets/hangfire_pro_token)" --store-password-in-clear-text

RUN --mount=type=cache,target=/root/.nuget/packages dotnet restore "src/Software/Event/Software.Event.YouTube/Software.Event.YouTube.csproj"
COPY . .
WORKDIR "/src/src/Software/Event/Software.Event.YouTube"
RUN dotnet build "./Software.Event.YouTube.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Software.Event.YouTube.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Software.Event.YouTube.dll"]