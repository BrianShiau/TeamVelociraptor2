using UnityEngine;
using System.Collections;

public class PlayerArm : MonoBehaviour {

    public GameObject bullet;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(bullet, gameObject.transform.position, Quaternion.identity);
        }
    }
}
