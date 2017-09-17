using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerImage : MonoBehaviour {

	void Update() {
       // if(pane != null && player != null) {
         //   pane.transform.LookAt(pane.transform.position + player.transform.rotation * Vector3.up,
        // player.transform.rotation * Vector3.up);
        //}
    }

    string imgid;
    public GameObject pane;
    public GameObject loader;
    public GameObject player;

    void Start() {
        player = GameObject.Find("VRCamera");
    }

    private void OnTriggerEnter(Collider other)    {
        if (other.tag.Equals("InteractionCube")) {
            imgid = other.name;
            loader.SetActive(true);
            //pane.SetActive(true);
            pane.GetComponent<Orient>().locked = false;

            GameObject.Find("Scripts").GetComponent<FirebaseTest>().getImageFromStorage(imgid, pane, loader);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag.Equals("InteractionCube") && other.name == imgid) {
            pane.SetActive(false);
            pane.GetComponent<Orient>().locked = true;
        }
    }
}
