# Use .NET SDK 9.0.4 for building
FROM mcr.microsoft.com/dotnet/sdk:9.0.4 AS build
WORKDIR /app

COPY . ./
RUN dotnet restore -s https://api.nuget.org/v3/index.json
RUN dotnet dev-certs https
RUN dotnet publish -c Release -o out

# Use ASP.NET Runtime 9.0.4 for running the app
FROM mcr.microsoft.com/dotnet/aspnet:9.0.4 AS base
WORKDIR /app

COPY --from=build /app/out .
EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "UniversityHelper.AuthService.dll"]
