#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/IdentityServer/IdentityServer.WebApi/IdentityServer.WebApi.csproj", "src/IdentityServer/IdentityServer.WebApi/"]
COPY ["src/MessageBus/MessageBus.csproj", "src/MessageBus/"]
COPY ["src/IdentityServer/IdentityServer.Application/IdentityServer.Application.csproj", "src/IdentityServer/IdentityServer.Application/"]
COPY ["src/IdentityServer/IdentityServer.Infrastructure.Data/IdentityServer.Infrastructure.Data.csproj", "src/IdentityServer/IdentityServer.Infrastructure.Data/"]
RUN dotnet restore "./src/IdentityServer/IdentityServer.WebApi/IdentityServer.WebApi.csproj"
COPY . .
WORKDIR "/src/src/IdentityServer/IdentityServer.WebApi"
RUN dotnet build "./IdentityServer.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./IdentityServer.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY ["src/certs/identityserverapi-api.pfx", "https/"]
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityServer.WebApi.dll"]
