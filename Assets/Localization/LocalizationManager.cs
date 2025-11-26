using System;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    public static event Action OnLanguageChanged;

    public TextAsset csvFile;
    public string currentLanguage = "es";

    private Dictionary<string, Dictionary<string, string>> table
        = new Dictionary<string, Dictionary<string, string>>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCSV();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LoadCSV()
    {
        table.Clear();

        if (csvFile == null)
        {
            Debug.LogWarning("CSV de localización no asignado.");
            return;
        }

        string[] lines = csvFile.text.Split(new[] { '\n' }, StringSplitOptions.None);
        if (lines.Length == 0) return;

        // buscar header real (evitar que haya comentarios o líneas en blanco arriba)
        int headerIndex = 0;
        while (headerIndex < lines.Length && (string.IsNullOrWhiteSpace(lines[headerIndex]) || lines[headerIndex].TrimStart().StartsWith("#")))
            headerIndex++;

        if (headerIndex >= lines.Length)
        {
            Debug.LogWarning("CSV de localización no tiene header válido.");
            return;
        }

        string[] header = SplitCsvLine(lines[headerIndex]);
        // normalizar encabezados (ej "key,es,en")
        for (int h = 0; h < header.Length; h++) header[h] = header[h].Trim();

        for (int i = headerIndex + 1; i < lines.Length; i++)
        {
            string raw = lines[i];
            if (string.IsNullOrWhiteSpace(raw)) continue;
            if (raw.TrimStart().StartsWith("#")) continue; // ignorar comentarios de sección

            string[] row = SplitCsvLine(raw);
            if (row.Length == 0) continue;

            string key = row[0].Trim();
            if (string.IsNullOrEmpty(key)) continue;

            var languages = new Dictionary<string, string>();

            for (int j = 1; j < header.Length; j++)
            {
                string lang = header[j].Trim();
                string value = "";
                if (j < row.Length)
                {
                    value = row[j].Trim();
                    // quitar comillas externas si las hay
                    if (value.Length >= 2 && value.StartsWith("\"") && value.EndsWith("\""))
                        value = value.Substring(1, value.Length - 2);
                    value = value.Replace("\\n", "\n");
                }
                languages[lang] = value;
            }

            table[key] = languages;
        }

        Debug.Log($"Localization: cargadas {table.Count} keys (idioma actual: {currentLanguage})");
    }

    // Simple CSV splitter que respeta comillas (no dependiente de librería)
    private string[] SplitCsvLine(string line)
    {
        var cols = new List<string>();
        bool inQuotes = false;
        var cur = new System.Text.StringBuilder();

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                // toggle quotes — si es doble comilla dentro de quotes, la consideramos parte del texto
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    cur.Append('"');
                    i++; // saltar la segunda comilla
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                cols.Add(cur.ToString());
                cur.Clear();
            }
            else
            {
                cur.Append(c);
            }
        }

        cols.Add(cur.ToString());
        return cols.ToArray();
    }

    public string GetText(string key)
    {
        if (string.IsNullOrEmpty(key)) return "";

        if (table.ContainsKey(key))
        {
            var langs = table[key];
            // si existe idioma actual y no está vacío, lo devolvemos
            if (!string.IsNullOrEmpty(currentLanguage) && langs.ContainsKey(currentLanguage) && !string.IsNullOrEmpty(langs[currentLanguage]))
                return langs[currentLanguage];

            // fallback a español si existe y no está vacío
            if (langs.ContainsKey("es") && !string.IsNullOrEmpty(langs["es"]))
                return langs["es"];

            // si hay alguna traducción disponible devolvemos la primera (evita #key tanto como sea posible)
            foreach (var v in langs.Values)
                if (!string.IsNullOrEmpty(v))
                    return v;
        }

        // DEBUG: loguear la key faltante (solo en desarrollo)
        Debug.LogWarning($"[Localization] Falta traducción para key '{key}' en idioma '{currentLanguage}'");
        return $"#{key}";
    }

    public void SetLanguage(string lang)
    {
        if (string.IsNullOrEmpty(lang)) return;

        currentLanguage = lang;

        // Refrescar textos de UI
        LocalizedText[] all = FindObjectsOfType<LocalizedText>(true);
        foreach (var t in all)
            t.RefreshText();

        // Avisar a todos los managers/listeners
        OnLanguageChanged?.Invoke();

        // También avisar a ScriptableObjects si quieres (no obligatorio)
        var localizedStrings = Resources.FindObjectsOfTypeAll<LocalizedString>();
        foreach (var ls in localizedStrings)
            ls.OnLanguageChanged();

        Debug.Log("Idioma cambiado a: " + lang);
    }
}


