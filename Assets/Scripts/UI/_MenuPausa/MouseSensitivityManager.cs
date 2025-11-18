using UnityEngine;

public class MouseSensitivityManager : MonoBehaviour
{
    public static MouseSensitivityManager Instance { get; private set; }

    public float CurrentSensitivity { get; private set; } = 1f; // Valor por defecto

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetSensitivity(float sensitivity)
    {
        CurrentSensitivity = sensitivity;
    }
}
