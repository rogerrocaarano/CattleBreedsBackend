# CattleBreedsBackend

## Descripción
CattleBreedsBackend es una aplicación Web API desarrollada en .NET 9.0 que permite la clasificación de razas de ganado a partir de imágenes.

## Características
- API RESTful para subir imágenes y gestionar trabajos de predicción.
- Hangfire para procesamiento de tareas en segundo plano.
- Entity Framework Core con SQLite para almacenamiento local.
- Documentación de la API con Swagger (Swashbuckle).
- Dashboard de Hangfire para monitorizar trabajos.
- Almacenamiento de archivos en la carpeta `uploads/`.

## Requisitos
- .NET 9.0 SDK

## Configuración
- Las cadenas de conexión y ajustes están en `CattleBreedsApi/appsettings.json` y `CattleBreedsApi/appsettings.Development.json`.
- Por defecto, se utiliza SQLite con la base de datos `ApiDb.db` en `CattleBreedsApi/Data/`.

## Migraciones
Para aplicar o generar migraciones de Entity Framework Core:
```powershell
cd CattleBreedsApi
dotnet ef database update
```
Si se modifica el modelo:
```powershell
dotnet ef migrations add NombreDeLaMigracion
```

## Ejecución
Para ejecutar la API:
```powershell
cd CattleBreedsApi
dotnet run
```
La aplicación estará disponible en `http://localhost:5000`.

### Dashboard Hangfire
Acceder a [https://localhost:5001/hangfire](https://localhost:5000/hangfire) para monitorizar los trabajos en segundo plano.

## Uso de la API
- `POST /Predictions`: subir una imagen y crear un trabajo de predicción.
- `GET /Predictions/{jobId}`: consultar el estado y resultado de un trabajo.
- `GET /Predictions/image/{imageId}`: obtener la imagen subida.

## Documentación
La documentación Swagger está disponible en `https://localhost:5000/swagger`.

## Licencia
Este proyecto está bajo la licencia MIT.
