# XLSConfigBackupService

XLSConfigBackupService es un servicio de Windows desarrollado por [victorsosaMx](https://github.com/victorsosaMx) que realiza respaldos diarios de archivos de configuración (web.config, app.config, appsettings.json) de varias aplicaciones web en un servidor Windows.

## Descripción

Este servicio de Windows está diseñado para ejecutar respaldos automáticos de archivos de configuración críticos. Los respaldos se organizan en directorios basados en la aplicación, el mes y el día actuales, siguiendo la estructura `/RESPALDOS/Aplicacion/Mes/Dia`.

## Características

- Respaldos automáticos diarios.
- Estructura de directorios organizada por aplicación, mes y día.
- Soporte para múltiples aplicaciones web.

## Instalación

Para instalar y configurar el servicio, sigue estos pasos:

1. **Configuración del archivo `app.config`**:
   - Agrega las rutas de las aplicaciones y sus respectivos archivos de configuración en la sección `appSettings`.

2. **Creación del origen de EventLog**:
   - Asegúrate de que el origen de `EventLog` necesario para el servicio esté creado y configurado correctamente.

3. **Instalación del servicio**:
   - Utiliza `InstallUtil.exe` para instalar el servicio en el sistema.

4. **Inicio del servicio**:
   - Inicia el servicio desde la consola de comandos o desde el panel de servicios de Windows.

## Uso

Una vez instalado y en ejecución, el servicio realizará respaldos automáticos de los archivos de configuración diariamente. Los respaldos se almacenarán en la ruta especificada, organizada por aplicación, mes y día.

## Contribución

Las contribuciones son bienvenidas. Si deseas contribuir, por favor, sigue estos pasos:

1. Haz un fork del repositorio.
2. Crea una nueva rama (`git checkout -b feature/nueva-funcionalidad`).
3. Realiza tus cambios y haz commit (`git commit -am 'Agrega nueva funcionalidad'`).
4. Haz push a la rama (`git push origin feature/nueva-funcionalidad`).
5. Abre un Pull Request.

## Licencia

Este proyecto está bajo la licencia MIT. Consulta el archivo [LICENSE](LICENSE) para más detalles.

## Contacto

Para cualquier pregunta o sugerencia, puedes contactar al desarrollador en [victorsosaMx](https://github.com/victorsosaMx).
