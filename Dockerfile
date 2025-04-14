# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-bullseye-slim AS build
WORKDIR /app

# Copy project files and restore dependencies
COPY . ./
RUN dotnet restore --no-cache --source https://api.nuget.org/v3/index.json

# Build and publish the application
RUN dotnet publish -c Release -o out

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-bullseye-slim AS base
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/out .

# Expose ports for HTTP and HTTPS
EXPOSE 80
EXPOSE 443

# Set environment variables (optional)
ENV DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_USE_POLLING_FILE_WATCHER=true \
    ASPNETCORE_URLS=http://+:80

# Entry point for the application
ENTRYPOINT ["dotnet", "UniversityHelper.AuthService.dll"]
