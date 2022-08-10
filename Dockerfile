#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

##
# Base
##
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
## Necessary to ensure that we listen on ANY IP address
ENV ASPNETCORE_URLS=http://*:3000
EXPOSE 3000

##
# Build
##

# Copy over project into new dir
# Restore nuget packs and other dependencies
# Copy over any newly generated files to working dir
# Change working dir to where the project files are
# Perform a build with a "Release" flag
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["sample-net6-api-service/sample-net6-api-service.csproj", "sample-net6-api-service/"]
RUN dotnet restore "sample-net6-api-service/sample-net6-api-service.csproj"
COPY . .
WORKDIR "/src/sample-net6-api-service"
RUN dotnet build "sample-net6-api-service.csproj" -c Release -o /app/build

##
# Publish
##

# Perform a publish to prepare the project for deployment on a remote system
FROM build AS publish
RUN dotnet publish "sample-net6-api-service.csproj" -c Release -o /app/publish


##
# Final
##

# Copy over the folder from the "Publish" step into the base working dir
# Run the binary
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "sample-net6-api-service.dll"]