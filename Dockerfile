FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY MidasAPI/MidasAPI.csproj MidasAPI/
RUN dotnet restore MidasAPI/MidasAPI.csproj

COPY MidasAPI/. MidasAPI/
WORKDIR /src/MidasAPI
RUN dotnet publish MidasAPI.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "MidasAPI.dll"]
