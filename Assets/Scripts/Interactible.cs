using UnityEngine;

public class Interactible : MonoBehaviour {

    // All variables to be set in the Editor
    public string interactibleName;
    public Sprite icon;
    public bool isPickable;
    public bool isInteractive;
    public bool consumesRequirements;
    public bool allowsMultipleInteractions;
    public bool showInInventory = true;
    public bool isSelected = false;
    public string requirementText;
    public string interactionText;
    public Interactible[] inventoryRequirements;
    public int missionRequirements;

    // Method responsible for disabling an object 
    // if it doesn't allow multiple interactions
    public void Interact() {

        if (isPickable || isInteractive) {

            if (!allowsMultipleInteractions) {

                isInteractive = false;
            }
        }
    }
}
