FROM mcr.microsoft.com/dotnet/sdk:latest AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:latest AS build
WORKDIR /src
COPY ["AccountSyncData.Consumer/AccountSyncData.Consumer.csproj", "AccountSyncData.Consumer/"]
RUN dotnet restore "AccountSyncData.Consumer/AccountSyncData.Consumer.csproj"
COPY . .
WORKDIR "/src/AccountSyncData.Consumer"
RUN dotnet build "AccountSyncData.Consumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AccountSyncData.Consumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AccountSyncData.Consumer.dll"]
