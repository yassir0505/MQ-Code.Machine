# STAGE 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything
COPY . ./

# Restore packages
RUN dotnet restore

# Build the project
RUN dotnet publish -c Release -o out

# STAGE 2: Run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy build result
COPY --from=build /app/out ./

# Set environment variable to production
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose port
EXPOSE 80

# Run the app
ENTRYPOINT ["dotnet", "WebApplication1.dll"]
