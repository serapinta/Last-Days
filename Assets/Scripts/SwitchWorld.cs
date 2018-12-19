using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchWorld : MonoBehaviour {

    private void OnTriggerEnter(Collider hit) {

        if (hit.tag == "Player") {

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
