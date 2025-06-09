using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class UI_Manager_MainMenu : MonoBehaviour
{
    [Header("Paneles de UI")]
    [SerializeField] private GameObject panelMain;
    [SerializeField] private GameObject panelOptions;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        panelMain.SetActive(true);
        panelOptions.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && panelOptions.activeSelf == true)
        {
            panelMain.SetActive(true);
            panelOptions.SetActive(false);
        }
    }

    public void ButtonPlay()
    {
        SceneLoader.LoadScene("LaPeladaTeAcosaFuerte");
    }

    public void ButtonOptions()
    { 
        panelMain.SetActive(false);
        panelOptions.SetActive(true);
    }

    public void ButtonBack()
    {
        panelMain.SetActive(true);
        panelOptions.SetActive(false);
    }

    public void ButtonExit()
    {
        Debug.Log("_Cerrando el - JUEGO - ");
    }
}
