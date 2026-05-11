Actividad 2 - Generador CRUD en Python
======================================

1) Propósito del código
-----------------------
Este proyecto implementa un **generador** de programas CRUD (alta, baja, consulta y
modificación), no solo un CRUD fijo. A partir de metadatos JSON de una tabla, el sistema
genera dinámicamente un archivo Python CRUD listo para ejecutar.

Objetivos cubiertos:
- Clase generadora que construye un programa CRUD completo desde metadatos.
- CRUD generado con métodos: alta, baja, consulta y modificación.
- Entrada por metadatos (campos, tipos, obligatoriedad, clave primaria).
- Configuración de idioma de salida (español e inglés).
- Soporte de dos SGBD configurables en el código generado (SQLite y PostgreSQL).
- Validaciones base y tratamiento de errores.
- Extensibilidad por validaciones externas dinámicas (custom code).
- Menú CRUD con opciones separadas para alta, baja, consulta y modificación en el programa generado.


2) Cómo se usa y cómo funciona
------------------------------
Requisitos mínimos:
- Python 3.10+
- Para SQLite no hace falta instalar nada extra.
- Para PostgreSQL: instalar psycopg2 o psycopg2-binary.

Archivos principales:
- crud_generator.py: generador principal.
- custom_validations.py: validaciones extra externas para el CRUD generado.
- prueba_usuarios.json, prueba_productos.json, prueba_pedidos.json: metadatos de prueba.

Flujo de uso recomendado:
1. Ejecutar el generador:
   python3 crud_generator.py
2. En el menú del generador elegir:
   - 1) Generar CRUD
3. Indicar (o dejar por defecto):
   - Metadata JSON de entrada.
   - Nombre del archivo de salida.
4. Ejecutar el archivo generado (por defecto `crud_app_generado.py`):
   python3 crud_app_generado.py

El CRUD generado mostrará su propio menú:
- 1) Alta
- 2) Baja
- 3) Consulta
- 4) Modificación
- 0) Salir


3) Explicación sencilla por bloques/secciones del código
---------------------------------------------------------
Bloque 1: Importaciones y utilidades base
- Importa módulos para JSON, rutas y construcción dinámica del código.

Bloque 2: Mensajes del generador (multi-idioma)
- Define textos de salida del proceso de generación.

Bloque 3: Modelos de metadatos
- Define FieldMeta y TableMeta para estructurar la entrada metadata.

Bloque 4: Carga y validación de metadatos
- Lee JSON y valida estructura mínima antes de generar.

Bloque 5: Clase generadora de código CRUD
- Núcleo del proyecto.
- Genera el código fuente completo del CRUD en un archivo `.py`.
- Incluye en el archivo generado:
  - Clase `CRUDService` con alta, baja, consulta y modificación.
  - Soporte sqlite/postgresql.
  - Carga dinámica de validaciones personalizadas.
  - Menú interactivo CRUD con opciones separadas.

Bloque 6: Menú principal del generador
- Permite al usuario generar nuevos CRUD desde distintos metadatos.
- Muestra metadata activa y controla la salida del programa.


4) Mecanismo de extensibilidad (custom code)
---------------------------------------------
El archivo `custom_validations.py` queda desacoplado del generador.
El programa CRUD generado lo carga dinámicamente en tiempo de ejecución mediante
`load_custom_validators`. De esta forma, el generador puede regenerarse sin perder
las validaciones personalizadas.


5) Archivos de prueba solicitados
---------------------------------
Se incluyen tres archivos de metadatos para probar el generador:
- prueba_usuarios.json
- prueba_productos.json
- prueba_pedidos.json

Cada archivo permite generar un CRUD distinto reutilizando el mismo generador.
