using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Manager : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RetryGame()
    {
        // Si existe el GameStateManager, reseteamos su estado antes de recargar
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ResetState();
        }

        SceneManager.LoadScene(1);
    }

    public void ButtonQuit()
    {
        Debug.Log("Cerrando el juego...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
