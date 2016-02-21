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
            Vector2 positionOnScreen = transform.position;
            Vector2 mouseOnScreen = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float angle = Mathf.Atan2(mouseOnScreen.y - positionOnScreen.y, mouseOnScreen.x - positionOnScreen.x) * Mathf.Rad2Deg;

            GameObject projectile = (GameObject) Instantiate(bullet, gameObject.transform.position, Quaternion.identity);

            projectile.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));

            var vel = (mouseOnScreen - (Vector2)transform.position) * Time.deltaTime * .1f;
            print(vel);
            projectile.GetComponent<Bullet>().vel = vel;
            //projectile.GetComponent<Rigidbody2D>().velocity 
            //    = ((mouseOnScreen - positionOnScreen) / Vector2.Distance(positionOnScreen, mouseOnScreen)) * bulletSpeed;
        }
    }
}
