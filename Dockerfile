FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["FintachartsApi/FintachartsApi.csproj", "FintachartsApi/"]
RUN dotnet restore "FintachartsApi/FintachartsApi.csproj"

COPY . .
WORKDIR "/src/FintachartsApi"
RUN dotnet publish "FintachartsApi.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FintachartsApi.dll"]