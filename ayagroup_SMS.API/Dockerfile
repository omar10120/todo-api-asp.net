# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ayagroup_SMS.API/ayagroup_SMS.API.csproj", "ayagroup_SMS.API/"]
COPY ["ayagroup_SMS.Application/ayagroup_SMS.Application.csproj", "ayagroup_SMS.Application/"]
COPY ["ayagroup_SMS.Core/ayagroup_SMS.Core.csproj", "ayagroup_SMS.Core/"]
COPY ["ayagroup_SMS.Infrastructure/ayagroup_SMS.Infrastructure.csproj", "ayagroup_SMS.Infrastructure/"]
RUN dotnet restore "./ayagroup_SMS.API/ayagroup_SMS.API.csproj"
COPY . .
WORKDIR "/src/ayagroup_SMS.API"
RUN dotnet build "./ayagroup_SMS.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ayagroup_SMS.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ayagroup_SMS.API.dll"]