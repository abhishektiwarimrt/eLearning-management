# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["lms.services/lms.services.coursemanagement/lms.services.coursemanagement.csproj", "lms.services/lms.services.coursemanagement/"]
COPY ["lms.services.aws/lms.services.aws.csproj", "lms.services.aws/"]
COPY ["lms.buildingblocks/lms.buildingblocks.csproj", "lms.buildingblocks/"]
COPY ["lms.shared.common/lms.shared.common.csproj", "lms.shared.common/"]
COPY ["lms.shared.data/lms.shared.data.csproj", "lms.shared.data/"]
RUN dotnet restore "./lms.services/lms.services.coursemanagement/lms.services.coursemanagement.csproj"
COPY . .
WORKDIR "/src/lms.services/lms.services.coursemanagement"
RUN dotnet build "./lms.services.coursemanagement.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./lms.services.coursemanagement.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "lms.services.coursemanagement.dll"]