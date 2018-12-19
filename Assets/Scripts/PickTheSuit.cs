using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickTheSuit : MonoBehaviour {

    private bool isInRange;

    private void Start() {
        Cursor.visible = false;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            isInRange = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.tag == "Player") {
            isInRange = false;
        }
    }

    // Update is called once per frame
    void Update () {
		if (isInRange && Input.GetButton("e")) {
            Destroy(this.gameObject);
        }
	}
}
