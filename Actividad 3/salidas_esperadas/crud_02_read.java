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
        try (Reader in = new FileReader("./datos/clientes.csv")) {
            Iterable<CSVRecord> records = CSVFormat.DEFAULT.withFirstRecordAsHeader().parse(in);
            rows = new ArrayList<>();
            for (CSVRecord record : records) {
                rows.add(new HashMap<>(record.toMap()));
            }
        }
        // Filtro de filas
        rows = rows.stream().filter(r -> r.getOrDefault("pais", "").equals("ES")).collect(Collectors.toList());
        // Filtro de filas
        rows = rows.stream().filter(r -> Double.parseDouble(r.getOrDefault("activo", "0")) == 1).collect(Collectors.toList());
        // Proyección de columnas
        List<String> columnas = Arrays.asList("id_cliente", "nombre", "email", "pais");
        rows = rows.stream().map(r -> {
            Map<String, String> n = new LinkedHashMap<>();
            for (String c : columnas) {
                n.put(c, r.get(c));
            }
            return n;
        }).collect(Collectors.toList());
        // Ordenación
        rows = rows.stream().sorted(Comparator.comparing((Map<String, String> r) -> r.getOrDefault("nombre", ""))).collect(Collectors.toList());
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
