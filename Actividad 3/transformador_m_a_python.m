// ============================================================================
// BLOQUE 1: CONFIGURACION GENERAL Y PROPOSITO DEL PROGRAMA
// ----------------------------------------------------------------------------
// Este archivo implementa un generador/transformador escrito en lenguaje M
// (Power Query) que recibe:
//   1) Una colección de sentencias M (lista de pasos de transformación).
//   2) Un registro de metadatos con información de origen y destino.
// y devuelve:
//   - Un texto con código en lenguaje de alto nivel (Python o Java).
//
// El objetivo es demostrar cómo modelar una transformación de programas:
// M (entrada declarativa) -> Python/Java (salida de alto nivel).
// ============================================================================
let
    // ========================================================================
    // BLOQUE 2: FUNCIONES DE APOYO PARA TEXTO Y TABLAS
    // ------------------------------------------------------------------------
    // Este bloque contiene utilidades para:
    // - Limpiar comillas en texto.
    // - Formatear listas de columnas para código Python.
    // - Producir líneas de código con indentación coherente.
    // ========================================================================
    QuitarComillas = (txt as text) as text =>
        if Text.StartsWith(txt, "\"") and Text.EndsWith(txt, "\"") then
            Text.Middle(txt, 1, Text.Length(txt) - 2)
        else
            txt,

    ListaPython = (columnas as list) as text =>
        "[" & Text.Combine(List.Transform(columnas, each "\"" & _ & "\""), ", ") & "]",

    Indentar = (linea as text, nivel as number) as text =>
        Text.Repeat("    ", nivel) & linea,

    // ========================================================================
    // BLOQUE 3: PARSER MINIMO DE SENTENCIAS M (DSL DE ENTRADA)
    // ------------------------------------------------------------------------
    // Este parser interpreta un subconjunto práctico de pasos M para ETL:
    // - Csv.Document("alias")
    // - Table.SelectRows("columna", "operador", "valor")
    // - Table.AddColumn("nueva", "colA", "op", "colB_o_literal")
    // - Table.SelectColumns({"c1","c2",...})
    // - Table.Sort("col", "ASC|DESC")
    //
    // La salida del parser es una lista de registros tipados:
    //   [Tipo = "SelectRows", Args = {...}]
    // ========================================================================
    ParsearSentencia = (sentencia as text) as record =>
        let
            limpia = Text.Trim(sentencia),
            esCsv = Text.StartsWith(limpia, "Csv.Document("),
            esFiltro = Text.StartsWith(limpia, "Table.SelectRows("),
            esAdd = Text.StartsWith(limpia, "Table.AddColumn("),
            esSelectCols = Text.StartsWith(limpia, "Table.SelectColumns("),
            esSort = Text.StartsWith(limpia, "Table.Sort("),

            contenido = (s as text) as text => Text.BetweenDelimiters(s, "(", ")", 0, 0),

            argsCsv = if esCsv then {QuitarComillas(Text.Trim(contenido(limpia)))} else {},
            argsFiltro = if esFiltro then List.Transform(Text.Split(contenido(limpia), ","), each QuitarComillas(Text.Trim(_))) else {},
            argsAdd = if esAdd then List.Transform(Text.Split(contenido(limpia), ","), each QuitarComillas(Text.Trim(_))) else {},
            argsSelectCols = if esSelectCols then
                let
                    bruto = Text.BetweenDelimiters(limpia, "{", "}", 0, 0),
                    cols = List.Transform(Text.Split(bruto, ","), each QuitarComillas(Text.Trim(_)))
                in
                    cols
            else
                {},
            argsSort = if esSort then List.Transform(Text.Split(contenido(limpia), ","), each QuitarComillas(Text.Trim(_))) else {},

            resultado =
                if esCsv then [Tipo = "CsvDocument", Args = argsCsv]
                else if esFiltro then [Tipo = "SelectRows", Args = argsFiltro]
                else if esAdd then [Tipo = "AddColumn", Args = argsAdd]
                else if esSelectCols then [Tipo = "SelectColumns", Args = argsSelectCols]
                else if esSort then [Tipo = "Sort", Args = argsSort]
                else [Tipo = "NoSoportado", Args = {limpia}]
        in
            resultado,

    // ========================================================================
    // BLOQUE 4: REGLAS DE TRADUCCION M -> PYTHON (PANDAS)
    // ------------------------------------------------------------------------
    // Este bloque implementa la semántica de transformación por cada tipo de
    // sentencia reconocida por el parser.
    //
    // Recibe un paso parseado y devuelve 1 o varias líneas Python.
    // ========================================================================
    TraducirPaso = (paso as record, metadata as record) as list =>
        let
            tipo = paso[Tipo],
            args = paso[Args],

            lineas =
                if tipo = "CsvDocument" then
                    let
                        alias = args{0},
                        ruta = Record.FieldOrDefault(metadata[fuentes], alias, "./datos.csv")
                    in
                        {
                            "# Carga del dataset principal desde metadatos",
                            "df = pd.read_csv(r\"" & ruta & "\")"
                        }

                else if tipo = "SelectRows" then
                    let
                        col = args{0},
                        op = args{1},
                        valor = args{2},
                        valorPy = if Value.Is(Value.FromText(valor), type number) then valor else "\"" & valor & "\"",
                        expr =
                            if op = ">" then "df[\"" & col & "\"] > " & Text.From(valorPy)
                            else if op = "<" then "df[\"" & col & "\"] < " & Text.From(valorPy)
                            else if op = ">=" then "df[\"" & col & "\"] >= " & Text.From(valorPy)
                            else if op = "<=" then "df[\"" & col & "\"] <= " & Text.From(valorPy)
                            else if op = "==" then "df[\"" & col & "\"] == " & Text.From(valorPy)
                            else "df[\"" & col & "\"] == " & Text.From(valorPy)
                    in
                        {
                            "# Filtro de filas",
                            "df = df[" & expr & "]"
                        }

                else if tipo = "AddColumn" then
                    let
                        nuevaCol = args{0},
                        izq = args{1},
                        operador = args{2},
                        der = args{3},
                        derEsNumero = Value.Is(Value.FromText(der), type number),
                        derExpr = if derEsNumero then der else "df[\"" & der & "\"]",
                        expr =
                            if operador = "+" then "df[\"" & izq & "\"] + " & derExpr
                            else if operador = "-" then "df[\"" & izq & "\"] - " & derExpr
                            else if operador = "*" then "df[\"" & izq & "\"] * " & derExpr
                            else if operador = "/" then "df[\"" & izq & "\"] / " & derExpr
                            else "df[\"" & izq & "\"]"
                    in
                        {
                            "# Creación de columna calculada",
                            "df[\"" & nuevaCol & "\"] = " & expr
                        }

                else if tipo = "SelectColumns" then
                    {
                        "# Proyección de columnas",
                        "df = df[" & ListaPython(args) & "]"
                    }

                else if tipo = "Sort" then
                    let
                        col = args{0},
                        orden = if List.Count(args) > 1 then Text.Upper(args{1}) else "ASC",
                        asc = if orden = "DESC" then "False" else "True"
                    in
                        {
                            "# Ordenación",
                            "df = df.sort_values(by=\"" & col & "\", ascending=" & asc & ")"
                        }

                else
                    {
                        "# AVISO: paso no soportado, se deja comentario para revisión manual",
                        "# " & Text.Combine(args, " ")
                    }
        in
            lineas,

    // ========================================================================
    // BLOQUE 4b: REGLAS DE TRADUCCION M -> JAVA (APACHE COMMONS CSV)
    // ------------------------------------------------------------------------
    // Traduce cada paso parseado a líneas Java equivalentes usando:
    // - CSVFormat + CSVRecord para carga de datos.
    // - Streams/Collectors para filtro.
    // - Map<String, String> para filas transformadas.
    // ========================================================================
    TraducirPasoJava = (paso as record, metadata as record) as list =>
        let
            tipo = paso[Tipo],
            args = paso[Args],
            lineas =
                if tipo = "CsvDocument" then
                    let
                        alias = args{0},
                        ruta = Record.FieldOrDefault(metadata[fuentes], alias, "./datos.csv")
                    in
                        {
                            "// Carga del dataset principal desde metadatos",
                            "try (Reader in = new FileReader(\"" & ruta & "\")) {",
                            "    Iterable<CSVRecord> records = CSVFormat.DEFAULT.withFirstRecordAsHeader().parse(in);",
                            "    rows = new ArrayList<>();",
                            "    for (CSVRecord record : records) {",
                            "        rows.add(new HashMap<>(record.toMap()));",
                            "    }",
                            "}"
                        }
                else if tipo = "SelectRows" then
                    let
                        col = args{0},
                        op = args{1},
                        valor = args{2},
                        valorNum = Value.Is(Value.FromText(valor), type number),
                        expr =
                            if valorNum then
                                "Double.parseDouble(r.getOrDefault(\"" & col & "\", \"0\")) " & op & " " & valor
                            else
                                "r.getOrDefault(\"" & col & "\", \"\").equals(\"" & valor & "\")"
                    in
                        {
                            "// Filtro de filas",
                            "rows = rows.stream().filter(r -> " & expr & ").collect(Collectors.toList());"
                        }
                else if tipo = "AddColumn" then
                    let
                        nuevaCol = args{0},
                        izq = args{1},
                        operador = args{2},
                        der = args{3},
                        derEsNumero = Value.Is(Value.FromText(der), type number),
                        derExpr = if derEsNumero then der else "Double.parseDouble(r.getOrDefault(\"" & der & "\", \"0\"))",
                        expr =
                            if operador = "+" then "base + " & derExpr
                            else if operador = "-" then "base - " & derExpr
                            else if operador = "*" then "base * " & derExpr
                            else if operador = "/" then "base / " & derExpr
                            else "base"
                    in
                        {
                            "// Creación de columna calculada",
                            "for (Map<String, String> r : rows) {",
                            "    double base = Double.parseDouble(r.getOrDefault(\"" & izq & "\", \"0\"));",
                            "    double calc = " & expr & ";",
                            "    r.put(\"" & nuevaCol & "\", String.valueOf(calc));",
                            "}"
                        }
                else if tipo = "SelectColumns" then
                    let
                        campos = List.Transform(args, each "\"" & _ & "\"")
                    in
                        {
                            "// Proyección de columnas",
                            "List<String> columnas = Arrays.asList(" & Text.Combine(campos, ", ") & ");",
                            "rows = rows.stream().map(r -> {",
                            "    Map<String, String> n = new LinkedHashMap<>();",
                            "    for (String c : columnas) { n.put(c, r.get(c)); }",
                            "    return n;",
                            "}).collect(Collectors.toList());"
                        }
                else if tipo = "Sort" then
                    let
                        col = args{0},
                        orden = if List.Count(args) > 1 then Text.Upper(args{1}) else "ASC",
                        comparator =
                            if orden = "DESC" then
                                "Comparator.comparing((Map<String, String> r) -> r.getOrDefault(\"" & col & "\", \"\")).reversed()"
                            else
                                "Comparator.comparing((Map<String, String> r) -> r.getOrDefault(\"" & col & "\", \"\"))"
                    in
                        {
                            "// Ordenación",
                            "rows = rows.stream().sorted(" & comparator & ").collect(Collectors.toList());"
                        }
                else
                    {
                        "// AVISO: paso no soportado",
                        "// " & Text.Combine(args, " ")
                    }
        in
            lineas,

    // ========================================================================
    // BLOQUE 5: MOTORES DE GENERACION (PYTHON/JAVA + SELECTOR)
    // ------------------------------------------------------------------------
    // Incluye:
    // - GenerarPython: motor original de salida Python.
    // - GenerarJava: nuevo motor de salida Java.
    // - GenerarCodigo: selector de motor según metadata[opciones][lenguaje_salida].
    // ========================================================================
    GenerarPython = (sentenciasM as list, metadata as record) as text =>
        let
            pasos = List.Transform(sentenciasM, each ParsearSentencia(_)),
            cuerpo = List.Combine(List.Transform(pasos, each TraducirPaso(_, metadata))),

            cabecera = {
                "# ============================================================",
                "# ARCHIVO GENERADO AUTOMATICAMENTE DESDE M (Power Query)",
                "# Lenguaje de salida: Python + pandas",
                "# ============================================================",
                "import pandas as pd",
                "",
                "def ejecutar_transformacion():"
            },

            cuerpoIndentado = List.Transform(cuerpo, each Indentar(_, 1)),

            destinoRuta = Record.FieldOrDefault(metadata[destino], "archivo_salida", "./salida.csv"),
            cola = {
                Indentar("# Persistencia del resultado", 1),
                Indentar("df.to_csv(r\"" & destinoRuta & "\", index=False)", 1),
                Indentar("return df", 1),
                "",
                "if __name__ == '__main__':",
                Indentar("resultado = ejecutar_transformacion()", 1),
                Indentar("print(resultado.head())", 1)
            },

            programa = Text.Combine(cabecera & cuerpoIndentado & cola, "#(lf)")
        in
            programa,

    GenerarJava = (sentenciasM as list, metadata as record) as text =>
        let
            pasos = List.Transform(sentenciasM, each ParsearSentencia(_)),
            cuerpo = List.Combine(List.Transform(pasos, each TraducirPasoJava(_, metadata))),
            destinoRuta = Record.FieldOrDefault(metadata[destino], "archivo_salida", "./salida.csv"),
            cabecera = {
                "// ============================================================",
                "// ARCHIVO GENERADO AUTOMATICAMENTE DESDE M (Power Query)",
                "// Lenguaje de salida: Java + Apache Commons CSV",
                "// ============================================================",
                "import java.io.*;",
                "import java.nio.file.*;",
                "import java.util.*;",
                "import java.util.stream.*;",
                "import org.apache.commons.csv.*;",
                "",
                "public class Main {",
                "    public static List<Map<String, String>> ejecutarTransformacion() throws Exception {",
                "        List<Map<String, String>> rows = new ArrayList<>();"
            },
            cuerpoIndentado = List.Transform(cuerpo, each Indentar(_, 2)),
            cola = {
                Indentar("// Persistencia del resultado (CSV simple)", 2),
                Indentar("try (BufferedWriter writer = Files.newBufferedWriter(Paths.get(\"" & destinoRuta & "\"))) {", 2),
                Indentar("if (!rows.isEmpty()) {", 3),
                Indentar("List<String> headers = new ArrayList<>(rows.get(0).keySet());", 4),
                Indentar("writer.write(String.join(\",\", headers));", 4),
                Indentar("writer.newLine();", 4),
                Indentar("for (Map<String, String> row : rows) {", 4),
                Indentar("List<String> values = headers.stream().map(h -> row.getOrDefault(h, \"\")).collect(Collectors.toList());", 5),
                Indentar("writer.write(String.join(\",\", values));", 5),
                Indentar("writer.newLine();", 5),
                Indentar("}", 4),
                Indentar("}", 3),
                Indentar("}", 2),
                Indentar("return rows;", 2),
                "    }",
                "",
                "    public static void main(String[] args) throws Exception {",
                "        List<Map<String, String>> resultado = ejecutarTransformacion();",
                "        System.out.println(\"Filas transformadas: \" + resultado.size());",
                "    }",
                "}"
            },
            programa = Text.Combine(cabecera & cuerpoIndentado & cola, "#(lf)")
        in
            programa,

    GenerarCodigo = (sentenciasM as list, metadata as record) as text =>
        let
            opciones = Record.FieldOrDefault(metadata, "opciones", []),
            lenguaje = Text.Lower(Record.FieldOrDefault(opciones, "lenguaje_salida", "python")),
            salida = if lenguaje = "java" then GenerarJava(sentenciasM, metadata) else GenerarPython(sentenciasM, metadata)
        in
            salida,

    // ========================================================================
    // BLOQUE 6: LECTURA REAL DE ENTRADAS DESDE DISCO + EJEMPLO DE USO
    // ------------------------------------------------------------------------
    // Este bloque carga metadatos JSON y sentencias M desde disco.
    // Nota importante:
    // File.Contents requiere permitir acceso a ficheros locales en Power Query
    // (Opciones -> Seguridad -> Permitir acceso a fuentes de datos locales)
    // o configurar correctamente las credenciales del origen de datos.
    // ========================================================================
    // IMPORTANTE: sustituye esta ruta por la ruta absoluta real donde esté
    // la carpeta "Actividad 3" en tu equipo antes de ejecutar en Power Query.
    RutaBase = "C:/RUTA/ABSOLUTA/DEL/PROYECTO",
    RutaMetadatos = RutaBase & "/Actividad 3/metadatos_crud.json",
    RutaSentencias = RutaBase & "/Actividad 3/pruebas/crud_01_create.mtxt",

    JsonMetadatos = Json.Document(File.Contents(RutaMetadatos)),
    MetadataDemo = [
        fuentes = Record.FieldOrDefault(JsonMetadatos, "fuentes", []),
        destino = Record.FieldOrDefault(JsonMetadatos, "destino", [archivo_salida = "./salida.csv"]),
        opciones = Record.FieldOrDefault(JsonMetadatos, "opciones", [lenguaje_salida = "python"])
    ],

    TextoSentencias = Text.FromBinary(File.Contents(RutaSentencias)),
    LineasSentencias = Text.Split(TextoSentencias, "#(lf)"),
    SentenciasDemo = List.Select(LineasSentencias, each Text.Trim(_) <> ""),

    CodigoGenerado = GenerarCodigo(SentenciasDemo, MetadataDemo)
in
    CodigoGenerado
