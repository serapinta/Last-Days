using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchGravity : MonoBehaviour {

    public GameObject gravityDevice;

    private bool makeRotation = false;

    private bool inverted = false;

    private bool lockRotation = false;

    private float rotationSpeed = 0f;

    private FirstPersonController myFPS;

    private void Start() {
        myFPS = GetComponent<FirstPersonController>();
    }

    // Update is called once per frame
    void LateUpdate() {
        
        InvertGravity();
    }

    private void InvertGravity() {

        if (!gravityDevice) {

            if (Input.GetKeyDown(KeyCode.F) && !makeRotation) {
                lockRotation = true;
                Physics.gravity *= -1;
                if (!inverted) {
                    inverted = true;
                    myFPS.m_MouseLook.invertYAxis = true;
                    GetComponent<CharacterController>().Move(new Vector3(0, 1, 0));
                } else {
                    inverted = false;
                    myFPS.m_MouseLook.invertYAxis = false;
                    GetComponent<CharacterController>().Move(new Vector3(0, -1, 0));
                }
                makeRotation = true;
            }

            if (!lockRotation) {
                if (inverted) {
                    transform.Rotate(0, 0, 180);
                } else {
                    transform.Rotate(0, 0, 0);
                }
            }

            if (makeRotation) {
                if (inverted) {
                    transform.Rotate(0, 0, rotationSpeed);
                    rotationSpeed += Time.deltaTime * 180;
                    if (transform.eulerAngles.z >= 170 && transform.eulerAngles.z <= 190) {
                        makeRotation = false;
                        lockRotation = false;
                        rotationSpeed = 180;
                    }
                } else if (!inverted) {
                    transform.Rotate(0, 0, rotationSpeed);
                    rotationSpeed -= Time.deltaTime * 180;
                    if (transform.eulerAngles.z <= 10) {
                        makeRotation = false;
                        lockRotation = false;
                        rotationSpeed = 0;
                    }
                }
            }
        }
    }
}
