FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Events.Application/", "Events.Application/"]
COPY ["Events.Core/", "Events.Core/"]
COPY ["Events.Infrastructure/", "Events.Infrastructure/"]
COPY ["Events.DataService/", "Events.DataService/"]

RUN dotnet restore "Events.DataService/Events.DataService.csproj"


WORKDIR /src/Events.DataService
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Events.DataService.dll"]