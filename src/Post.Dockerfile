#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Posts/Posts.WebApi/Posts.WebApi.csproj", "Posts/Posts.WebApi/"]
COPY ["Posts/Posts.Application/Posts.Application.csproj", "Posts/Posts.Application/"]
COPY ["MessageBus/MessageBus.csproj", "MessageBus/"]
COPY ["Posts/Posts.Domain/Posts.Domain.csproj", "Posts/Posts.Domain/"]
COPY ["Posts/Posts.Infrastructure.Data/Posts.Infrastructure.Data.csproj", "Posts/Posts.Infrastructure.Data/"]
COPY ["Posts/Posts.Infrastructure/Posts.Infrastructure.csproj", "Posts/Posts.Infrastructure/"]
RUN dotnet restore "./Posts/Posts.WebApi/Posts.WebApi.csproj"
COPY . .
WORKDIR "/src/Posts/Posts.WebApi"
RUN dotnet build "./Posts.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Posts.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY ["certs/postwebapi-api.pfx", "https/"]
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Posts.WebApi.dll"]