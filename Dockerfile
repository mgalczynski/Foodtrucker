FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /app
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
RUN curl -sL https://deb.nodesource.com/setup_11.x | bash - \ 
    && apt-get install -y nodejs \
    && rm -rf /var/lib/apt/lists/*


COPY ./ ./
WORKDIR /app/WebApplication
RUN dotnet publish -c Release -o out


FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
COPY --from=build /app/WebApplication/out  ./
ENTRYPOINT ["dotnet", "WebApplication.dll"]