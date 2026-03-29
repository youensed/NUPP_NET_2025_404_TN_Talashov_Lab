# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies (only projects needed for REST API)
COPY PetStore.Common/*.csproj ./PetStore.Common/
COPY PetStore.Infrastructure/*.csproj ./PetStore.Infrastructure/
COPY PetStore.REST/*.csproj ./PetStore.REST/

# Restore dependencies for REST project (will restore all referenced projects)
WORKDIR /src/PetStore.REST
RUN dotnet restore

# Copy everything else and build
WORKDIR /src
COPY . .
WORKDIR /src/PetStore.REST
RUN dotnet build -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

# Copy published app
COPY --from=publish /app/publish .

# Set environment to Production
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the app
ENTRYPOINT ["dotnet", "PetStore.REST.dll"]

