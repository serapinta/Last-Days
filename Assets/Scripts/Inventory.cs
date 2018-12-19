using System.Collections.Generic;
using UnityEngine;
using System;

public class Inventory : MonoBehaviour {

    // Size of the inventory
    public int inventorySpaces = 6;

    // List that will hold all Interactibles (this is the inventory)
    public Interactible[] items;

    private int itemsInInventory;

    // 'CanvasManager' instanced that will be called on the Start method
    private CanvasManager _canvasManager;

    // Creates the list with the previously defined size 
    // and calls the instance of the previous '_canvasManager'
    void Start() {

        itemsInInventory = 0;

        items = new Interactible[6];

        _canvasManager = CanvasManager.Instance;
    }

    // Method that adds Interactibles to the List
    public void AddItem(Interactible item) {

        // Checks if the interactible is showing in the inventory
        if (item.showInInventory) {

            // If so, checks if the List if Full
            if (itemsInInventory == inventorySpaces) {

                // ########## FOR NOW JUST SHOWS A DEBUG.LOG ##########
                // and leaves the method
                Debug.Log("The Inventory is Full");
                return;
            }

            // If the List isn't full, deactivate the object
            item.gameObject.SetActive(false);

            // Run through the array and fill the first null slot
            for (int index = 0; index < inventorySpaces; index++) {

                if (items[index] == null) {

                    items[index] = item;
                    itemsInInventory++;
                    AddIcons(item);

                    // Remove the Gravity Switching Device immediatelly
                    if (item.tag == "GSD") {
                        RemoveItem(item);
                    }

                    // Select the item automatically if the Array is empty
                    if (items[0] != null && !VerifyArraySpaces(1) && item.tag != "GSD") {

                        item.isSelected = true;
                        _canvasManager.SetSlotAsActive(0);
                    }

                    return;
                }
            }
        }
    }

    // Method that verifies if the List contains a certain Pickable Interactible
    public bool HasInInventory(Interactible item) {

        foreach (Interactible i in items) {

            if (i == item) {

                return true;
            }
        }

        return false;
    }

    // Method that removes items from the List
    public void RemoveItem(Interactible item) {

        for (int index = 0; index < inventorySpaces; index++) {

            if (items[index] == item) {

                itemsInInventory--;
                // Remove icon before item gets "destroyed"
                RemoveIcons(item);
                Destroy(item.gameObject);
                return;
            }
        }
    }

    // Adds icons on to the screen
    private void AddIcons(Interactible item) {

        foreach (Interactible i in items) {

            // GET INDEX
            int index = Array.FindIndex(items, m => m == item);

            _canvasManager.SetInventorySlotIcon(index, item.icon);
        }
    }

    // Removes icons from the screen
    private void RemoveIcons(Interactible item) {

        foreach (Interactible i in items) {

            if (i == item) {

                // GET INDEX
                int index = Array.FindIndex(items, m => m == item);

                _canvasManager.ClearInventorySlotIcon(index);
            }
        }
    }

    // Method that verifies which Interactible is selected
    public bool CheckItemsSelected(Interactible interactible) {

        // Run through the Array
        for (int i = 0; i < interactible.inventoryRequirements.Length; i++) {

            // Return 'true' if the required interactibles of another 
            // interactible are selected (although only one can be selected at a time)
            // ########## MIGHT NEED TO CHANGE THIS ##########
            if (interactible.inventoryRequirements[i].isSelected == true) {

                return true;
            }

        }

        // Return 'false' if the items are not selected
        return false;
    }

    // Verifies if the Array is empty
    private bool VerifyArraySpaces(int starterIndex) {

        for (int i = starterIndex; i < inventorySpaces; i++) {

            if (items[i] != null) {

                return true;
            }
        }

        return false;
    }
}
