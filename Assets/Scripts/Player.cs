using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public float speed = 10.0f;
    public float climbSpeed = 10f;
    public bool grounded = false;
    public bool climbing = false;
    public bool onCorner = false;
    public Rigidbody2D rb;
    public Collider2D playerBounds;

    // Use this for initialization
    void Start () {
        rb = gameObject.GetComponent<Rigidbody2D>();
        playerBounds = gameObject.GetComponent<Collider2D>();
	}
	
	void Update () {
        //Jump is placed in Update for responsiveness
        if (grounded && Input.GetKey("up"))
        {
            grounded = false;
            rb.AddForce(new Vector2(0, 500));
        }
        //For climbing
        if (climbing && Input.GetKey("up"))
        {
            transform.Translate(0, climbSpeed * Time.deltaTime, 0);
        }
        if (climbing && Input.GetKey("down"))
        {
            transform.Translate(0, -climbSpeed * Time.deltaTime, 0);
        }

        //Disable gravity for climbing
        if (climbing)
            rb.gravityScale = 0;
        else
            rb.gravityScale = 3;
    }

    void FixedUpdate() {
        //Horizontal Movement
        float move = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.Translate(move, 0, 0);
    }

    void OnCollisionStay2D(Collision2D c) {
        //Used for jumping
        if (playerBounds.bounds.min.y >= c.collider.bounds.max.y - .1f && !onCorner) {
            // - .1f is for some error
            grounded = true;
        }

        //Climbing the wall
        if (c.gameObject.CompareTag("Wall") && playerBounds.bounds.min.y > c.collider.bounds.min.y - .1f)
        {
            rb.velocity = new Vector2(0,0);
            climbing = true;
        }
    }

    void OnCollisionExit2D(Collision2D c) {
        grounded = false;
        climbing = false;
    }
}
