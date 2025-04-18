# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy project files and restore dependencies
COPY . ./
RUN dotnet restore --no-cache --source https://api.nuget.org/v3/index.json

COPY . ./
RUN dotnet dev-certs https --trust
#RUN dotnet dev-certs https --trust
RUN dotnet publish -c Release -o out

# Build and publish the application
# RUN dotnet publish -c Release -o out

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/out .

# Expose ports for HTTP and HTTPS
EXPOSE 80
EXPOSE 443
ENV PATH="${PATH}:/usr/bin/dotnet"

COPY --from=build /root/.dotnet/corefx/cryptography/x509stores/my/* /root/.dotnet/corefx/cryptography/x509stores/my/


# Entry point for the application
ENTRYPOINT ["dotnet", "UniversityHelper.AuthService.dll"]
