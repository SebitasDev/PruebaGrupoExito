# 📚 Sistema de Gestión de Biblioteca (LibrarySystem)

Sistema desarrollado en .NET 8 para la gestión de préstamos de material bibliográfico en un centro documental, implementando arquitectura limpia y principios SOLID.

## 🏗️ Arquitectura del Proyecto

```
LibrarySystem/
├── 📁 LibrarySystem/                 # Proyecto principal
│   ├── 🔷 Program.cs                 # Punto de entrada
│   ├── 🔧 LibrarySystem.csproj       # Configuración del proyecto
│   ├── 🔧 appsettings.json           # Configuración de la aplicación
│   ├── 🔧 appsettings.Development.json
│   ├── 📁 Controllers/               # Controladores API REST
│   ├── 📁 Data/                      # Contexto de Base de Datos
│   │   ├── 🔷 LibraryDbContext.cs    # Contexto principal de Entity Framework
│   │   └── 📁 Migrations/            # Migraciones de base de datos
│   ├── 📁 Extensions/                # Métodos de extensión
│   ├── 📁 Models/                    # Modelos de dominio
│   │   ├── 📁 DTOs/                 # Data Transfer Objects
│   │   ├── 📁 Entities/             # Entidades del dominio
│   │   └── 📁 Enums/                # Enumeraciones
│   ├── 📁 Repository/                # Patrón Repository
│   │   ├── 📁 Interface/            # Interfaces de repositorios
│   │   ├── 🔷 BibliographicMaterialRepository.cs
│   │   └── 🔷 LoanRepository.cs
│   └── 📁 Service/                   # Lógica de negocio
│       ├── 📁 Interface/            # Interfaces de servicios
│       ├── 🔷 BusinessDaysService.cs # Gestión de días hábiles
│       └── 🔷 LoanService.cs        # Lógica de préstamos
├── 📁 Test/                          # Proyecto de pruebas unitarias
│   ├── 🔧 Test.csproj               # Configuración de pruebas
│   ├── 📁 Extensions/               # Pruebas de extensiones
│   ├── 📁 Services/                 # Pruebas de servicios
│   └── 📁 TestData/                 # Datos de prueba
├── 🔧 LibrarySystem.sln             # Archivo de solución
├── 🐳 Dockerfile                    # Configuración Docker
├── 🔧 azure-pipelines.yml           # Pipeline CI/CD Azure DevOps
├── 🔧 .gitignore                    # Archivos ignorados por Git
└── 📄 README.md                     # Este archivo
```

## ✨ Características Principales

### 🎯 Funcionalidades
- ✅ **Gestión de Préstamos**: Solicitud y validación de préstamos de libros y revistas
- ✅ **Validaciones de Negocio**: Implementación completa de todas las reglas especificadas
- ✅ **API REST**: Endpoints para operaciones CRUD
- ✅ **Arquitectura Limpia**: Separación clara de responsabilidades
- ✅ **Inyección de Dependencias**: Usando AutoFac
- ✅ **Async/Await**: Todos los métodos son asíncronos
- ✅ **Docker**: Containerización para despliegue
- ✅ **CI/CD**: Pipeline automatizado

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
- **Containerización**: Docker
- **CI/CD**: Azure DevOps Pipelines

## 📦 Instalación y Configuración

### Prerrequisitos
- .NET 8 SDK
- MySQL (LocalDB o instancia completa)
- Docker (opcional)
- Visual Studio 2022 o VS Code

### 🔧 Configuración Local

1. **Clonar el repositorio**
   ```bash
   git clone https://dev.azure.com/PruebaGrupoExito/Grupo%20Exito/_git/Back-End
   cd LibrarySystem
   ```

2. **Configurar la cadena de conexión**
   
   Editar `LibrarySystem/appsettings.json`:
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
   cd LibrarySystem
   dotnet ef database update
   ```

5. **Ejecutar la aplicación**
   ```bash
   dotnet run
   ```

   La aplicación estará disponible en: `https://localhost:5001`

### 🐳 Ejecución con Docker

1. **Build de la imagen**
   ```bash
   docker build -t library-system .
   ```

