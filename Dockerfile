#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CarPlay.csproj", "."]
RUN dotnet restore "./CarPlay.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./CarPlay.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Установка пакета tzdata для настройки временной зоны
RUN apt-get update && apt-get install -y tzdata

# Установка временной зоны
ENV TZ=Europe/Moscow
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CarPlay.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CarPlay.dll"]