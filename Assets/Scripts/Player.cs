using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public float speed = 10.0f;
    public bool grounded = false;
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
    }

    void FixedUpdate() {
        //Horizontal Movement
        float move = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.Translate(move, 0, 0);
        print(move);
    }

    void OnCollisionStay2D(Collision2D c) {
        if (playerBounds.bounds.min.y >= c.collider.bounds.max.y - .1f) {
            // - .1f is for some error
            grounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D c) {
        grounded = false;
    }
}
