﻿using UnityEngine;

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

    public float EvacTime;

    [Header("Collision")]
    public LayerMask CollisionLayers;

    public Transform LeftSensor;
    public Transform RightSensor;
    public Transform GroundSensor;

    [Header("Animation")]
    public Animator Animator;

    public string TurningBool;
    protected int TurningBoolHash;

    public ChannelVisual EvacVisual;

    [Header("Sound")]
    public AudioSource EvacuationLoop;
    public AudioClip EvacuationClip;

    [Header("Debug")]
    public State CurrentState;
    public enum State
    {
        Idle,
        Turning,
        Moving,
        Evacuating,
    }

    public bool FacingForward;
    public bool OnGround;
    public float TurnTimer;
    public float TurnChanceTimer;
    public float EvacTimer;

    public Player EvacuatingPlayer;

    public void Reset()
    {
        RunSpeed = 1f;
        TurnTime = .2f;
        TurnChance = .2f;
        EvacTime = 3f;
        CollisionLayers = LayerMask.NameToLayer("Everything") & ~(1 << LayerMask.NameToLayer("People"));

        if (!LeftSensor) CreateLeftSensor();
        if (!RightSensor) CreateRightSensor();
        if (!GroundSensor) CreateGroundSensor();

        Animator = GetComponent<Animator>();
    }

    public void Awake()
    {
        CurrentState = State.Moving;
        FacingForward = Random.value > 0.5f;
        EvacTimer = 0f;
        OnGround = false;
        TurnTimer = 0f;
        TurnChanceTimer = Random.value;
        TurningBoolHash = Animator.StringToHash(TurningBool);
        EvacuatingPlayer = null;
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

            if ((!FacingForward && Physics2D.Linecast(transform.position, LeftSensor.position, CollisionLayers)) ||
                (FacingForward && Physics2D.Linecast(transform.position, RightSensor.position, CollisionLayers)))
            {
                TurnAround();
            }
            else if (OnGround && !Physics2D.Linecast(transform.position, GroundSensor.position, CollisionLayers))
            {
                TurnAround();
                OnGround = false;
            }
            else if (Physics2D.Linecast(transform.position, GroundSensor.position, CollisionLayers))
            {
                OnGround = true;
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

        if (CurrentState != State.Evacuating)
        {
            if (EvacuatingPlayer != null)
            {
                if(EvacuationLoop) EvacuationLoop.Play();
                if (EvacVisual) EvacVisual.gameObject.SetActive(true);

                CurrentState = State.Evacuating;
                EvacTimer = 0f;
            }
        }
        else
        {
            if (EvacuatingPlayer == null)
            {
                if (EvacuationLoop) EvacuationLoop.Stop();
                if (EvacVisual) EvacVisual.gameObject.SetActive(false);

                CurrentState = State.Moving;
                EvacTimer = 0f;
            }
            else
            {
                vx = 0f;

                if ((EvacTimer += Time.deltaTime) > EvacTime)
                {
                    Evacuate(EvacuatingPlayer);
                }
                else
                {
                    if(EvacVisual)
                    {
                        EvacVisual.ChannelTime = EvacTime;
                        EvacVisual.TimeRemaining = EvacTime - EvacTimer;
                    }
                }
            }
        }

        Rigidbody2D.velocity = new Vector2(vx, vy);

        Physics2D.queriesStartInColliders = true;
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        if (!IsLethal(other.gameObject)) return;

		other.gameObject.GetComponent<Punch>().OwnerHero.score++;
        Die();
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public void Evacuate(Player player)
    {
        if (EvacuationClip) AudioSource.PlayClipAtPoint(EvacuationClip, transform.position);

        ++player.PeopleEvacuated;
        Destroy(gameObject);
    }

    public bool IsLethal(GameObject gameObject)
    {
        return gameObject.GetComponent<Punch>();
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
