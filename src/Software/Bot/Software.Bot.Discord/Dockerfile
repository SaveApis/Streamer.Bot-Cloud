FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Software/Bot/Software.Bot.Discord/Software.Bot.Discord.csproj", "src/Software/Bot/Software.Bot.Discord/"]

RUN --mount=type=secret,id=hangfire_ace_url \
    --mount=type=secret,id=hangfire_ace_user \
    --mount=type=secret,id=hangfire_ace_password \
    --mount=type=secret,id=hangfire_pro_url \
    --mount=type=secret,id=hangfire_pro_user \
    --mount=type=secret,id=hangfire_pro_password \
    dotnet nuget add source "$(cat /run/secrets/hangfire_ace_url)" -n HangfireAce -u "$(cat /run/secrets/hangfire_ace_user)" -p "$(cat /run/secrets/hangfire_ace_password)" --store-password-in-clear-text && \
    dotnet nuget add source "$(cat /run/secrets/hangfire_pro_url)" -n HangfirePro -u "$(cat /run/secrets/hangfire_pro_user)" -p "$(cat /run/secrets/hangfire_pro_password)" --store-password-in-clear-text

RUN --mount=type=cache,target=/root/.nuget/packages dotnet restore "src/Software/Bot/Software.Bot.Discord/Software.Bot.Discord.csproj"
COPY . .
WORKDIR "/src/src/Software/Bot/Software.Bot.Discord"
RUN dotnet build "./Software.Bot.Discord.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Software.Bot.Discord.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Software.Bot.Discord.dll"]