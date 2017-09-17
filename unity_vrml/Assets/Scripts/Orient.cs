using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orient : MonoBehaviour {
    public bool locked = false;
    // Update is called once per frame
    void Start() {
        transform.position += new Vector3(0, .1f, 0);
    }
    void Update() {
        transform.rotation = Camera.main.transform.rotation * Quaternion.Euler(new Vector3(90f, 0f, 180f));
    }
}