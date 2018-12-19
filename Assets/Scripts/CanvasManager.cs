using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour {

    // Singleton that hides the 'CanvasManager' instance
    #region Singleton

    public static CanvasManager Instance { get; private set; }

    void Awake() {

        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    #endregion

    // A few variables to be assigned in the Editor
    public GameObject interactionPanel;
    public Text interactionText;
    public GameObject inventoryPanel;
    public Image[] inventoryIcons;
    public Image[] inventorySelections;
    public GameObject dialogueBox;
    public GameObject centeredDot;
    public Animator aniDoor;
    public GameObject pauseMenu;

    // Confines the cursor to the game window
    // and clears all inventory slot icons
    void Start() {

        Cursor.lockState = CursorLockMode.Confined;
        HideInteractionPanel();

        ClearAllInventorySlotIcons();
    }

    private void Update() {

        // Inventory Up
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!aniDoor.GetBool("IsUp"))
            {
                aniDoor.SetBool("IsUp", true);
            }
            // Inventory Down
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            if (aniDoor.GetBool("IsUp"))
            {
                aniDoor.SetBool("IsUp", false);
            }
        }

        if (dialogueBox.activeSelf) {

            HideInteractionPanel();
            HideCenteredDot();

        } else {

            ShowCenteredDot();
        }
    }

    public void ShowCenteredDot() {

        centeredDot.SetActive(true);
    }

    public void HideCenteredDot() {

        centeredDot.SetActive(false);
    }

    // Method responsible for showing the interaction panel and its text*
    // *(which is defined in the Interactible itself)
    public void ShowInteractionPanel(string text) {

        interactionText.text = text;
        interactionPanel.SetActive(true);
    }

    // Method responsible for hiding the interaction panel
    public void HideInteractionPanel() {

        interactionPanel.SetActive(false);
    }

    // Method responsible for showing a certain Interactible icon* 
    // on the inventory slots
    // *(which is defined on the Interactible itself)
    public void SetInventorySlotIcon(int slotNumber, Sprite icon) {

        inventoryIcons[slotNumber].sprite = icon;
        inventoryIcons[slotNumber].enabled = true;
    }

    // Method responsible for hiding a certain Interactible icon* 
    // on the inventory slots
    // *(which is defined on the Interactible itself)
    public void ClearInventorySlotIcon(int slotNumber) {

        inventoryIcons[slotNumber].enabled = false;
        inventoryIcons[slotNumber].sprite = null;
    }

    // Method responsible for clearing all slot icons
    private void ClearAllInventorySlotIcons() {

        for (int i = 0; i < inventoryIcons.Length; i++) {

            ClearInventorySlotIcon(i);
        }
    }

    // Method responsible for enabling the selection sprite on a selected slot
    public void SetSlotAsActive(int slotNumber) {

        inventorySelections[slotNumber].enabled = true;
    }

    // Method responsible for disabling the selection sprite on a selected slot
    public void SetSlotAsInactive(int slotNumber) {

        inventorySelections[slotNumber].enabled = false;
    }

    // Method responsible for clearing all selected slots 
    // (although only one ever gets selected)
    public void ClearAllSelectedSlots() {

        for (int i = 0; i < inventorySelections.Length; i++) {

            SetSlotAsInactive(i);
        }
    }
}
