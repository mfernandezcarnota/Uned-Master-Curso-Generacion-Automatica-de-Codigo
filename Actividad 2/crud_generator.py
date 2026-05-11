#!/usr/bin/env python3
"""Generador de programas CRUD basados en metadatos."""

# ============================================================
# BLOQUE 1: IMPORTACIONES Y UTILIDADES BASE
# ------------------------------------------------------------
# Este bloque define las importaciones necesarias para:
# - Leer metadatos JSON.
# - Construir código Python dinámicamente.
# - Gestionar rutas de archivos para entrada/salida del generador.
# ============================================================
from __future__ import annotations

import json
from dataclasses import dataclass
from pathlib import Path
from typing import Any, Dict, List


# ============================================================
# BLOQUE 2: MENSAJES DEL GENERADOR (MULTI-IDIOMA)
# ------------------------------------------------------------
# Mensajes del programa generador (no del CRUD generado).
# Sirven para informar al usuario durante el proceso de
# generación y ejecución del flujo principal.
# ============================================================
GENERATOR_MESSAGES = {
    "es": {
        "generator_menu": "Menú Generador: 1) Generar CRUD 2) Mostrar metadata activa 0) Salir",
        "choose_option": "Selecciona una opción",
        "enter_metadata": "Ruta del metadata JSON (Enter para usar prueba_usuarios.json)",
        "enter_output": "Nombre del archivo de salida (Enter para usar crud_app_generado.py)",
        "generated_ok": "CRUD generado correctamente en",
        "metadata_loaded": "Metadata cargada",
        "invalid_option": "Opción inválida",
        "bye": "Fin del generador CRUD.",
    },
    "en": {
        "generator_menu": "Generator Menu: 1) Generate CRUD 2) Show active metadata 0) Exit",
        "choose_option": "Choose an option",
        "enter_metadata": "Metadata JSON path (Enter to use prueba_usuarios.json)",
        "enter_output": "Output filename (Enter to use crud_app_generado.py)",
        "generated_ok": "CRUD successfully generated at",
        "metadata_loaded": "Metadata loaded",
        "invalid_option": "Invalid option",
        "bye": "End of CRUD generator.",
    },
}


# ============================================================
# BLOQUE 3: MODELOS DE METADATOS DEL GENERADOR
# ------------------------------------------------------------
# Estructuras internas para validar y manejar la definición
# de tablas que alimenta al generador.
# ============================================================
@dataclass
class FieldMeta:
    name: str
    type: str
    required: bool = False
    primary_key: bool = False


@dataclass
class TableMeta:
    table_name: str
    fields: List[FieldMeta]


# ============================================================
# BLOQUE 4: CARGA Y VALIDACIÓN DE METADATOS
# ------------------------------------------------------------
# Lee un JSON de entrada y valida su estructura mínima para
# asegurar que se puede generar un programa CRUD correcto.
# ============================================================
def load_metadata(path: Path) -> TableMeta:
    with path.open("r", encoding="utf-8") as fh:
        raw = json.load(fh)

    if "table_name" not in raw or "fields" not in raw:
        raise ValueError("El metadata debe contener 'table_name' y 'fields'.")

    fields = [FieldMeta(**field) for field in raw["fields"]]
    if not fields:
        raise ValueError("El metadata debe incluir al menos un campo.")

    return TableMeta(table_name=raw["table_name"], fields=fields)


