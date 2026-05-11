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
        try (Reader in = new FileReader("./datos/ventas.csv")) {
            Iterable<CSVRecord> records = CSVFormat.DEFAULT.withFirstRecordAsHeader().parse(in);
            rows = new ArrayList<>();
            for (CSVRecord record : records) {
                rows.add(new HashMap<>(record.toMap()));
            }
        }
        // Creación de columna calculada
        for (Map<String, String> r : rows) {
            double base = Double.parseDouble(r.getOrDefault("cantidad", "0"));
            double calc = base * Double.parseDouble(r.getOrDefault("precio", "0"));
            r.put("importe_total", String.valueOf(calc));
        }
        // Creación de columna calculada
        for (Map<String, String> r : rows) {
            double base = Double.parseDouble(r.getOrDefault("importe_total", "0"));
            double calc = base * 0.21;
            r.put("impuesto", String.valueOf(calc));
        }
        // Proyección de columnas
        List<String> columnas = Arrays.asList("producto", "cantidad", "precio", "importe_total", "impuesto");
        rows = rows.stream().map(r -> {
            Map<String, String> n = new LinkedHashMap<>();
            for (String c : columnas) {
                n.put(c, r.get(c));
            }
            return n;
        }).collect(Collectors.toList());
        // Ordenación
        rows = rows.stream().sorted(Comparator.comparing((Map<String, String> r) -> r.getOrDefault("importe_total", "")).reversed()).collect(Collectors.toList());
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
