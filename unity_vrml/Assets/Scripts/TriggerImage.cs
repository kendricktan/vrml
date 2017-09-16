using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerImage : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("LeftHand") || other.tag.Equals("RightHand")) {
            // pop up image
            Debug.Log("SHOW IMAGE");
        }
    }
}
