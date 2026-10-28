# ---- Stage 1: build ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore src/UberSy.Api/UberSy.Api.csproj
RUN dotnet publish src/UberSy.Api/UberSy.Api.csproj -c Release -o /app/publish --no-restore

# ---- Stage 2: run ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "UberSy.Api.dll"]
