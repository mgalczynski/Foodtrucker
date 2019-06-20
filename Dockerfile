FROM docker.miroslawgalczynski.com/dotnet:2.2-sdk-node AS build
WORKDIR /app
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

ARG ASPNETCORE_ENVIRONMENT
ARG GENERATE_SOURCEMAP=false
ARG CONFIGURATION=Release

COPY ./ ./
WORKDIR /app/WebApplication
RUN dotnet publish -c $CONFIGURATION -o out


FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
COPY --from=build /app/WebApplication/out  ./
HEALTHCHECK --interval=5s --timeout=3s CMD curl -f http://localhost/ || exit 1
ENTRYPOINT ["dotnet"]
CMD ["WebApplication.dll"]