2. **Ejecutar el contenedor**
   ```bash
   docker run -p 8080:80 library-system
   ```

   La aplicación estará disponible en: `http://localhost:8080`

## 🧪 Pruebas

### Ejecutar todas las pruebas
```bash
dotnet test
```

### Ejecutar con cobertura de código
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Ejecutar pruebas específicas
```bash
# Solo pruebas de servicios
dotnet test --filter "FullyQualifiedName~Services"

# Solo pruebas de extensiones
dotnet test --filter "FullyQualifiedName~Extensions"
```

### Casos de Prueba Principales
- ✅ Validación de ISBN palíndromo (`1234554321`)
- ✅ Cálculo de días hábiles para libros
- ✅ Restricciones de fin de semana para revistas
- ✅ Validación de material ya prestado
- ✅ Cálculo correcto de fechas de devolución
- ✅ Manejo de excepciones y casos edge

## 📡 API Endpoints

### Préstamos

#### Solicitar Préstamo
```http
POST /api/loans
Content-Type: application/json

{
  "isbn": "9876543210",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "requestedDays": 5
}
```

#### Respuesta exitosa
```json
{
  "success": true,
  "message": "Préstamo procesado exitosamente",
  "loan": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "isbn": "9876543210",
    "requestDate": "2025-01-23T10:00:00Z",
    "returnDate": "2025-01-30T10:00:00Z",
    "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "status": "Activo",
    "materialName": "Libro de Ejemplo",
    "materialType": "Book"
  }
}
```

#### Consultar Préstamos Activos
```http
GET /api/loans/active
```

#### Consultar Préstamo por ID
```http
GET /api/loans/{id}
```

### Ejemplos de Validaciones

#### ISBN Palíndromo
```json
{
  "success": false,
  "message": "El material con ISBN en palíndromo solo es para uso en la biblioteca"
}
```

#### Revista para Fin de Semana
```json
{
  "success": false,
  "message": "La revista no puede ser prestada para el fin de semana"
}
```

#### Material Ya Prestado
```json
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
- **DTO Pattern**: Transferencia de datos entre capas


### Convenciones de Código

- Seguir convenciones de C# y .NET
- Usar async/await para operaciones asíncronas
- Implementar pruebas unitarias para nuevas funcionalidades
- Documentar métodos públicos con XML comments
- Seguir principios SOLID

## 📝 Decisiones de Diseño

### ❓ Dudas Asumidas y Documentadas

1. **Días hábiles para libros**: Se interpretó que "no incluye domingos" significa solo excluir domingos, no sábados
2. **Validación de revistas**: Se validó que la fecha de retorno no caiga en fin de semana
3. **ISBN palíndromo**: Se implementó como método de extensión según especificación
4. **Fecha de entrega domingo**: Se mueve automáticamente al lunes siguiente
5. **Parámetro requestedDays**: Se agregó para permitir flexibilidad en días solicitados

### 🔧 Configuraciones Técnicas

- **Base de datos**: SQL Server con Entity Framework Code First
- **Logging**: Integración con ILogger de .NET
- **Validación**: Data Annotations y validaciones personalizadas
- **Manejo de errores**: Try-catch con respuestas estructuradas
- **Serialización**: System.Text.Json con configuración personalizada

## 🔍 Ejemplos de Uso

### ISBN de Prueba

```csharp
// ISBN palíndromo (no se puede prestar)
"1234554321"

// ISBN con suma alta (15 días máximo)
"9876543210" // suma = 45 > 30

// ISBN con suma baja (10 días máximo)  
"1111111111" // suma = 10 <= 30
```

### Casos de Prueba Manual

1. **Préstamo exitoso de libro**:
   - ISBN: `9876543210`
   - Días solicitados: `10`
   - Resultado esperado: ✅ Aprobado

2. **Revista para fin de semana**:
   - ISBN: `1111111111`
   - Día: Viernes
   - Días solicitados: `2`
   - Resultado esperado: ❌ Rechazado

3. **ISBN palíndromo**:
   - ISBN: `1234554321`
   - Resultado esperado: ❌ Rechazado
