using UnityEngine;
using System.Collections;

public class Elevator : MonoBehaviour {

    private Animator myAnimator;

    private float _waitTime = 5;
    private bool _startTimer;

    private void Start() {
        myAnimator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player" && _waitTime >= 5) {
            other.transform.SetParent(gameObject.transform);
            _waitTime = 0;
            if (myAnimator.GetBool("goingDown")) {
                myAnimator.SetBool("goingDown", false);
            } else if (!myAnimator.GetBool("goingDown")) {
                myAnimator.SetBool("goingDown", true);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            _startTimer = true;
        }
        other.transform.SetParent(null);
        StopAllCoroutines();
    }

    // Update is called once per frame
    void Update () {
        if (_startTimer) {
            _waitTime += Time.deltaTime;
            if (_waitTime >= 5) {
                _startTimer = false;
            }
        }
	}
}
