FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/lms.services/lms.services.coursemanagement/lms.services.coursemanagement.csproj", "src/lms.services/lms.services.coursemanagement/"]
COPY ["src/lms.buildingblocks/lms.buildingblocks.csproj", "src/lms.buildingblocks/"]
COPY ["src/lms.shared.common/lms.shared.common.csproj", "src/lms.shared.common/"]
COPY ["src/lms.shared.data/lms.shared.data.csproj", "src/lms.shared.data/"]
COPY ["src/lms.services.aws/lms.services.aws.csproj", "src/lms.services.aws/"]
COPY ["aspire/ServiceDefaults/ServiceDefaults.csproj", "aspire/ServiceDefaults/"]
RUN dotnet restore "src/lms.services/lms.services.coursemanagement/lms.services.coursemanagement.csproj"
COPY . .
WORKDIR "/src/src/lms.services/lms.services.coursemanagement"
RUN dotnet publish "lms.services.coursemanagement.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "lms.services.coursemanagement.dll"]
