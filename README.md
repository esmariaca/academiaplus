# AcademiaPlus

Proyecto técnico desarrollado con .NET **AcademiaPlus**.

## Tecnologías utilizadas

- [.NET SDK](https://dotnet.microsoft.com/) (8)
- [SQLite](https://www.sqlite.org/index.html)
- [Entity Framework Core](https://learn.microsoft.com/ef/core/)
- C#
- GitLab

---

## Cómo ejecutar el proyecto localmente
=>
### Requisitos previos

1. [.NET SDK](https://dotnet.microsoft.com/download) (v3.1 o superior)  
2. [Visual Studio](https://visualstudio.microsoft.com/) o [Visual Studio Code](https://code.visualstudio.com/)  
3. Git

---

### Clonar el repositorio

git clone https://gitlab.com/mariaca-group/academiaplus.git
cd academiaplus

---
### Restaurar dependencias
dotnet restore

# Configurar y crear la base de datos SQLite
dotnet ef database update --project AcademiaPlus.Data
# Si no tienes instalado el CLI de EF Core:
dotnet tool install --global dotnet-ef

---
## Ejecutar la aplicación
dotnet run

---
## Autor
Maria Cadavid
