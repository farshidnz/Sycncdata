FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR / 	

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /code
COPY AccountsSyncData.sln ./
COPY ./src ./src
COPY ./test ./test
RUN dotnet restore "AccountsSyncData.sln"
RUN dotnet build "AccountsSyncData.sln" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish --no-restore "./src/AccountsSyncData/AccountsSyncData.csproj" -c Release -o /app/publish

FROM base AS final
ARG FUNCTION_DIR="/var/task"
RUN mkdir -p ${FUNCTION_DIR}
WORKDIR ${FUNCTION_DIR}
COPY --from=publish /app/publish .
CMD [ "AccountsSyncData::AccountsSyncData.Function::FunctionHandler" ]