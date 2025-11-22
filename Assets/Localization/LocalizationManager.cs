using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

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

        string[] lines = csvFile.text.Split('\n');
        if (lines.Length == 0) return;

        string[] header = lines[0].Split(','); // key,es,en,...

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] row = lines[i].Split(',');

            string key = row[0].Trim();

            var languages = new Dictionary<string, string>();

            for (int j = 1; j < row.Length; j++)
            {
                string lang = header[j].Trim();
                string value = row[j].Trim().Replace("\"", "");
                languages[lang] = value;
            }

            table[key] = languages;
        }
    }

    public string GetText(string key)
    {
        if (table.ContainsKey(key) && table[key].ContainsKey(currentLanguage))
            return table[key][currentLanguage];

        return $"#{key}"; // Si falta te lo muestra clarito
    }

    public void SetLanguage(string lang)
    {
        currentLanguage = lang;

        LocalizedText[] all = FindObjectsOfType<LocalizedText>(true);
        foreach (var t in all)
            t.RefreshText();
    }
}

