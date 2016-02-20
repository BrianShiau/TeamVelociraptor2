using UnityEngine;
using System.Collections;

public class PlayerArm : MonoBehaviour {

    public GameObject bullet;
    public float bulletSpeed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject projectile = (GameObject) Instantiate(bullet, gameObject.transform.position, Quaternion.identity);

            Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);
            Vector2 positionOnScreen = Camera.main.WorldToViewportPoint(transform.position);

            projectile.GetComponent<Rigidbody2D>().velocity = (mouseOnScreen - positionOnScreen) * bulletSpeed;
        }
    }
}
