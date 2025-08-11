using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using TMPro;


public class UI_Manager_MainMenu : MonoBehaviour
{
    [Header("Paneles de UI")]
    [SerializeField] private GameObject panel02;
    [SerializeField] private GameObject panel01;
    [SerializeField] private GameObject panelOptions;

    [Space]

    [Header("Ref TMP")]
    [SerializeField] private TextMeshProUGUI pressStar_TMP;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None; // Allow free mouse movement
        Cursor.visible = true; // Show the cursor

        panel01.SetActive(true);
        panel02.SetActive(false);
        panelOptions.SetActive(false);

        StartCoroutine(BlinkText());
    }

    private void Update()
    {
        Debug.Log($"Cursor.lockState: {Cursor.lockState}, Cursor.visible: {Cursor.visible}");

        if (Input.GetKeyDown(KeyCode.Escape) && panelOptions.activeSelf == true)
        {
            panel02.SetActive(true);
            panelOptions.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Return) && panel01.activeSelf == true)
        {
            panel01.SetActive(false);
            panel02.SetActive(true);
        }
    }

    public void ButtonPlay()
    {
        //AudioManager02.Instance.PlayOneShot("event:/UI/PlayGame_Sound (Main_Menu)");
        SceneLoader.LoadScene("LaPeladaTeAcosaFuerte");
    }

    public void ButtonOptions()
    {
        panel02.SetActive(false);
        panelOptions.SetActive(true);
    }

    public void ButtonBack()
    {
        panel02.SetActive(true);
        panelOptions.SetActive(false);
    }

    public void ButtonExit()
    {
        Debug.Log("_Cerrando el - JUEGO - ");
    }

    IEnumerator BlinkText()
    {
        while (true)
        {
            pressStar_TMP.enabled = !pressStar_TMP.enabled;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
