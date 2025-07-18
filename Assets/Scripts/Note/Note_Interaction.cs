using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note_Interaction : Interactable
{
    [SerializeField] private NoteData _data;
    public NoteData Data => _data;

    private bool hasPlayedAudio = false;

    public override void Interact()
    {
        NoteManager.Instance.noteInteraction = this;
        NoteManager.Instance.ShowInteracText_First(_data);

        if (_data.playAudioOnRead && !hasPlayedAudio)
        {
            AudioManager02.Instance.Read_Dario_LaVoz.start();
            hasPlayedAudio = true;
        }
        else
        {
            Debug.Log("El sonido ya fue reproducido antes, no se vuelve a lanzar.");
        }

        Debug.Log("Estas leyendo la nota");
    }

    public void ResetAudio()
    {
        hasPlayedAudio = false;
    }
}
