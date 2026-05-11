"""Script puente para materializar en disco el resultado textual de Power Query.

Power Query (M) puede generar texto con el código fuente, pero normalmente no
escribe archivos arbitrarios en disco desde la consulta. Este script cubre ese
paso manual: toma el texto copiado y lo guarda como .py.
"""

from __future__ import annotations

import argparse
import subprocess
import sys
from pathlib import Path


# ============================================================================
# BLOQUE 1: LECTURA DEL TEXTO GENERADO EN POWER QUERY
# ----------------------------------------------------------------------------
# Lee el fichero de texto plano que contiene el script generado en la salida de
# la consulta M (por ejemplo, "salida_power_query.txt").
# ============================================================================
def leer_salida_power_query(ruta_entrada: Path) -> str:
    """Lee el contenido de texto exportado/copiado desde Power Query."""
    if not ruta_entrada.exists():
        raise FileNotFoundError(f"No existe el fichero de entrada: {ruta_entrada}")
    return ruta_entrada.read_text(encoding="utf-8")


# ============================================================================
# BLOQUE 2: ESCRITURA DEL SCRIPT PYTHON
# ----------------------------------------------------------------------------
# Convierte el texto leído en un archivo .py listo para ejecutar.
# ============================================================================
def guardar_script_python(contenido: str, ruta_salida: Path) -> Path:
    """Guarda el contenido como script Python en la ruta indicada."""
    ruta_salida.write_text(contenido + ("" if contenido.endswith("\n") else "\n"), encoding="utf-8")
    return ruta_salida


# ============================================================================
# BLOQUE 3: INTEGRACIÓN OPCIONAL CON MEJORA POR IA
# ----------------------------------------------------------------------------
# Si el usuario activa --mejorar, se invoca el script mejorar_con_ia.py sobre
# el archivo recién creado para obtener una versión mejorada.
# ============================================================================
def ejecutar_mejora_ia(ruta_script: Path) -> None:
    """Ejecuta mejorar_con_ia.py para mejorar automáticamente el script."""
    script_mejora = Path(__file__).with_name("mejorar_con_ia.py")
    if not script_mejora.exists():
        raise FileNotFoundError(
            f"No se encuentra el script de mejora: {script_mejora}"
        )

    subprocess.run(
        [sys.executable, str(script_mejora), str(ruta_script)],
        check=True,
    )


# ============================================================================
# BLOQUE 4: CLI PRINCIPAL
# ----------------------------------------------------------------------------
# Orquesta todo el pipeline local: lectura de texto, escritura .py y mejora
# opcional con IA.
# ============================================================================
def main() -> None:
    """Punto de entrada de la automatización del pipeline."""
    parser = argparse.ArgumentParser(
        description=(
            "Convierte el texto generado por Power Query en un .py y, "
            "opcionalmente, lo mejora con IA."
        )
    )
    parser.add_argument(
        "--entrada",
        default="salida_power_query.txt",
        help="Ruta del fichero de texto exportado/copiado desde Power Query.",
    )
    parser.add_argument(
        "--salida",
        default="transformacion_generada.py",
        help="Nombre/ruta del script Python de salida.",
    )
    parser.add_argument(
        "--mejorar",
        action="store_true",
        help="Si se indica, llama a mejorar_con_ia.py sobre el .py generado.",
    )
    args = parser.parse_args()

    ruta_entrada = Path(args.entrada).expanduser().resolve()
    ruta_salida = Path(args.salida).expanduser().resolve()

    contenido = leer_salida_power_query(ruta_entrada)
    generado = guardar_script_python(contenido, ruta_salida)

    print(f"Script Python generado en: {generado}")

    if args.mejorar:
        ejecutar_mejora_ia(generado)


if __name__ == "__main__":
    main()
