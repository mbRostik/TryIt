#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Notifications/Notifications.WebApi/Notifications.WebApi.csproj", "Notifications/Notifications.WebApi/"]
COPY ["Notifications/Notifications.Application/Notifications.Application.csproj", "Notifications/Notifications.Application/"]
COPY ["MessageBus/MessageBus.csproj", "src/MessageBus/"]
COPY ["Notifications/Notifications.Domain/Notifications.Domain.csproj", "Notifications/Notifications.Domain/"]
COPY ["Notifications/Notifications.Infrastructure.Data/Notifications.Infrastructure.Data.csproj", "Notifications/Notifications.Infrastructure.Data/"]
RUN dotnet restore "./Notifications/Notifications.WebApi/Notifications.WebApi.csproj"
COPY . .
WORKDIR "/src/Notifications/Notifications.WebApi"
RUN dotnet build "./Notifications.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Notifications.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY ["certs/notificationwebapi-api.pfx", "https/"]
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notifications.WebApi.dll"]