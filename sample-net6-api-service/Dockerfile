#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["sample-net6-api-service/sample-net6-api-service.csproj", "sample-net6-api-service/"]
RUN dotnet restore "sample-net6-api-service/sample-net6-api-service.csproj"
COPY . .
WORKDIR "/src/sample-net6-api-service"
RUN dotnet build "sample-net6-api-service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "sample-net6-api-service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "sample-net6-api-service.dll"]