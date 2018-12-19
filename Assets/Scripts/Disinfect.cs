using System;
using System.Collections;
using UnityEngine;

public class Disinfect : MonoBehaviour {

    public GameObject[] doors;
    private FirstPersonController _player;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            _player = other.gameObject.GetComponent<FirstPersonController>();
            for (int i = 0; i < doors.Length; i++) {
                if (doors[i].GetComponent<Animator>().GetBool("isOpen")) {
                    doors[i].GetComponent<Animator>().SetBool("isOpen", false);
                }
            }
            StartCoroutine(RestrainPlayer());
        }
    }

    IEnumerator RestrainPlayer() {
        _player.enabled = false;
        yield return new WaitForSecondsRealtime(3);
        doors[2].GetComponent<Animator>().SetBool("isOpen", true);
        doors[3].GetComponent<Animator>().SetBool("isOpen", true);
        _player.enabled = true;
    }
}
