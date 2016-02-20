using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Person : MonoBehaviour
{
    protected Rigidbody2D Rigidbody2D;
    protected Collider2D Collider2D;

    [Header("Movement")]
    public float RunSpeed;
    public float TurnTime;

    [Range(0f, 1f), Tooltip("Chance each second.")]
    public float TurnChance;

    [Header("Collision")]
    public LayerMask CollisionLayers;

    public Transform LeftSensor;
    public Transform RightSensor;
    public Transform GroundSensor;

    [Header("Animation")]
    public Animator Animator;

    public string TurningBool;
    protected int TurningBoolHash;

    [Header("Debug")]
    public State CurrentState;
    public enum State
    {
        Idle,
        Turning,
        Moving,
    }

    public bool FacingForward;
    public bool OnGround;
    public float TurnTimer;
    public float TurnChanceTimer;

    public void Reset()
    {
        RunSpeed = 1f;
        TurnTime = .2f;
        TurnChance = .01f;
        CollisionLayers = LayerMask.NameToLayer("Everything") ^ LayerMask.NameToLayer("Person");

        if (!LeftSensor) CreateLeftSensor();
        if (!RightSensor) CreateRightSensor();
        if (!GroundSensor) CreateGroundSensor();

        Animator = GetComponent<Animator>();
    }

    public void Awake()
    {
        CurrentState = State.Moving;
        FacingForward = Random.value > 0.5f;
        OnGround = false;
        TurnTimer = 0f;
        TurnChanceTimer = Random.value;
        TurningBoolHash = Animator.StringToHash(TurningBool);
    }
    
	// Use this for initialization
	public void Start ()
	{
	    Collider2D = GetComponent<Collider2D>();
	    Rigidbody2D = GetComponent<Rigidbody2D>();
	}

    public void FixedUpdate()
    {
        Physics2D.queriesStartInColliders = false;

        float vx = Rigidbody2D.velocity.x, vy = Rigidbody2D.velocity.y;

        if (CurrentState == State.Idle)
        {
            vx = 0f;
        }
        else if (CurrentState == State.Moving)
        {
            vx = FacingForward ? RunSpeed : -RunSpeed;

            if (Physics2D.Linecast(transform.position, LeftSensor.position, CollisionLayers) ||
                Physics2D.Linecast(transform.position, RightSensor.position, CollisionLayers))
            {
                TurnAround();
            }

            if ((TurnChanceTimer += Time.deltaTime) > 1f)
            {
                TurnChanceTimer = 0f;

                if(Random.value < TurnChance)
                    TurnAround();
            }
        }
        else if (CurrentState == State.Turning)
        {
            vx = 0f;

            if ((TurnTimer += Time.deltaTime) > TurnTime)
            {
                TurnTimer = 0f;

                FacingForward = !FacingForward;
                CurrentState = State.Moving;

                if (Animator)
                {
                    if(TurningBoolHash != 0) Animator.SetBool(TurningBoolHash, false);
                }
            }
        }

        Rigidbody2D.velocity = new Vector2(vx, vy);

        Physics2D.queriesStartInColliders = true;
    }

    public void TurnAround()
    {
        TurnTimer = 0f;
        CurrentState = State.Turning;

        if (Animator)
        {
            if (TurningBoolHash != 0)
                Animator.SetBool(TurningBoolHash, true);
        }
    }

    protected void CreateSensors()
    {
        if(LeftSensor) Destroy(LeftSensor.gameObject);
        CreateLeftSensor();

        if(RightSensor) Destroy(RightSensor.gameObject);
        CreateRightSensor();

        if (GroundSensor) Destroy(GroundSensor.gameObject);
        CreateGroundSensor();
    }

    protected void CreateLeftSensor()
    {
        LeftSensor = new GameObject("Left Sensor").transform;
        LeftSensor.SetParent(transform);
        LeftSensor.position = new Vector3(GetComponent<Collider2D>().bounds.min.x, transform.position.y);
    }

    protected void CreateRightSensor()
    {
        RightSensor = new GameObject("Right Sensor").transform;
        RightSensor.SetParent(transform);
        RightSensor.position = new Vector3(GetComponent<Collider2D>().bounds.max.x, transform.position.y);
    }

    protected void CreateGroundSensor()
    {
        GroundSensor = new GameObject("Ground Sensor").transform;
        GroundSensor.SetParent(transform);

        var collider2D = GetComponent<Collider2D>();
        GroundSensor.position = new Vector3(transform.position.x, collider2D.bounds.min.y - collider2D.bounds.size.y * 0.05f);
    }
}
