using UnityEngine;
using System.Collections;

public class PlayerRespawn : MonoBehaviour {

    public GameObject person;
    public GameObject player;
    public bool respawning = false;

    // Use this for initialization
    void Start () {
        Instantiate(person, transform.position, Quaternion.identity);
    }
	
	// Update is called once per frame
	void Update () {
        player = GameObject.Find("player(Clone)");

        if(player == null && respawning == false)
        {
            StartCoroutine(Respawn());
        }
	}

    //Respawns the player with a delay
    IEnumerator Respawn()
    {
        respawning = true;
        yield return new WaitForSeconds(5);

        if (player == null && person != null)
        {
            Instantiate(person, transform.position, Quaternion.identity);
        }
        respawning = false;
    }
}
