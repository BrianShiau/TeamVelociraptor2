using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Player : MonoBehaviour {

    public float speed = 10.0f;
    public float climbSpeed = 10f;
    public float move = 0;

    [SerializeField]
    private bool grounded, climbing, nexttowall, onplatform;

    public Rigidbody2D rb;
    public Collider2D playerBounds;
    public Animator anim;

    public HashSet<Collider2D> WallCollisions;

    public void Awake()
    {
        WallCollisions = new HashSet<Collider2D>();
    }

    // Use this for initialization
    void Start () {
        rb = gameObject.GetComponent<Rigidbody2D>();
        playerBounds = gameObject.GetComponent<Collider2D>();
        anim = gameObject.GetComponent<Animator>();
	}
	
	void Update () {
        anim.SetBool("PlayerJump", !grounded && !climbing);
        if (move == 0)
            anim.SetBool("PlayerWalk", false);
        else
            anim.SetBool("PlayerWalk", true);


        //Jump is placed in Update for responsiveness
        if (Input.GetKey("up")){
            if (climbing)
            {
                transform.Translate(0, climbSpeed * Time.deltaTime, 0);

            } else {
                if (nexttowall){
                    rb.velocity = new Vector2(0,0);
                    gameObject.layer = LayerMask.NameToLayer("Hero Climbing");
                } else if (grounded){
                    grounded = false;
                    rb.AddForce(new Vector2(0, 500));
                }
            }
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
        move = Input.GetAxis("Horizontal2") * speed * Time.deltaTime;
        transform.Translate(move, 0, 0);

        CheckWallCollisions();
    }

    protected void CheckWallCollisions()
    {
        WallCollisions.RemoveWhere(d => !d.enabled);
        if (!(nexttowall || onplatform) && !WallCollisions.Any()) {
            climbing = false;
            gameObject.layer = LayerMask.NameToLayer("Hero");
        }
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        //Player dies to punch
        if (c.gameObject.GetComponent<Punch>())
        {
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        anim.SetBool("PlayerDeath", true);
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

    void OnCollisionStay2D(Collision2D c) {
        //Used for jumping
        if (!climbing
                && playerBounds.bounds.min.y >= c.collider.bounds.max.y - .1f) {
            // - .1f is for some error
            grounded = true;
        }

        //Climbing the wall
        if (gameObject.layer == LayerMask.NameToLayer("Hero Climbing")
                && c.gameObject.CompareTag("Wall")
                && playerBounds.bounds.min.y > c.collider.bounds.min.y - .1f)
        {
            if (!WallCollisions.Contains(c.collider))
                WallCollisions.Add(c.collider);
            climbing = true;
            rb.velocity = new Vector2(0,0);
        }
        //Climbing the platforms 
        if (gameObject.layer == LayerMask.NameToLayer("Hero Platform")
                && c.gameObject.CompareTag("Platform")
                && playerBounds.bounds.min.y > c.collider.bounds.min.y - .1f)
        {
            if (!WallCollisions.Contains(c.collider))
                WallCollisions.Add(c.collider);
            grounded = true;
            onplatform = true;
        }
    }

    void OnCollisionExit2D(Collision2D c)
    {
        WallCollisions.Remove(c.collider);

        grounded = false;
    }

    void OnTriggerStay2D (Collider2D other){
        if (other.tag == "Wall"){
            nexttowall = true;
        } else if (other.tag == "Platform" && rb.velocity.y < 0){
            gameObject.layer = LayerMask.NameToLayer("Hero Platform");
            onplatform = true;
        }
    }

    void OnTriggerExit2D (Collider2D other){
        if (other.tag == "Wall"){
            nexttowall = false;
        } else if (other.tag == "Platform"){
            onplatform = false;
        }
    }
}
