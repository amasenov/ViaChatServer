FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# It's important to keep lines from here down to "COPY . ." identical in all Dockerfiles
# to take advantage of Docker's build cache, to speed up local container builds
COPY "ViaChatServer.sln" "ViaChatServer.sln"

COPY "BuildingBlocks/Infrastructure/Infrastructure.csproj" "BuildingBlocks/Infrastructure/Infrastructure.csproj"
COPY "BuildingBlocks/ClientUtility/ClientUtility.csproj" "BuildingBlocks/ClientUtility/ClientUtility.csproj"
COPY "Clients/Clients/Clients.csproj" "Clients/Clients/Clients.csproj"
COPY "Clients/SimpleClient/SimpleClient.csproj" "Clients/SimpleClient/SimpleClient.csproj"
COPY "Services/Chat.API/Chat.API.API/Chat.API.API.csproj" "Services/Chat.API/Chat.API.API/Chat.API.API.csproj"
COPY "Services/Chat.API/Chat.API.Application/Chat.API.Application.csproj" "Services/Chat.API/Chat.API.Application/Chat.API.Application.csproj"
COPY "Services/Chat.API/Chat.API.Domain/Chat.API.Domain.csproj" "Services/Chat.API/Chat.API.Domain/Chat.API.Domain.csproj"
COPY "Services/Chat.API/Chat.API.Persistence/Chat.API.Persistence.csproj" "Services/Chat.API/Chat.API.Persistence/Chat.API.Persistence.csproj"
COPY "Web/WebSPA/WebSPA.csproj" "Web/WebSPA/WebSPA.csproj"

COPY "docker-compose.dcproj" "docker-compose.dcproj"

COPY "NuGet.config" "NuGet.config"

RUN dotnet restore "ViaChatServer.sln"

COPY . .
WORKDIR /src/Services/Chat.API/Chat.API.API
RUN dotnet publish --no-restore -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Chat.API.API.dll"]