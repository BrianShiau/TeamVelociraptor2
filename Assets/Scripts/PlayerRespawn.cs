using UnityEngine;
using System.Collections;

public class PlayerRespawn : MonoBehaviour {

    public GameObject person;
    public GameObject player;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        player = GameObject.Find("player(Clone)");
        if (player == null && person != null)
        {
            Instantiate(person, transform.position, Quaternion.identity);
        }
	}
}
