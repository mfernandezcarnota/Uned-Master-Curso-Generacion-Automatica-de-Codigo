import pandas as pd

def ejecutar_transformacion():
    df = pd.read_csv(r"./datos/clientes.csv")
    df = df[df["pais"] == "ES"]
    df = df[df["activo"] == 1]
    df = df[["id_cliente", "nombre", "email", "pais"]]
    df = df.sort_values(by="nombre", ascending=True)
    df.to_csv(r"./salida/resultado_crud.csv", index=False)
    return df
