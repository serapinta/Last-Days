using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour {

    public GameObject door;

    public void DoorOpen() {
        door.GetComponent<Animator>().SetBool("isOpen", true);
        door.GetComponent<Interactible>().missionRequirements = 0;
        door.GetComponent<Interactible>().interactionText = "[E] Open.";
    }
}
