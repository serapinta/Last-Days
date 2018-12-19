using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TemporaryTrigger : MonoBehaviour {

    public GameObject player;

    private void OnTriggerEnter(Collider hit) {

        if (hit.tag == "Player") {

            SceneManager.LoadScene("Menu");
        }
    }


}
