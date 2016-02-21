using UnityEngine;
using System.Collections;

public class PlayerArm : MonoBehaviour {

    public GameObject bullet;
    public float bulletSpeed;
    public bool shotgun = false;
    public bool machinegun = false;

    protected Collider2D PlayerCollider;

	// Use this for initialization
	void Start ()
	{
	    PlayerCollider = GetComponentInParent<Collider2D>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            if (shotgun)
            {
                Shotgun();
            }
            else if (machinegun)
            {
                StartCoroutine(MachineGun());
            }
            else
                Shoot();
        }
    }

    void Shoot ()
    {
        Vector2 mouseOnScreen = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angle = Mathf.Atan2(mouseOnScreen.y - transform.position.y, mouseOnScreen.x - transform.position.x) * Mathf.Rad2Deg;

        GameObject projectile = (GameObject)Instantiate(bullet, gameObject.transform.position, Quaternion.identity);

        projectile.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));

        var vel = (mouseOnScreen - (Vector2)transform.position) / Vector2.Distance(transform.position, mouseOnScreen) * Time.deltaTime * bulletSpeed;
        projectile.GetComponent<Bullet>().vel = vel;

        Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), PlayerCollider);
    }

    void Shotgun()
    {
        for (int i = 0; i < 6; i++)
        {
            float randX = Random.Range(-.5f, .5f);
            float randY = Random.Range(-.5f, .5f);
            Vector2 mouseOnScreen = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(randX, randY, 0);
            float angle = Mathf.Atan2(mouseOnScreen.y - transform.position.y, mouseOnScreen.x - transform.position.x) * Mathf.Rad2Deg;

            GameObject projectile = (GameObject)Instantiate(bullet, gameObject.transform.position, Quaternion.identity);

            projectile.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));

            var vel = (mouseOnScreen - (Vector2)transform.position) / Vector2.Distance(transform.position, mouseOnScreen) * Time.deltaTime * bulletSpeed;
            projectile.GetComponent<Bullet>().vel = vel;

            Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), PlayerCollider);
        }
    }

    IEnumerator MachineGun()
    {
        Shoot();
        yield return new WaitForSeconds(.1f);
        Shoot();
        yield return new WaitForSeconds(.1f);
        Shoot();
    }
}
