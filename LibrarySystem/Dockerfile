FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de solución y proyectos
COPY *.sln ./
COPY LibrarySystem/*.csproj LibrarySystem/
COPY Test/*.csproj Test/

# Restaurar dependencias
RUN dotnet restore

# Copiar todo el código
COPY . .

# Build del proyecto principal
RUN dotnet build LibrarySystem/LibrarySystem.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish LibrarySystem/LibrarySystem.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LibrarySystem.dll"]