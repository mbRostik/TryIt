#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Reports/Reports.WebApi/Reports.WebApi.csproj", "Reports/Reports.WebApi/"]
COPY ["Reports/Reports.Application/Reports.Application.csproj", "Reports/Reports.Application/"]
COPY ["MessageBus/MessageBus.csproj", "MessageBus/"]
COPY ["Reports/Reports.Domain/Reports.Domain.csproj", "Reports/Reports.Domain/"]
COPY ["Reports/Reports.Infrastructure.Data/Reports.Infrastructure.Data.csproj", "Reports/Reports.Infrastructure.Data/"]
RUN dotnet restore "./Reports/Reports.WebApi/Reports.WebApi.csproj"
COPY . .
WORKDIR "/src/Reports/Reports.WebApi"
RUN dotnet build "./Reports.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Reports.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY ["certs/reportwebapi-api.pfx", "https/"]
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Reports.WebApi.dll"]