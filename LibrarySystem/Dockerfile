FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar archivo de proyecto (desde la raíz del proyecto)
COPY ["LibrarySystem.csproj", "./"]
RUN dotnet restore "LibrarySystem.csproj"

# Copiar todo el código fuente
COPY . .

# Build de la aplicación (sin cambiar directorio)
RUN dotnet build "LibrarySystem.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "LibrarySystem.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LibrarySystem.dll"]