using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class PickupUIManager : Singleton<PickupUIManager>
{
    [Header("UI Elements")]
    [SerializeField] private GameObject panel;
    [Space]
    [SerializeField] private TextMeshProUGUI messageText;
    [Space]
    [SerializeField] private GameObject _yesButton;
    [SerializeField] private GameObject _noButton;

    [Header("3D Preview")]
    [SerializeField] private GameObject itemPreviewHolder;
    [SerializeField] private GameObject itemView_rawImage; //esta es la referencia del gameObject que contiene la rawImage que muestra el objeto3D
    [SerializeField] private RawImage itemPreviewImage; // Asegurate que esta sea tu RawImage en el canvas
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private Camera previewCamera;

    private GameObject currentPreviewObject;

    public bool firtTextWasShown;
    private bool secondTextWasShown;
    private bool descriptionItemAmin;
    private PickupItem_interac currentItem;

    private void Start()
    {
        panel.SetActive(false);
        if (previewCamera != null)
        {
            previewCamera.targetTexture = renderTexture;
            previewCamera.gameObject.SetActive(false); // Se activa sólo cuando hace falta
        }
    }

    private void Update()
    {
        if (!panel.activeSelf) return;

        if (panel.activeSelf)
        {
            GameStateManager.Instance.LockPlayer();
        }
        else
        {
            GameStateManager.Instance.UnlockPlayer();
        }

        if (Input.GetMouseButtonDown(0) && firtTextWasShown && descriptionItemAmin)
        {
            ShowSecondTextPickup(currentItem);
        }

        if (Input.GetMouseButtonDown(0) && secondTextWasShown)
        {
            PastNextAction();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePanel();
        }

        if (currentPreviewObject != null)
        {
            currentPreviewObject.transform.Rotate(Vector3.up, 45 * Time.deltaTime);
        }
    }

    public void ShowPickupPrompt(PickupItem_interac item)
    {
        currentItem = item;
        panel.SetActive(true);
        ShowTextAmin(item.Ref_ScriptableObject.pickupText); 
        _yesButton.SetActive(false);
        _noButton.SetActive(false);

        firtTextWasShown = true;

        HideItemPreview(); // Por si quedó algo anterior
    }

    private void ShowSecondTextPickup(PickupItem_interac item)
    {
        currentItem = item;
        ShowTextAmin(item.Ref_ScriptableObject.pickupText02);
        _yesButton.SetActive(true);
        _noButton.SetActive(true);

        HideItemPreview();
    }

    public void ConfirmPickup()
    {
        AudioManager.Instance.PlaySoundFX(AudioManager.Instance.pickUP_Sound, 0.6f); //Laza el sonido de PickUp_Item
        
        StopAllCoroutines();
        ShowTextAmin(currentItem.Ref_ScriptableObject.confirmationText);
        itemView_rawImage.SetActive(true);
        _yesButton.SetActive(false);
        _noButton.SetActive(false);
        secondTextWasShown = true;
        firtTextWasShown = false;

        ShowItemPreview(currentItem.Ref_ScriptableObject);

        Destroy(currentItem.gameObject);
    }

    public void ClosePanel()
    {
        StopAllCoroutines();
        panel.SetActive(false);
        itemView_rawImage.SetActive(false);
        secondTextWasShown = false;
        descriptionItemAmin = false;
        firtTextWasShown = false;
        currentItem = null;

        HideItemPreview();
    }

    private void PastNextAction()
    {
        currentItem.Pickup();
        ClosePanel();
    }

    private void ShowItemPreview(Inventory_Item itemData)
    {
        if (itemData == null || itemData.prefabModel == null || itemPreviewHolder == null) return;

        currentPreviewObject = Instantiate(itemData.prefabModel, itemPreviewHolder.transform);
        currentPreviewObject.transform.localPosition = Vector3.zero;
        currentPreviewObject.transform.localRotation = Quaternion.identity;
        currentPreviewObject.transform.localScale = Vector3.one;

        if (previewCamera != null)
        {
            previewCamera.gameObject.SetActive(true);
        }
    }

    private void HideItemPreview()
    {
        if (currentPreviewObject != null)
        {
            Destroy(currentPreviewObject);
        }

        if (previewCamera != null)
        {
            previewCamera.gameObject.SetActive(false);
        }
    }

    private void ShowTextAmin(string text)
    {
        StartCoroutine(AminText(text));
    }

    private IEnumerator AminText(string text)
    {
        descriptionItemAmin = false;
        messageText.text = "";
        char[] chars = text.ToCharArray();

        for (int i = 0; i < chars.Length; i++)
        {
            messageText.text += chars[i];
            yield return new WaitForSeconds(0.03f);
        }

        descriptionItemAmin = true;
    }
}
