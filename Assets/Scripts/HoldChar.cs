using UnityEngine;

public class HoldChar : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            other.transform.parent = gameObject.transform;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            other.transform.parent = null;
        }
    }
}
