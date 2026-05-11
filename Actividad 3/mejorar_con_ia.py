"""Mejora automática de scripts Python generados desde el transformador M.

Este script representa un segundo paso del pipeline: recibe un script Python
base (generado por Power Query/M) y solicita a Claude una versión mejorada.
"""

from __future__ import annotations

import argparse
import os
from pathlib import Path

from anthropic import Anthropic


# ============================================================================
# BLOQUE 1: UTILIDADES DE ENTRADA/SALIDA
# ----------------------------------------------------------------------------
# Funciones auxiliares para leer el código de entrada y construir el nombre
# del fichero de salida con sufijo _mejorado.py.
# ============================================================================
def leer_codigo(ruta_script: Path) -> str:
    """Lee el código Python que se desea mejorar."""
    return ruta_script.read_text(encoding="utf-8")


def construir_ruta_salida(ruta_script: Path) -> Path:
    """Construye la ruta de salida usando el sufijo _mejorado.py."""
    return ruta_script.with_name(f"{ruta_script.stem}_mejorado.py")


# ============================================================================
# BLOQUE 2: PROMPT Y LLAMADA A CLAUDE
# ----------------------------------------------------------------------------
# Se construye un prompt con requisitos explícitos de mejora y se invoca la
# API de Anthropic usando la variable de entorno ANTHROPIC_API_KEY.
# ============================================================================
def construir_prompt(codigo_fuente: str) -> str:
    """Genera el prompt de mejora que se enviará al LLM."""
    return (
        "Eres un experto en Python y pandas. Mejora el siguiente script "
        "generado automáticamente.\\n\\n"
        "Requisitos obligatorios:\\n"
        "1) Añadir manejo de errores con try/except y mensajes descriptivos.\\n"
        "2) Añadir docstrings a la función ejecutar_transformacion().\\n"
        "3) Validar que el fichero CSV de entrada existe antes de leerlo.\\n"
        "4) Aplicar optimizaciones de pandas cuando sean adecuadas sin "
        "cambiar la lógica funcional.\\n\\n"
        "Devuelve únicamente código Python válido, sin explicaciones fuera "
        "del código.\\n\\n"
        "SCRIPT A MEJORAR:\\n"
        f"{codigo_fuente}"
    )


def mejorar_con_claude(codigo_fuente: str, model: str) -> str:
    """Solicita a Claude una versión mejorada del script."""
    api_key = os.getenv("ANTHROPIC_API_KEY")
    if not api_key:
        raise EnvironmentError(
            "No se encontró ANTHROPIC_API_KEY en variables de entorno."
        )

    client = Anthropic(api_key=api_key)
    prompt = construir_prompt(codigo_fuente)

    response = client.messages.create(
        model=model,
        max_tokens=3500,
        temperature=0,
        messages=[{"role": "user", "content": prompt}],
    )

    partes_texto = [
        bloque.text for bloque in response.content if getattr(bloque, "type", "") == "text"
    ]
    codigo_mejorado = "\n".join(partes_texto).strip()

    if not codigo_mejorado:
        raise ValueError("La respuesta del modelo no contiene código utilizable.")

    return codigo_mejorado


# ============================================================================
# BLOQUE 3: CLI PRINCIPAL
# ----------------------------------------------------------------------------
# Orquesta el flujo de ejecución: parsea argumentos, lee el fichero de
# entrada, llama al LLM y guarda el nuevo fichero mejorado.
# ============================================================================
def main() -> None:
    """Punto de entrada del script de mejora con IA."""
    parser = argparse.ArgumentParser(
        description=(
            "Mejora un script Python generado desde M usando Claude "
            "(Anthropic API)."
        )
    )
    parser.add_argument(
        "script",
        nargs="?",
        default="transformacion_generada.py",
        help="Ruta del script Python a mejorar.",
    )
    parser.add_argument(
        "--model",
        default="claude-3-5-sonnet-latest",
        help="Modelo de Anthropic a utilizar.",
    )
    args = parser.parse_args()

    ruta_script = Path(args.script).expanduser().resolve()
    if not ruta_script.exists():
        raise FileNotFoundError(f"No existe el script de entrada: {ruta_script}")

    codigo_fuente = leer_codigo(ruta_script)
    codigo_mejorado = mejorar_con_claude(codigo_fuente, args.model)

    ruta_salida = construir_ruta_salida(ruta_script)
    ruta_salida.write_text(codigo_mejorado + "\n", encoding="utf-8")

    print(f"Script mejorado generado en: {ruta_salida}")


if __name__ == "__main__":
    main()