# ============================================================
# BLOQUE 5: CLASE GENERADORA DE CÓDIGO CRUD
# ------------------------------------------------------------
# Este es el núcleo del proyecto: no ejecuta directamente un
# CRUD fijo, sino que genera un archivo Python CRUD completo
# a partir de metadatos.
# ============================================================
class CRUDGenerator:
    def __init__(self, metadata: TableMeta, language: str = "es", db_type: str = "sqlite") -> None:
        self.metadata = metadata
        self.language = language if language in GENERATOR_MESSAGES else "es"
        self.db_type = db_type.lower()
        if self.db_type not in {"sqlite", "postgresql"}:
            raise ValueError("db_type debe ser 'sqlite' o 'postgresql'.")

    def _sql_fields_literal(self) -> str:
        """Convierte los campos metadata en una lista literal Python."""
        lines = []
        for f in self.metadata.fields:
            lines.append(
                "        {"
                f"'name': {f.name!r}, "
                f"'type': {f.type!r}, "
                f"'required': {f.required!r}, "
                f"'primary_key': {f.primary_key!r}"
                "}"
            )
        return "[\n" + ",\n".join(lines) + "\n    ]"

    def render(self) -> str:
        """Genera el código fuente completo del programa CRUD."""
        fields_literal = self._sql_fields_literal()
        table_name = self.metadata.table_name

        # Nota: este template es el programa CRUD final generado.
        return f'''#!/usr/bin/env python3
"""CRUD generado automáticamente desde metadatos."""

from __future__ import annotations

import importlib.util
import json
import sqlite3
from pathlib import Path
from typing import Any, Callable, Dict, Optional

try:
    import psycopg2  # type: ignore
except ImportError:  # pragma: no cover
    psycopg2 = None

MESSAGES = {{
    "es": {{
        "connected": "Conexión establecida correctamente.",
        "created": "Registro creado correctamente.",
        "updated": "Registro actualizado correctamente.",
        "deleted": "Registro eliminado correctamente.",
        "not_found": "No se encontró el registro solicitado.",
        "validation_error": "Error de validación",
        "db_error": "Error de base de datos",
        "query_result": "Resultado de la consulta",
        "menu": "Menú CRUD: 1) Alta 2) Baja 3) Consulta 4) Modificación 0) Salir",
        "option": "Selecciona una opción",
        "insert_json": "Introduce el JSON del nuevo registro",
        "update_json": "Introduce el JSON con campos a modificar",
        "key_field": "Nombre del campo clave",
        "key_value": "Valor del campo clave",
        "bye": "Fin del programa CRUD.",
        "invalid_option": "Opción inválida.",
    }},
    "en": {{
        "connected": "Connection established successfully.",
        "created": "Record created successfully.",
        "updated": "Record updated successfully.",
        "deleted": "Record deleted successfully.",
        "not_found": "Requested record was not found.",
        "validation_error": "Validation error",
        "db_error": "Database error",
        "query_result": "Query result",
        "menu": "CRUD Menu: 1) Create 2) Delete 3) Read 4) Update 0) Exit",
        "option": "Select an option",
        "insert_json": "Enter JSON for new record",
        "update_json": "Enter JSON with fields to update",
        "key_field": "Key field name",
        "key_value": "Key field value",
        "bye": "End of CRUD program.",
        "invalid_option": "Invalid option.",
    }},
}}

TABLE_METADATA = {{
    "table_name": {table_name!r},
    "fields": {fields_literal}
}}


def load_custom_validators(custom_path: str) -> Dict[str, Callable[[Any], Optional[str]]]:
    validators: Dict[str, Callable[[Any], Optional[str]]] = {{}}
    path = Path(custom_path)
    if not path.exists():
        return validators

    spec = importlib.util.spec_from_file_location("custom_validations", custom_path)
    if spec is None or spec.loader is None:
        return validators

    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    loaded = getattr(module, "CUSTOM_VALIDATORS", {{}})
    return loaded if isinstance(loaded, dict) else validators


class CRUDService:
    def __init__(self, db_type: str, db_config: Dict[str, Any], language: str = "es", custom_validators=None) -> None:
        self.metadata = TABLE_METADATA
        self.db_type = db_type.lower()
        self.db_config = db_config
        self.language = language if language in MESSAGES else "es"
        self.custom_validators = custom_validators or {{}}
        self.conn = self._connect()
        print(self._msg("connected"))

    def _msg(self, key: str) -> str:
        return MESSAGES[self.language].get(key, key)

    def _connect(self):
        if self.db_type == "sqlite":
            return sqlite3.connect(self.db_config.get("database", "crud_generated.db"))

        if self.db_type == "postgresql":
            if psycopg2 is None:
                raise RuntimeError("psycopg2 no está instalado para usar PostgreSQL.")
            return psycopg2.connect(**self.db_config)

        raise ValueError(f"SGBD no soportado: {{self.db_type}}")

    def _placeholder(self) -> str:
        return "?" if self.db_type == "sqlite" else "%s"

    def initialize_table(self) -> None:
        sql_fields = []
        for field in self.metadata["fields"]:
            field_def = f"{{field['name']}} {{field['type']}}"
            if field.get("primary_key"):
                field_def += " PRIMARY KEY"
            if field.get("required"):
                field_def += " NOT NULL"
            sql_fields.append(field_def)

        sql = f"CREATE TABLE IF NOT EXISTS {{self.metadata['table_name']}} ({{', '.join(sql_fields)}})"
        cur = self.conn.cursor()
        cur.execute(sql)
        self.conn.commit()

    def _validate(self, payload: Dict[str, Any], partial: bool = False) -> None:
        fields = {{f["name"]: f for f in self.metadata["fields"]}}
        for name, meta in fields.items():
            if not partial and meta.get("required") and name not in payload:
                raise ValueError(f"{{self._msg('validation_error')}}: {{name}} es obligatorio")

            if name in payload:
                value = payload[name]
                if value is None and meta.get("required"):
                    raise ValueError(f"{{self._msg('validation_error')}}: {{name}} no puede ser nulo")

                custom = self.custom_validators.get(name)
                if custom:
                    error = custom(value)
                    if error:
                        raise ValueError(f"{{self._msg('validation_error')}}: {{error}}")

    def alta(self, data: Dict[str, Any]) -> None:
        self._validate(data)
        cols = ", ".join(data.keys())
        placeholders = ", ".join([self._placeholder()] * len(data))
        sql = f"INSERT INTO {{self.metadata['table_name']}} ({{cols}}) VALUES ({{placeholders}})"
        cur = self.conn.cursor()
        cur.execute(sql, list(data.values()))
        self.conn.commit()
        print(self._msg("created"))

    def consulta(self, key_field: str, key_value: Any):
        sql = f"SELECT * FROM {{self.metadata['table_name']}} WHERE {{key_field}} = {{self._placeholder()}}"
        cur = self.conn.cursor()
        cur.execute(sql, (key_value,))
        row = cur.fetchone()
        if row is None:
            print(self._msg("not_found"))
            return None

        columns = [d[0] for d in cur.description]
        result = dict(zip(columns, row))
        print(f"{{self._msg('query_result')}}: {{result}}")
        return result

    def modificacion(self, key_field: str, key_value: Any, updates: Dict[str, Any]) -> None:
        self._validate(updates, partial=True)
        set_clause = ", ".join([f"{{k}} = {{self._placeholder()}}" for k in updates.keys()])
        sql = f"UPDATE {{self.metadata['table_name']}} SET {{set_clause}} WHERE {{key_field}} = {{self._placeholder()}}"
        cur = self.conn.cursor()
        cur.execute(sql, list(updates.values()) + [key_value])
        self.conn.commit()
        print(self._msg("updated") if cur.rowcount else self._msg("not_found"))

    def baja(self, key_field: str, key_value: Any) -> None:
        sql = f"DELETE FROM {{self.metadata['table_name']}} WHERE {{key_field}} = {{self._placeholder()}}"
        cur = self.conn.cursor()
        cur.execute(sql, (key_value,))
        self.conn.commit()
        print(self._msg("deleted") if cur.rowcount else self._msg("not_found"))


def parse_json_input(text: str) -> Dict[str, Any]:
    data = json.loads(text)
    if not isinstance(data, dict):
        raise ValueError("La entrada debe ser un objeto JSON.")
    return data


def run_menu(service: CRUDService) -> None:
    while True:
        print("\\n" + service._msg("menu"))
        option = input(f"{{service._msg('option')}}: ").strip()
        try:
            if option == "1":
                payload = parse_json_input(input(f"{{service._msg('insert_json')}}: ").strip())
                service.alta(payload)
            elif option == "2":
                key_field = input(f"{{service._msg('key_field')}}: ").strip()
                key_value = input(f"{{service._msg('key_value')}}: ").strip()
                service.baja(key_field, key_value)
            elif option == "3":
                key_field = input(f"{{service._msg('key_field')}}: ").strip()
                key_value = input(f"{{service._msg('key_value')}}: ").strip()
                service.consulta(key_field, key_value)
            elif option == "4":
                key_field = input(f"{{service._msg('key_field')}}: ").strip()
                key_value = input(f"{{service._msg('key_value')}}: ").strip()
                updates = parse_json_input(input(f"{{service._msg('update_json')}}: ").strip())
                service.modificacion(key_field, key_value, updates)
            elif option == "0":
                print(service._msg("bye"))
                break
            else:
                print(service._msg("invalid_option"))
        except (ValueError, json.JSONDecodeError) as exc:
            print(f"{{service._msg('validation_error')}}: {{exc}}")
        except RuntimeError as exc:
            print(exc)


def main() -> None:
    base_dir = Path(__file__).parent
    service = CRUDService(
        db_type={self.db_type!r},
        db_config={{"database": str(base_dir / "crud_generated.db")}},
        language={self.language!r},
        custom_validators=load_custom_validators(str(base_dir / "custom_validations.py")),
    )
    service.initialize_table()
    run_menu(service)


if __name__ == "__main__":
    main()
'''

    def generate_file(self, output_path: Path) -> Path:
        output_path.write_text(self.render(), encoding="utf-8")
        return output_path


