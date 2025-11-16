# Build and run the API service
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DeliFHery.Api/DeliFHery.Api.csproj", "DeliFHery.Api/"]
COPY ["DeliFHery.Logic/DeliFHery.Logic.csproj", "DeliFHery.Logic/"]
COPY ["DeliFHery.Persistence/DeliFHery.Persistence.csproj", "DeliFHery.Persistence/"]
RUN dotnet restore "DeliFHery.Api/DeliFHery.Api.csproj"
COPY . .
WORKDIR /src/DeliFHery.Api
RUN dotnet build "DeliFHery.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DeliFHery.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DeliFHery.Api.dll"]
