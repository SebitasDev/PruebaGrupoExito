# 📚 Sistema de Gestión de Biblioteca (LibrarySystem)

Sistema desarrollado en .NET 8 para la gestión de préstamos de material bibliográfico en un centro documental, implementando arquitectura limpia y principios SOLID.

## 🏗️ Arquitectura del Proyecto

```
LibrarySystem/
├── 📁 Data/                          # Contexto de Base de Datos
│   ├── 🔷 LibraryDbContext.cs        # Contexto principal de Entity Framework
│   └── 📁 Migrations/                # Migraciones de base de datos
├── 📁 Extensions/                    # Métodos de extensión
├── 📁 Models/                        # Modelos de dominio
│   ├── 📁 DTOs/                     # Data Transfer Objects
│   ├── 📁 Entities/                 # Entidades del dominio
│   └── 📁 Enums/                    # Enumeraciones
├── 📁 Repository/                    # Patrón Repository
│   ├── 📁 Interface/                # Interfaces de repositorios
│   ├── 🔷 BibliographicMaterialRepository.cs
│   └── 🔷 LoanRepository.cs
├── 📁 Service/                       # Lógica de negocio
│   ├── 📁 Interface/                # Interfaces de servicios
│   ├── 🔷 BusinessDaysService.cs    # Gestión de días hábiles
│   └── 🔷 LoanService.cs           # Lógica de préstamos
├── 📁 Controllers/                   # Controladores API REST
├── 🔧 appsettings.json              # Configuración de la aplicación
└── 🔧 .gitignore                    # Archivos ignorados por Git
```

## ✨ Características Principales

### 🎯 Funcionalidades
- ✅ **Gestión de Préstamos**: Solicitud y validación de préstamos de libros y revistas
- ✅ **Validaciones de Negocio**: Implementación completa de todas las reglas especificadas
- ✅ **API REST**: Endpoints para operaciones CRUD
- ✅ **Arquitectura Limpia**: Separación clara de responsabilidades
- ✅ **Inyección de Dependencias**: Usando AutoFac
- ✅ **Async/Await**: Todos los métodos son asíncronos

### 📋 Reglas de Negocio Implementadas

| Regla | Descripción | Estado |
|-------|-------------|--------|
| 📖 **Libros** | 15 días hábiles si suma dígitos ISBN > 30, sino 10 días | ✅ |
| 📰 **Revistas** | Máximo 2 días, no para fin de semana | ✅ |
| 🔄 **Palíndromos** | ISBN palíndromo solo para uso interno | ✅ |
| 📅 **Domingos** | Si entrega cae domingo, se mueve al siguiente día hábil | ✅ |
| 🔒 **Material Prestado** | Validación de disponibilidad | ✅ |

## 🚀 Tecnologías Utilizadas

- **Framework**: .NET 8
- **ORM**: Entity Framework Core
- **Base de Datos**: MySQL
- **Inyección de Dependencias**: AutoFac
- **Testing**: MSTest / NUnit
- **Mocking**: NSubstitute
- **CI/CD**: Azure DevOps Pipelines
- **Análisis de Código**: SonarQube

## 📦 Instalación y Configuración

### Prerrequisitos
- .NET 8 SDK
- MySQL (LocalDB o instancia completa)
- Visual Studio 2022 o VS Code

### 🔧 Configuración

1. **Clonar el repositorio**
   ```bash
   git clone https://PruebaGrupoExito@dev.azure.com/PruebaGrupoExito/Grupo%20Exito/_git/Grupo%20Exito
   cd "Grupo Exito"
   ```

2. **Configurar la cadena de conexión**

   Editar `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LibrarySystemDb;Trusted_Connection=true;MultipleActiveResultSets=true"
     }
   }
   ```

3. **Restaurar paquetes**
   ```bash
   dotnet restore
   ```

4. **Ejecutar migraciones**
   ```bash
   dotnet ef database update
   ```

5. **Ejecutar la aplicación**
   ```bash
   dotnet run
   ```
## 📡 API Endpoints

### Préstamos
```http
POST /api/loans
Content-Type: application/json

{
  "isbn": "1234567890",
  "userId": "uuid-here",
  "requestedDays": 5
}
```

### Respuesta exitosa
```json
{
  "success": true,
  "message": "Préstamo procesado exitosamente",
  "loan": {
    "id": "uuid-here",
    "isbn": "1234567890",
    "requestDate": "2025-01-20T10:00:00Z",
    "returnDate": "2025-01-27T10:00:00Z",
    "userId": "uuid-here",
    "status": "Activo"
  }
}
```

### Ejemplos de validaciones
```json
// ISBN palíndromo
{
  "success": false,
  "message": "El material con ISBN en palíndromo solo es para uso en la biblioteca"
}

// Revista fin de semana
{
  "success": false,
  "message": "La revista no puede ser prestada para el fin de semana"
}

// Material ya prestado
{
  "success": false,
  "message": "El material no puede ser prestado: Ese ISBN ya está prestado"
}
```

## 🏛️ Patrones de Diseño Implementados

### SOLID Principles
- **S** - Single Responsibility: Cada clase tiene una responsabilidad específica
- **O** - Open/Closed: Extensible sin modificar código existente
- **L** - Liskov Substitution: Interfaces implementadas correctamente
- **I** - Interface Segregation: Interfaces específicas y cohesivas
- **D** - Dependency Inversion: Dependencias inyectadas mediante interfaces

### Patrones Arquitectónicos
- **Repository Pattern**: Abstracción de acceso a datos
- **Service Layer Pattern**: Lógica de negocio separada
- **Dependency Injection**: Inversión de control con AutoFac
- **Extension Methods**: Funcionalidades adicionales (ej: `IsPalindrome()`)

## 📝 Decisiones de Diseño

### ❓ Dudas Asumidas y Documentadas

1. **Días hábiles para libros**: Se interpretó que "no incluye domingos" significa solo excluir domingos, no sábados
2. **Validación de revistas**: Se validó que la fecha de retorno no caiga en fin de semana
3. **ISBN palíndromo**: Se implementó como método de extensión según especificación
4. **Fecha de entrega domingo**: Se mueve automáticamente al lunes siguiente

### 🔧 Configuraciones Técnicas

- **Base de datos**: SQL Server con Entity Framework Code First
- **Manejo de errores**: Try-catch con respuestas estructuradas

## 📞 Contacto

- **Desarrollador**: Sebastian Ramirez Garcia
- **Proyecto**: Sistema de Biblioteca - Prueba Técnica .NET
