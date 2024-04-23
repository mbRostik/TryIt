#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ApiGateWay/Aggregator/Aggregator.WebApi/Aggregator.WebApi.csproj", "ApiGateWay/Aggregator/Aggregator.WebApi/"]
COPY ["ApiGateWay/Aggregator/Aggregator.Application/Aggregator.Application.csproj", "ApiGateWay/Aggregator/Aggregator.Application/"]
COPY ["ApiGateWay/Aggregator/Aggregator.Infrastructure/Aggregator.Infrastructure.csproj", "ApiGateWay/Aggregator/Aggregator.Infrastructure/"]
RUN dotnet restore "./ApiGateWay/Aggregator/Aggregator.WebApi/Aggregator.WebApi.csproj"
COPY . .
WORKDIR "/src/ApiGateWay/Aggregator/Aggregator.WebApi"
RUN dotnet build "./Aggregator.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Aggregator.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY ["certs/aggregatorwebapi-api.pfx", "https/"]
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Aggregator.WebApi.dll"]