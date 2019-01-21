FROM docker.miroslawgalczynski.com/dotnet:2.2-sdk-node AS build
WORKDIR /app
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1


COPY Foodtrucker.sln ./
COPY Persistency/ ./Persistency
COPY WebApplication/ ./WebApplication
WORKDIR /app/WebApplication
RUN dotnet publish -c Release -o out


FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
COPY --from=build /app/WebApplication/out  ./
ENTRYPOINT ["dotnet", "WebApplication.dll"]