# ============================================================
# BLOQUE 6: MENÚ PRINCIPAL DEL GENERADOR
# ------------------------------------------------------------
# Menú del programa generador. Desde aquí se crea el archivo
# CRUD final a partir del metadata seleccionado.
# ============================================================
def msg(language: str, key: str) -> str:
    return GENERATOR_MESSAGES[language].get(key, key)


def main() -> None:
    selected_language = input("Selecciona idioma del generador (es/en): ").strip().lower()
    language = selected_language if selected_language in {"es", "en"} else "es"
    base_dir = Path(__file__).parent
    active_metadata_path = base_dir / "prueba_usuarios.json"

    while True:
        print("\n" + msg(language, "generator_menu"))
        option = input(f"{msg(language, 'choose_option')}: ").strip()

        if option == "1":
            input_metadata = input(f"{msg(language, 'enter_metadata')}: ").strip()
            input_output = input(f"{msg(language, 'enter_output')}: ").strip()
            input_db_type = input("SGBD del CRUD generado (sqlite/postgresql): ").strip().lower()
            db_type = input_db_type if input_db_type in {"sqlite", "postgresql"} else "sqlite"

            metadata_path = Path(input_metadata) if input_metadata else active_metadata_path
            output_path = Path(input_output) if input_output else (base_dir / "crud_app_generado.py")

            metadata = load_metadata(metadata_path)
            generator = CRUDGenerator(metadata=metadata, language=language, db_type=db_type)
            generated = generator.generate_file(output_path)
            print(f"{msg(language, 'generated_ok')} {generated}")

        elif option == "2":
            metadata = load_metadata(active_metadata_path)
            print(f"{msg(language, 'metadata_loaded')}: {metadata}")

        elif option == "0":
            print(msg(language, "bye"))
            break

        else:
            print(msg(language, "invalid_option"))


if __name__ == "__main__":
    main()
