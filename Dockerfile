FROM mcr.microsoft.com/dotnet/sdk:9.0-bullseye-slim AS build
WORKDIR /app

COPY . ./
RUN dotnet restore -s https://api.nuget.org/v3/index.json

COPY . ./
RUN dotnet dev-certs https
#RUN dotnet dev-certs https --trust
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0-bullseye-slim AS base
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 80
EXPOSE 443
ENV PATH="${PATH}:/usr/bin/dotnet"

COPY --from=build /root/.dotnet/corefx/cryptography/x509stores/my/* /root/.dotnet/corefx/cryptography/x509stores/my/
#RUN dotnet dev-certs https
#RUN dotnet dev-certs https --trust

ENTRYPOINT ["dotnet", "UniversityHelper.AuthService.dll"]
