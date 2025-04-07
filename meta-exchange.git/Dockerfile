# syntax=docker/dockerfile:1

# Define the .NET version to use
ARG DOTNET_VERSION=8.0

# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS builder
WORKDIR /source

# Copy the project file and restore dependencies
COPY meta-exchange.git/meta-exchange.git.csproj .
RUN --mount=type=cache,target=/root/.nuget/packages \
    --mount=type=cache,target=/root/.cache/msbuild \
    dotnet restore

# Copy the rest of the application source code
COPY meta-exchange.git/ .

# Build the application
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS final
WORKDIR /app

# Copy the published application from the builder stage
COPY --from=builder /app/publish .

# Set the entry point for the application
ENTRYPOINT ["dotnet", "meta-exchange.git.dll"]