# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY *.sln .

# Copy csproj files and restore dependencies
COPY PetStore.REST/*.csproj ./PetStore.REST/
COPY PetStore.Common/*.csproj ./PetStore.Common/
COPY PetStore.Infrastructure/*.csproj ./PetStore.Infrastructure/
RUN dotnet restore

# Copy everything else and build
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

