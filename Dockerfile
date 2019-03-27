FROM docker.miroslawgalczynski.com/dotnet:2.2-sdk-node AS build
WORKDIR /app
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1


COPY ./ ./
WORKDIR /app/WebApplication
RUN GENERATE_SOURCEMAP=false dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
COPY --from=build /app/WebApplication/out  ./
ENTRYPOINT ["dotnet", "WebApplication.dll"]
