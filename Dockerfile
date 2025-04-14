# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# 1. Copy project/solution files explicitly
COPY *.sln ./
COPY *.csproj ./

# 2. Robust restore command
RUN dotnet restore "AuthService.csproj" --no-cache --source https://api.nuget.org/v3/index.json || \
    (echo "NuGet restore failed. Check network connectivity." && exit 1)

# 3. Copy remaining files
COPY . ./

# 4. Build and publish the application
RUN dotnet publish -c Release -o out

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/out .

# Expose ports for HTTP and HTTPS
EXPOSE 80
EXPOSE 443

# Entry point for the application
ENTRYPOINT ["dotnet", "UniversityHelper.AuthService.dll"]
