using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DressSuit : MonoBehaviour {

    public bool AsSuit { get; private set; }

    private bool isInRange;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Suit") {
            isInRange = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Suit") {
            isInRange = false;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (isInRange && Input.GetButton("e")) {
            AsSuit = true;
        }
    }
}
