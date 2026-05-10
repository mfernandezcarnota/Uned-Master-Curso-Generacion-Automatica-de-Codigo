"""Validaciones personalizadas (custom code) cargadas dinámicamente."""


def validar_edad_mayor_0(valor):
    if valor is None:
        return None
    if not isinstance(valor, int) or valor < 0:
        return "La edad debe ser un entero positivo o cero"
    return None


def validar_email_basico(valor):
    if valor is None:
        return None
    if "@" not in str(valor):
        return "El email debe contener '@'"
    return None


CUSTOM_VALIDATORS = {
    "edad": validar_edad_mayor_0,
    "email": validar_email_basico,
}
