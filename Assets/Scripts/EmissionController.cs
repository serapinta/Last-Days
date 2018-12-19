using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissionController : MonoBehaviour {

    [SerializeField]
    private Material[] materials;

    [SerializeField]
    private Player player;

    private bool active;

    private void Awake() {
        for (int i = 0; i < materials.Length; i++) {
            materials[i].SetColor("_EmissionColor", Color.black);
        }
    }

    // Update is called once per frame
    void LateUpdate () {
		if (!active && player.currentMission >= 1) {
            for (int i = 0; i < materials.Length; i++) {
                materials[i].SetColor("_EmissionColor", Color.white);
            }
            active = true;
        }
	}
}
