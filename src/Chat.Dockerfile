#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Chats/Chats.WebApi/Chats.WebApi.csproj", "Chats/Chats.WebApi/"]
COPY ["Chats/Chats.Application/Chats.Application.csproj", "Chats/Chats.Application/"]
COPY ["MessageBus/MessageBus.csproj", "MessageBus/"]
COPY ["Chats/Chats.Domain/Chats.Domain.csproj", "Chats/Chats.Domain/"]
COPY ["Chats/Chats.Infrastructure.Data/Chats.Infrastructure.Data.csproj", "Chats/Chats.Infrastructure.Data/"]
COPY ["Chats/Chats.Infrastructure/Chats.Infrastructure.csproj", "Chats/Chats.Infrastructure/"]
RUN dotnet restore "./Chats/Chats.WebApi/Chats.WebApi.csproj"
COPY . .
WORKDIR "/src/Chats/Chats.WebApi"
RUN dotnet build "./Chats.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Chats.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY ["certs/chatwebapi-api.pfx", "https/"]
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Chats.WebApi.dll"]