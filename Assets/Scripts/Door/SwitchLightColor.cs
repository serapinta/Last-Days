using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchLightColor : MonoBehaviour {

    public bool guardedDoor;

    [SerializeField]
    private Player player;
    [SerializeField]
    private Color color1;
    [SerializeField]
    private Color color2;
    [SerializeField]
    private Color specialColor;
    private Renderer myRenderer;
    private Material myMaterial;

	// Use this for initialization
	void Start () {
        myRenderer = GetComponent<Renderer>();
        myMaterial = myRenderer.material;

    }
	
	// Update is called once per frame
	void Update () {
        if (guardedDoor) {
            switch (player.currentMission) {
                case 0:
                    myMaterial.SetColor("_EmissionColor", color1);
                    break;
                case 1:
                    myMaterial.SetColor("_EmissionColor", color2);
                    break;
                default:
                    myMaterial.SetColor("_EmissionColor", specialColor);
                    break;
            }
        } else {
            if (player.currentMission == 0) {
                myMaterial.SetColor("_EmissionColor", color1);
            } else {
                myMaterial.SetColor("_EmissionColor", color2);
            }
        }
        
        
    }
}
