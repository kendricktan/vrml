using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereSpawner : MonoBehaviour {

    public GameObject sphere;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void Spawn(Vector3 spawnPosition)
    {
        Quaternion rot = new Quaternion(0, 0, 0, 0);
        // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
        Instantiate(sphere, spawnPosition, rot);

    
    }
}
