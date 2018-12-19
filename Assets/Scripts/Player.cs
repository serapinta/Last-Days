using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    // Maximum distance from which the player interacts with objects
    public float maxInteractionDistance = 1.8f;
    [ExecuteInEditMode]
    public bool equipedSuit = false;
    public int currentMission = 0;

    // A few variables to be instanced
    private CanvasManager _canvasManager;
    private Camera _camera;
    private RaycastHit _raycastHit;
    private Interactible _currentInteractible;
    private Inventory _myInventory;
    private Color _white;
    private Color _red;

    // The previous variables are instanced here
    private void Start() {

        _canvasManager = CanvasManager.Instance;

        _camera = GetComponentInChildren<Camera>();

        _currentInteractible = null;

        _myInventory = gameObject.AddComponent<Inventory>();

        _white = new Color(255, 255, 255);

        _red = new Color(255, 0, 0);
    }
    
    // Unity's Update
    void Update() {
        
        CheckInput();
    }

    // Unity's FixedUpdate
    private void FixedUpdate() {

        CheckForItem();
    }

    // Method that detects interactible objects
    private void CheckForItem() {

        // Detects interactible in front of the player
        if (Physics.Raycast(_camera.transform.position,
            _camera.transform.forward, out _raycastHit,
            maxInteractionDistance)) {

            // Instance of a new Interactible if it is detected
            Interactible newInteractible = 
                _raycastHit.collider.GetComponent<Interactible>();

            // Check if interactible exists and is interactive
            if (newInteractible != null && newInteractible.isInteractive) {

                // Set this new Interactible as the one the player 
                // can interact with
                SetInteractible(newInteractible);

            } else {

                // Set the interactible as null
                ClearInteractible();
            }

        } else {

            // Set the interactible as null
            ClearInteractible();
        }
    }

    // Method that detects player Input
    private void CheckInput() {

        // Variable to be changed depending on the number pressed
        // on the keyboard
        int slotNumber = 0;

        // The 'E' key makes the player interact with any Interactible object
        if (Input.GetKeyDown(KeyCode.E) && _currentInteractible != null) {
            if (CheckForNPC(_currentInteractible)) {

                _currentInteractible.GetComponent<DialogueTrigger>().TriggerDialogue();

                // Adds the Interactible to the Inventory if it can be picked up
            } else if (_currentInteractible.isPickable) {
                if (_currentInteractible.tag != "Suit") {
                    _myInventory.AddItem(_currentInteractible);
                } else {
                    currentMission++;
                    equipedSuit = true;
                    _currentInteractible.gameObject.SetActive(false);
                }


                // Interacts with an object that requires another object only if
                // that other object is in the inventory and is selected
            } else if (HasRequirements(_currentInteractible) &&
                _myInventory.CheckItemsSelected(_currentInteractible)) {

                Interact(_currentInteractible);
            } else if (HasRequirements(_currentInteractible) &&
                _currentInteractible.interactibleName == "Door") {
                if (_currentInteractible.gameObject.GetComponent<Animator>().GetBool("isOpen")) {
                    _currentInteractible.interactionText = "[E] Open";
                    _currentInteractible.gameObject.GetComponent<Animator>().SetBool("isOpen", false);
                } else {
                    _currentInteractible.gameObject.GetComponent<Animator>().SetBool("isOpen", true);
                    _currentInteractible.interactionText = "[E] Close";
                }
                if (_currentInteractible.requirementText == "Suit_Here") {
                    _currentInteractible.isInteractive = false;
                    _currentInteractible.GetComponent<BoxCollider>().enabled = false;
                    _currentInteractible.GetComponentsInChildren<BoxCollider>()[1].enabled = false;
                }
            }

            // The following 'GetKeyDown's work only if there is an item in said slot
        } else if (Input.GetKeyDown("1")) {

            slotNumber = 1;

            ChangeSelectedSlot(slotNumber);

        } else if (Input.GetKeyDown("2")) {

            slotNumber = 2;

            ChangeSelectedSlot(slotNumber);

        } else if (Input.GetKeyDown("3")) {

            slotNumber = 3;

            ChangeSelectedSlot(slotNumber);

        } else if (Input.GetKeyDown("4")) {

            slotNumber = 4;

            ChangeSelectedSlot(slotNumber);

        } else if (Input.GetKeyDown("5")) {

            slotNumber = 5;

            ChangeSelectedSlot(slotNumber);

        } else if (Input.GetKeyDown("6")) {

            slotNumber = 6;

            ChangeSelectedSlot(slotNumber);
        } 
    }

    // Method that sets the '_currentInteractible' 
    // as the one the player is looking at
    private void SetInteractible(Interactible newInteractible) {

        _currentInteractible = newInteractible;

        // If there's an NPC
        if (CheckForNPC(_currentInteractible)) {
            // Display the Interaction text
            _canvasManager.ShowInteractionPanel(_currentInteractible.requirementText);

            if (currentMission == 2) {
                _currentInteractible.gameObject.GetComponent<OpenDoor>().DoorOpen();
            }

            // Displays the glow only if the interactible is not an npc
        } else if (_currentInteractible.interactibleName == "Door") {
            if (HasRequirements(_currentInteractible)) {
                _canvasManager.ShowInteractionPanel(_currentInteractible.interactionText);
            } else {
                _canvasManager.ShowInteractionPanel(_currentInteractible.requirementText);
            }
        } else {
            // If the current Interactible does not have requirements 
            // (or if they are fulfilled)...
            if (HasRequirements(_currentInteractible)) {
                // ...it displays a text
                _canvasManager.ShowInteractionPanel(_currentInteractible.interactionText);

                // ...if not...
            } else {
                // ...it displays a requirement text
                _canvasManager.ShowInteractionPanel(_currentInteractible.requirementText);
            }

            _canvasManager.centeredDot.GetComponent<Image>().color = _red;
        }
    }

    // Method that clears the current Interactible
    private void ClearInteractible() {

        // "Disable" the outline
        if (_currentInteractible != null) {

            _canvasManager.centeredDot.GetComponent<Image>().color = _white;
        }

        // The current Interactible stops existing
        _currentInteractible = null;

        // Hide the text
        _canvasManager.HideInteractionPanel();
    }

    // Method that checks if the current Interactible has requirements
    private bool HasRequirements(Interactible interactible) {

        if (interactible.missionRequirements != 0) {
            if (interactible.missionRequirements <= currentMission) {
                return true;
            } else {
                return false;
            }
        }

        // Runs through the array 'inventoryRequirements'of said Interactible...
        for (int i = 0; i < interactible.inventoryRequirements.Length; i++) {

            // ...and checks if the requirement(s) is(are) in the inventory
            if (!_myInventory.HasInInventory(interactible.inventoryRequirements[i])) {
                
                // Return 'false' if the requirements are not met
                return false;
            }

        }

        // Returns 'true' if they are
        return true;
    }

    private bool CheckForNPC(Interactible interactible) {
        if (interactible.tag == "NPC") {
            return true;
        } else {
            return false;
        }
    }

    // Method responsible for the interactions
    private void Interact(Interactible interactible) {

        // If said interactible consumes requirements...
        if (interactible.consumesRequirements) {

            // ...all requirements present in the inventory get removed
            // and the selected slot stops existing
            for (int i = 0; i < interactible.inventoryRequirements.Length; i++) {

                if (interactible.inventoryRequirements[i].interactibleName == "Fuse") {
                    currentMission++;
                }

                _myInventory.RemoveItem(interactible.inventoryRequirements[i]);
                _canvasManager.ClearAllSelectedSlots();
            }
        }

        // Check also if the interactible allows multiple interactions
        interactible.Interact();
    }

    // Method that changes the selected slot
    private void ChangeSelectedSlot(int slotNumber) {

        // Checks if the slot the player wants to choose contains something
        if (_myInventory.items[slotNumber - 1] != null) {

            // If it does, run through the inventory...
            for (int i = 0; i < _myInventory.items.Length; i++) {

                // ...unselect all slots and objects...
                if (_myInventory.items[i] != null) {

                    _canvasManager.SetSlotAsInactive(i);
                    _myInventory.items[i].isSelected = false;
                }
            }

            // ... and assign the current slot and object in it as selected
            _canvasManager.SetSlotAsActive(slotNumber - 1);
            _myInventory.items[slotNumber - 1].isSelected = true;
        }
    }
}
