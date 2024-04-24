#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Subscriptions/Subscriptions.WebApi/Subscriptions.WebApi.csproj", "Subscriptions/Subscriptions.WebApi/"]
COPY ["Subscriptions/Subscriptions.Application/Subscriptions.Application.csproj", "Subscriptions/Subscriptions.Application/"]
COPY ["MessageBus/MessageBus.csproj", "MessageBus/"]
COPY ["Subscriptions/Subscriptions.Domain/Subscriptions.Domain.csproj", "Subscriptions/Subscriptions.Domain/"]
COPY ["Subscriptions/Subscriptions.Infrastructure.Data/Subscriptions.Infrastructure.Data.csproj", "Subscriptions/Subscriptions.Infrastructure.Data/"]
RUN dotnet restore "./Subscriptions/Subscriptions.WebApi/Subscriptions.WebApi.csproj"
COPY . .
WORKDIR "/src/Subscriptions/Subscriptions.WebApi"
RUN dotnet build "./Subscriptions.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Subscriptions.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY ["certs/subscriptionwebapi-api.pfx", "https/"]
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Subscriptions.WebApi.dll"]