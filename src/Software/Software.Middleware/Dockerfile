FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Libraries/Software.Core/Software.Core.csproj", "src/Libraries/Software.Core/"]
COPY ["src/Libraries/Utils/Utils.Core/Utils.Core.csproj", "src/Libraries/Utils/Utils.Core/"]
COPY ["src/Libraries/Utils/Utils.Correlation/Utils.Correlation.csproj", "src/Libraries/Utils/Utils.Correlation/"]
COPY ["src/Libraries/Utils/Utils.Hangfire/Utils.Hangfire.csproj", "src/Libraries/Utils/Utils.Hangfire/"]
COPY ["src/Libraries/Utils/Utils.Mediator/Utils.Mediator.csproj", "src/Libraries/Utils/Utils.Mediator/"]
COPY ["src/Libraries/Utils/Utils.Rest/Utils.Rest.csproj", "src/Libraries/Utils/Utils.Rest/"]
COPY ["src/Libraries/Utils/Utils.Swagger/Utils.Swagger.csproj", "src/Libraries/Utils/Utils.Swagger/"]
COPY ["src/Libraries/Utils/Utils.Validation/Utils.Validation.csproj", "src/Libraries/Utils/Utils.Validation/"]
COPY ["src/Software/Software.Middleware/Software.Middleware.csproj", "src/Software/Software.Middleware/"]

RUN --mount=type=secret,id=hangfire_ace_url \
    --mount=type=secret,id=hangfire_ace_user \
    --mount=type=secret,id=hangfire_ace_password \
    --mount=type=secret,id=hangfire_pro_url \
    --mount=type=secret,id=hangfire_pro_user \
    --mount=type=secret,id=hangfire_pro_password \
    dotnet nuget add source "$(cat /run/secrets/hangfire_ace_url)" -n HangfireAce -u "$(cat /run/secrets/hangfire_ace_user)" -p "$(cat /run/secrets/hangfire_ace_password)" --store-password-in-clear-text && \
    dotnet nuget add source "$(cat /run/secrets/hangfire_pro_url)" -n HangfirePro -u "$(cat /run/secrets/hangfire_pro_user)" -p "$(cat /run/secrets/hangfire_pro_password)" --store-password-in-clear-text

RUN --mount=type=cache,target=/root/.nuget/packages dotnet restore "src/Software/Software.Middleware/Software.Middleware.csproj"
COPY . .
WORKDIR "/src/src/Software/Software.Middleware"
RUN dotnet build "./Software.Middleware.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Software.Middleware.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Software.Middleware.dll"]