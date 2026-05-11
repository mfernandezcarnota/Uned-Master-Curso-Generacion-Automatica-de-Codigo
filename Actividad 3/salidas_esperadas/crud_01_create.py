import pandas as pd

def ejecutar_transformacion():
    df = pd.read_csv(r"./datos/ventas.csv")
    df["importe_total"] = df["cantidad"] * df["precio"]
    df["impuesto"] = df["importe_total"] * 0.21
    df = df[["producto", "cantidad", "precio", "importe_total", "impuesto"]]
    df = df.sort_values(by="importe_total", ascending=False)
    df.to_csv(r"./salida/resultado_crud.csv", index=False)
    return df
