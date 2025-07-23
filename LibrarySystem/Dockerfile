FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar todos los archivos .csproj primero
COPY *.csproj ./
RUN dotnet restore

# Copiar todo el código
COPY . .

# Build y publish
RUN dotnet build -c Release -o /app/build
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Buscar automáticamente el DLL principal
RUN ls -la
ENTRYPOINT ["dotnet", "$(ls *.dll | head -1)"]