ACTIVIDAD 3 - TRANSFORMADOR M -> PYTHON (CRUD)
===============================================

1) PROPÓSITO DEL CÓDIGO
-----------------------
Este desarrollo implementa un generador/transformador de información.
Recibe sentencias en lenguaje declarativo M (Power Query) junto con un
fichero de metadatos, y produce un programa equivalente en lenguaje de
alto nivel (Python + pandas).

El objetivo académico es demostrar una transformación de programas:
- Entrada: fórmulas M de tratamiento de datos.
- Metainformación: origen de datos y destino de salida.
- Salida: código fuente Python ejecutable para procesar los datos.

2) ESTRUCTURA DE LA CARPETA
---------------------------
- transformador_m_a_python.m
  Programa principal en lenguaje M (declarativo).

- metadatos_crud.json
  Metadatos de ejemplo: alias de fuentes y archivo de salida.

- pruebas/crud_01_create.mtxt
  Sentencias M de prueba para un escenario CREATE.

- pruebas/crud_02_read.mtxt
  Sentencias M de prueba para un escenario READ.

- pruebas/crud_03_update_delete.mtxt
  Sentencias M de prueba para escenario UPDATE + DELETE.

- salidas_esperadas/*.py
  Resultados Python esperados para cada prueba.

- salidas_esperadas/*.java
  Resultados Java esperados para cada prueba.

3) CÓMO USAR EL PROGRAMA PASO A PASO (DETALLADO)
------------------------------------------------
Objetivo práctico:
  Tomar un fichero con sentencias M + metadatos y generar un script Python
  listo para ejecutar.

Requisitos previos:
  - Power BI Desktop o Excel con Power Query.
  - Python 3.10+ recomendado.
  - Librería pandas instalada:
      pip install pandas

Paso 1. Preparar las entradas:
  1.1) Sentencias M:
       - Usa como referencia los ficheros dentro de "pruebas/".
       - Debe haber una sentencia por línea.
       - Operaciones soportadas actualmente:
         * Csv.Document("alias")
         * Table.SelectRows("col","op","valor")
         * Table.AddColumn("nueva","colA","op","colB_o_literal")
         * Table.SelectColumns({"c1","c2"})
         * Table.Sort("col","ASC|DESC")

  1.2) Metadatos JSON:
       - Usa "metadatos_crud.json" como plantilla.
       - "fuentes": relaciona alias con rutas reales de datos.
       - "destino.archivo_salida": ruta del CSV final.

Paso 2. Generar el código Python desde M:
  2.1) Abre Power BI Desktop (recomendado) o Excel.
  2.2) Entra en Power Query -> Consulta en blanco.
  2.3) Abre "transformador_m_a_python.m" y copia su contenido.
  2.4) Pega el código en el editor avanzado.
  2.5) Sustituye:
       - SentenciasDemo por tu colección de sentencias M.
       - MetadataDemo por tus metadatos.
       - RutaBase por la ruta absoluta real del proyecto en tu equipo
         (ejemplo: C:/Users/TuUsuario/Uned-Master-Curso-...).
  2.6) Ejecuta la consulta: el resultado será un texto largo con el script
       Python generado.

Paso 3. Guardar y ejecutar el script generado:
  3.1) Copia el texto resultante a un fichero, por ejemplo:
       transformacion_generada.py
  3.2) Ejecuta el script:
       python transformacion_generada.py
  3.3) Revisa:
       - La consola (muestra resultado.head()).
       - El CSV de salida indicado en metadatos.

Paso 4. Verificar con pruebas CRUD incluidas:
  - CREATE: pruebas/crud_01_create.mtxt
  - READ:   pruebas/crud_02_read.mtxt
  - UPDATE/DELETE lógico (filtros + cálculos): pruebas/crud_03_update_delete.mtxt
  - Compara la salida generada con los ejemplos de "salidas_esperadas/".
  - También puedes validar salida Java cambiando en metadatos:
      "opciones": { "lenguaje_salida": "java" }
    y comparando con los ficheros .java de salidas_esperadas.

4) EXPLICACIÓN SENCILLA DE CADA BLOQUE/SECCIÓN DEL CÓDIGO
----------------------------------------------------------
BLOQUE 1: Configuración y propósito
  Define el objetivo general del transformador y su alcance.

BLOQUE 2: Funciones de apoyo
  Incluye utilidades para limpieza de texto, creación de listas Python e
  indentación; facilita que el generador produzca código legible.

BLOQUE 3: Parser mínimo de sentencias M
  Analiza cada sentencia M textual y la convierte en una estructura común
  (tipo de operación + argumentos).

BLOQUE 4: Reglas de traducción M -> Python
  Implementa la equivalencia semántica:
  - Csv.Document          -> pd.read_csv(...)
  - Table.SelectRows      -> filtro de dataframe
  - Table.AddColumn       -> cálculo de columna
  - Table.SelectColumns   -> proyección de columnas
  - Table.Sort            -> sort_values

BLOQUE 5: Motor principal de generación
  Encadena parser + traductor para generar un programa Python completo,
  incluyendo cabecera, función principal y guardado de salida.

BLOQUE 6: Ejemplo de uso
  Muestra datos de prueba internos para validar rápidamente el generador.

5) ESPECIFICACIÓN DE COMENTARIOS Y BLOQUES
------------------------------------------
El código está dividido en bloques numerados y cada bloque contiene:
- Un encabezado comentado que identifica el bloque.
- Explicación de su papel en el programa completo.
- Comentarios internos detallando funciones y lógica.

Esto convierte el código en una especificación trazable del programa,
facilitando mantenimiento, evaluación y extensión.

6) DISEÑO DE METAINFORMACIÓN (DSL EXTERNO EN JSON)
--------------------------------------------------
Estructura mínima propuesta:
{
  "fuentes": {
    "alias_fuente": "ruta/fichero_origen"
  },
  "destino": {
    "archivo_salida": "ruta/fichero_salida"
  },
  "opciones": {
    "lenguaje_salida": "python"
  }
}

Notas:
- Este diseño puede extenderse para múltiples fuentes y múltiples salidas.
- La clave "lenguaje_salida" prepara el sistema para evolución a salida
  múltiple (Python/Java/C#), como opción avanzada del enunciado.

7) ALCANCE Y LIMITACIONES ACTUALES
----------------------------------
- Se implementa un subconjunto de sentencias M frecuentes en ETL.
- Se prioriza claridad y trazabilidad sobre cobertura completa del lenguaje M.
- La extensión natural es ampliar ParsearSentencia y TraducirPaso con nuevas
  reglas.

8) ¿QUÉ HERRAMIENTA ES MEJOR PARA VER LOS RESULTADOS?
------------------------------------------------------
Recomendación principal (mejor equilibrio para esta actividad):
  - Visual Studio Code + extensión de Python + Jupyter.
  ¿Por qué?
  - Permite ejecutar el .py generado paso a paso.
  - Puedes inspeccionar el DataFrame en tablas de forma cómoda.
  - Facilita comparar rápidamente los CSV de salida.

Recomendación complementaria:
  - Power BI Desktop para la fase de generación (código M) y validación de
    que las fuentes están bien definidas.

Resumen práctico:
  - Para generar el script desde M -> Power BI Desktop.
  - Para analizar el resultado de ejecución del script -> VS Code/Jupyter.
