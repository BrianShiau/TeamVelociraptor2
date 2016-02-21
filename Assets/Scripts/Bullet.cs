using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    public Vector3 vel;

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        transform.position += vel;
    }
}
