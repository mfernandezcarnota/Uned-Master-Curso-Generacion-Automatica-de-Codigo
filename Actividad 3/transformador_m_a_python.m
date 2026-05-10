// ============================================================================
// BLOQUE 1: CONFIGURACION GENERAL Y PROPOSITO DEL PROGRAMA
// ----------------------------------------------------------------------------
// Este archivo implementa un generador/transformador escrito en lenguaje M
// (Power Query) que recibe:
//   1) Una colección de sentencias M (lista de pasos de transformación).
//   2) Un registro de metadatos con información de origen y destino.
// y devuelve:
//   - Un texto con código Python (pandas) equivalente a las transformaciones.
//
// El objetivo es demostrar cómo modelar una transformación de programas:
// M (entrada declarativa) -> Python (salida de alto nivel imperativa).
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
    // BLOQUE 5: MOTOR PRINCIPAL DE GENERACION
    // ------------------------------------------------------------------------
    // Entrada esperada:
    //   sentenciasM : list  -> colección de sentencias M (texto)
    //   metadata    : record -> [fuentes = record, destino = record]
    //
    // Salida:
    //   text con el programa Python completo.
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

    // ========================================================================
    // BLOQUE 6: EJEMPLO DE USO (PRUEBA RAPIDA DENTRO DE POWER QUERY)
    // ------------------------------------------------------------------------
    // Este bloque crea una entrada de ejemplo y devuelve el código Python
    // generado. En un escenario real, sentencias y metadatos se cargarían
    // desde ficheros externos (JSON, TXT, tabla de configuración, etc.).
    // ========================================================================
    SentenciasDemo = {
        "Csv.Document(\"ventas_csv\")",
        "Table.SelectRows(\"cantidad\",\">\",\"10\")",
        "Table.AddColumn(\"importe_total\",\"cantidad\",\"*\",\"precio\")",
        "Table.SelectColumns({\"producto\",\"cantidad\",\"importe_total\"})",
        "Table.Sort(\"importe_total\",\"DESC\")"
    },

    MetadataDemo = [
        fuentes = [ventas_csv = "./datos/ventas.csv"],
        destino = [archivo_salida = "./salida/ventas_transformadas.csv"]
    ],

    CodigoPythonGenerado = GenerarPython(SentenciasDemo, MetadataDemo)
in
    CodigoPythonGenerado
