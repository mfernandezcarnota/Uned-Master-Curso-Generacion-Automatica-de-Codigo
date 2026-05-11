// ============================================================
// ARCHIVO GENERADO AUTOMATICAMENTE DESDE M (Power Query)
// Lenguaje de salida: Java + Apache Commons CSV
// ============================================================
import java.io.*;
import java.nio.file.*;
import java.util.*;
import java.util.stream.*;
import org.apache.commons.csv.*;

public class Main {
    public static List<Map<String, String>> ejecutarTransformacion() throws Exception {
        List<Map<String, String>> rows = new ArrayList<>();
        // Carga del dataset principal desde metadatos
        try (Reader in = new FileReader("./datos/inventario.csv")) {
            Iterable<CSVRecord> records = CSVFormat.DEFAULT.withFirstRecordAsHeader().parse(in);
            rows = new ArrayList<>();
            for (CSVRecord record : records) {
                rows.add(new HashMap<>(record.toMap()));
            }
        }
        // Filtro de filas
        rows = rows.stream().filter(r -> Double.parseDouble(r.getOrDefault("stock", "0")) > 0).collect(Collectors.toList());
        // Creación de columna calculada
        for (Map<String, String> r : rows) {
            double base = Double.parseDouble(r.getOrDefault("stock", "0"));
            double calc = base - 5;
            r.put("stock_seguridad", String.valueOf(calc));
        }
        // Filtro de filas
        rows = rows.stream().filter(r -> Double.parseDouble(r.getOrDefault("stock_seguridad", "0")) > 0).collect(Collectors.toList());
        // Proyección de columnas
        List<String> columnas = Arrays.asList("sku", "descripcion", "stock", "stock_seguridad");
        rows = rows.stream().map(r -> {
            Map<String, String> n = new LinkedHashMap<>();
            for (String c : columnas) {
                n.put(c, r.get(c));
            }
            return n;
        }).collect(Collectors.toList());
        // Persistencia del resultado (CSV simple)
        try (BufferedWriter writer = Files.newBufferedWriter(Paths.get("./salida/resultado_crud.csv"))) {
            if (!rows.isEmpty()) {
                List<String> headers = new ArrayList<>(rows.get(0).keySet());
                writer.write(String.join(",", headers));
                writer.newLine();
                for (Map<String, String> row : rows) {
                    List<String> values = headers.stream().map(h -> row.getOrDefault(h, "")).collect(Collectors.toList());
                    writer.write(String.join(",", values));
                    writer.newLine();
                }
            }
        }
        return rows;
    }

    public static void main(String[] args) throws Exception {
        List<Map<String, String>> resultado = ejecutarTransformacion();
        System.out.println("Filas transformadas: " + resultado.size());
    }
}
