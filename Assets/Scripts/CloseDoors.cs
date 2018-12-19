using UnityEngine;

public class CloseDoors : MonoBehaviour {

    // Start animation on doors
    public Animator[] doors;

    // Check if the doors are open, if so they will close.
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            for (int i = 0; i < doors.Length; i++) {
                if (doors[i].GetBool("isOpen")) {
                    doors[i].SetBool("isOpen", false);
                }
            }
        }
    }
}
