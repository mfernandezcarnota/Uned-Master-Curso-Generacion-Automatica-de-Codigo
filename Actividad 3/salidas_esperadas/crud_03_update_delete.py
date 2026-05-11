import pandas as pd

def ejecutar_transformacion():
    df = pd.read_csv(r"./datos/inventario.csv")
    df = df[df["stock"] > 0]
    df["stock_seguridad"] = df["stock"] - 5
    df = df[df["stock_seguridad"] > 0]
    df = df[["sku", "descripcion", "stock", "stock_seguridad"]]
    df.to_csv(r"./salida/resultado_crud.csv", index=False)
    return df
