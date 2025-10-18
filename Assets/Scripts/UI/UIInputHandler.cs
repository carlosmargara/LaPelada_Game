using UnityEngine;
using UnityEngine.InputSystem;

public class UIInputHandler : MonoBehaviour
{
    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (InventoryUI.Instance?.IsInventoryOpen == true)
            InventoryUI.Instance.OnNavigate(context);
        else if (PickupUIManager.Instance?.IsActive == true)
            PickupUIManager.Instance.OnNavigate(context);
        else if (NoteManager.Instance?.IsDescribing == true)
            NoteManager.Instance.OnNavigate(context);
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
        if (InventoryUI.Instance?.IsInventoryOpen == true)
            InventoryUI.Instance.OnPoint(context);
        else if (PickupUIManager.Instance?.IsActive == true)
            PickupUIManager.Instance.OnPoint(context);
        else if (NoteManager.Instance?.IsDescribing == true)
            NoteManager.Instance.OnPoint(context);
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (DialogueManager.Instance?.IsTalking == true)
            DialogueManager.Instance.OnSubmit(context);
        else if (PickupUIManager.Instance != null && PickupUIManager.Instance.IsActive)
        {
            Debug.Log("ENTRO EN EL IF_de UIInputHandle");
            PickupUIManager.Instance.OnSubmit(context);
        }
        else if (NoteManager.Instance?.IsDescribing == true)
            NoteManager.Instance.OnSubmit(context);
        else if (InventoryUI.Instance?.IsInventoryOpen == true)
        {
            /*
            if (context.performed)
            {
                var selected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
                if (selected != null)
                {
                    var slot = selected.GetComponent<Slot_Inventory>();
                    slot?.SelectSlotWithKeyboard();
                }
            }
            */
        }
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (DialogueManager.Instance?.IsTalking == true)
            DialogueManager.Instance.OnCancel(context);
        else if (PickupUIManager.Instance?.IsActive == true)
            PickupUIManager.Instance.OnCancel(context);
        else if (NoteManager.Instance?.IsDescribing == true)
            NoteManager.Instance.OnCancel();
        else if (InventoryUI.Instance?.IsInventoryOpen == true)
            InventoryUI.Instance.ToggleInventory();
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // 🗨️ Si estás en un diálogo, tratamos el clic como "Submit"
        if (DialogueManager.Instance?.IsTalking == true)
            DialogueManager.Instance.OnClick(context);
        else if (PickupUIManager.Instance?.IsActive == true)
            PickupUIManager.Instance.OnClick(context);
        else if (NoteManager.Instance?.IsDescribing == true)
            NoteManager.Instance.OnClick(context);
        // 🪄 Si querés también podrías hacerlo interactuar con otras UIs más adelante:
        // else if (InventoryUI.Instance?.IsInventoryOpen == true)
        //     InventoryUI.Instance.OnClick(context);
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (InventoryUI.Instance != null)
        {
            InventoryUI.Instance.ToggleInventory();
        }
    }
}

