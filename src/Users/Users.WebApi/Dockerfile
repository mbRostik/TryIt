#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Users/Users.WebApi/Users.WebApi.csproj", "Users/Users.WebApi/"]
COPY ["Users/Users.Application/Users.Application.csproj", "Users/Users.Application/"]
COPY ["MessageBus/MessageBus.csproj", "MessageBus/"]
COPY ["Users/Users.Domain/Users.Domain.csproj", "Users/Users.Domain/"]
COPY ["Users/Users.Infrastructure.Data/Users.Infrastructure.Data.csproj", "Users/Users.Infrastructure.Data/"]
COPY ["Users/Users.Infrastructure/Users.Infrastructure.csproj", "Users/Users.Infrastructure/"]
RUN dotnet restore "./Users/Users.WebApi/Users.WebApi.csproj"
COPY . .
WORKDIR "/src/Users/Users.WebApi"
RUN dotnet build "./Users.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Users.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY ["certs/userwebapi-api.pfx", "https/"]
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Users.WebApi.dll"]