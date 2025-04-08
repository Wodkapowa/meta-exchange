# syntax=docker/dockerfile:1

ARG DOTNET_VERSION=9.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /src
COPY CryptoExecutionService/CryptoExecutionService.csproj ./CryptoExecutionService/
RUN dotnet restore CryptoExecutionService/CryptoExecutionService.csproj
COPY CryptoExecutionService/ ./CryptoExecutionService/
COPY meta-exchange.git/ ./meta-exchange.git/
RUN dotnet publish CryptoExecutionService/CryptoExecutionService.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION}
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "CryptoExecutionService.dll"]
