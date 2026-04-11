FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/lms.web/lms.web.csproj", "src/lms.web/"]
COPY ["aspire/ServiceDefaults/ServiceDefaults.csproj", "aspire/ServiceDefaults/"]
RUN dotnet restore "src/lms.web/lms.web.csproj"
COPY . .
WORKDIR "/src/src/lms.web"
RUN dotnet publish "lms.web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "lms.web.dll"]
