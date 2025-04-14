# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy project files and restore dependencies
COPY *.sln ./
COPY *.csproj ./
RUN dotnet restore --no-cache --source https://api.nuget.org/v3/index.json

# Copy remaining files
COPY . .

# Build and publish the application
RUN dotnet publish -c Release -o out

# Runtime Stage
FROM mcr.microsoft.com/dotnet/runtime-deps:9.0-alpine AS base
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/out .

# Expose ports for HTTP and HTTPS
EXPOSE 80
EXPOSE 443

# Entry point for the application
ENTRYPOINT ["dotnet", "UniversityHelper.AuthService.dll"]
