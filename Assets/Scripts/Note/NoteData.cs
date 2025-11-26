using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewNote", menuName = "Note")]
public class NoteData : ScriptableObject
{
    [Header("Meta")]
    public LocalizedString noteTitle;     // 🔥 título ahora localizado
    public Sprite foregroundImage;
    public bool activaIA;

    [Header("Audio")]
    public bool playAudioOnRead;

    [Header("Texto Interac (localizado)")]
    public LocalizedString interacText01;
    public LocalizedString interacText02;

    [Space]

    [Header("Texto Notas (páginas localizadas)")]
    public LocalizedString[] pages;

    // --- Helpers públicos ---

    public string GetTitle() => noteTitle != null ? noteTitle.GetValue() : "";

    public string GetInteracText01() => interacText01 != null ? interacText01.GetValue() : "";
    public string GetInteracText02() => interacText02 != null ? interacText02.GetValue() : "";

    public int GetPageCount() => pages == null ? 0 : pages.Length;

    public string GetPage(int index)
    {
        if (pages == null || index < 0 || index >= pages.Length)
            return "";

        return pages[index] != null ? pages[index].GetValue() : "";
    }

    public List<string> GetAllPages()
    {
        var outList = new List<string>();
        int c = GetPageCount();
        for (int i = 0; i < c; i++)
            outList.Add(GetPage(i));
        return outList;
    }
}

