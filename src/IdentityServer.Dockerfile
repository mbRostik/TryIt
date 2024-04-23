FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["IdentityServer/IdentityServer.WebApi/IdentityServer.WebApi.csproj", "IdentityServer/IdentityServer.WebApi/"]
COPY ["MessageBus/MessageBus.csproj", "MessageBus/"]
COPY ["IdentityServer/IdentityServer.Application/IdentityServer.Application.csproj", "IdentityServer/IdentityServer.Application/"]
COPY ["IdentityServer/IdentityServer.Infrastructure.Data/IdentityServer.Infrastructure.Data.csproj", "IdentityServer/IdentityServer.Infrastructure.Data/"]
RUN dotnet restore "./IdentityServer/IdentityServer.WebApi/IdentityServer.WebApi.csproj"
COPY . .
WORKDIR "/src/IdentityServer/IdentityServer.WebApi"
RUN dotnet build "./IdentityServer.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./IdentityServer.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY ["certs/identityserverapi-api.pfx", "https/"]
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityServer.WebApi.dll"]