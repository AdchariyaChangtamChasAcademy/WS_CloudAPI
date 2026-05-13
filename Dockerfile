# STAGE 1: BUILD (Byggmiljön)

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY ["CloudNativeApp.Api.csproj", "./"]
RUN dotnet restore "./CloudNativeApp.Api.csproj"

COPY . .
RUN dotnet publish "CloudNativeApp.Api.csproj" -c Release -o /app/publish

# STAGE 2: RUNTIME (Produktionsmiljön)

FROM mcr.microsoft.com/dotnet/aspnet:9.0

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

WORKDIR /app

USER app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "CloudNativeApp.Api.dll"]