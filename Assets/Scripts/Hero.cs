﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Jolly;

public class Hero : MonoBehaviour
{
    public Animator anim;

	public float ScaleAdjustment;
	public int ScaleIterations;
	public Vector2 HUDPosition
	{
		get
		{
			switch (this.PlayerIndex)
			{
			case 1: return new Vector2 (15, 35);
			case 2: return new Vector2 (495, 35);
			case 3: return new Vector2 (975, 35);
			case 4: return new Vector2 (1455, 35);
			}
			return Vector2.zero;
		}
	}

    public bool CanPunch;

	public float SpawnMagnitude;
	public GameObject GroundDetector;
	public GameObject ProjectileEmitLocator;
	public GameObject ChannelLocator;
	public GameObject CounterLocator;
	public GameObject Projectile;
	public GameObject Punch;
	public GameObject ProjectileExplosion;
	public GameObject StunVisual;
	public GameObject ChannelVisual;
	public GameObject MaxGrowthVisual;
	public bool EnableDoubleJump;
	public float ChannelTime;
	public float RespawnTime;
	public float RespawnTimeIncreasePerDeath;
	public float StunTime;
	public float JumpForgivenessTimeAmount;
	public int PlayerIndex
	{
		get
		{
			return 1+this.HeroController.PlayerNumber;
		}
	}
	public GUIText HUDText;
	public float TimeAtMaxSize;

	public HeroController HeroController;

	public float ProjectileLaunchVelocity;
	public float ProjectileDelay;
	private float TimeUntilNextProjectile = 0.0f;

	public bool FacingRight = true;

	private bool Stomping = false;
	private float RespawnTimeCalculated = 0.0f;
	private float RespawnTimeLeft = 0.0f;
	private float TimeLeftStunned = 0.0f;
	private float TimeSpentChanneling = 0.0f;
	private bool IsChanneling = false;
	private GameObject ChannelVisualInstance;
	private GameObject MaxVisualInstance;
	private GameObject StunVisualInstance;
	public bool CanDoubleJump;
	private bool GroundedLastFrame;
	private float StartScale;
	private float StartWidth;
	private float JumpForgivenessTimeLeft;
	private GameObject MaxSizeSound;
	private int NumDeaths;

	public Sprite[] BodySprites;
	public Sprite[] ProjectileSprites;
	public Sprite[] ProjectileExplosions;
	public Sprite ProjectileSprite;
	public Sprite ProjectileExplosionSprite;

	public int health;
	public bool victory;
	public GameObject healthBar;

    ////////////// My fucking amazing code ////////////////
    public float speed = 1.0f;
    public float climbSpeed = 1.0f;

    [SerializeField]
    private bool grounded, climbing, nexttowall, onplatform;

    public Rigidbody2D rb;
    public Collider2D playerBounds;

    public HashSet<Collider2D> WallCollisions;
    //////////////////////////////////////////////////////

    [SerializeField] private int _score;
    public int score
    {
        get { return _score; }
        set
        {
            if (_score == value) return;

            var previousScore = _score;
            _score = value;
            CheckPowerup(previousScore);
        }
    }

    [Header("Powerups")]
    public AudioSource PowerupAudio;
    public AudioClip LaserFireClip;
    public AudioSource DoubleJumpAudio;
    public int DoubleJumpScore;
    public int LaserScore;

    protected void CheckPowerup(int previousScore)
    {
        if (previousScore < DoubleJumpScore && score >= DoubleJumpScore)
        {
            this.AddPowerup<MonsterDoubleJump>(10f);
            if(PowerupAudio) PowerupAudio.Play();
        }
        else if (previousScore < LaserScore && score >= LaserScore)
        {
            var laser = this.AddPowerup<MonsterLaser>();
            laser.Clip = LaserFireClip;
            if(PowerupAudio) PowerupAudio.Play();
        }
    }

    public void Awake()
    {
        WallCollisions = new HashSet<Collider2D>();
    }

	void Start ()
	{
        anim = gameObject.GetComponentInChildren<Animator>();

		this.HeroController = this.GetComponent<HeroController>();
		//this.GetComponentInChildren<SpriteRenderer>().sprite = this.BodySprites[this.HeroController.PlayerNumber];
		this.ProjectileSprite = this.ProjectileSprites[this.HeroController.PlayerNumber];
		this.ProjectileExplosionSprite = this.ProjectileExplosions[this.HeroController.PlayerNumber];
		this.StartScale = this.scale;
		this.SetGrowStage(0);
		this.StartWidth = this.GetComponent<Collider2D>().bounds.size.x;
		this.RespawnTimeCalculated = this.RespawnTime;

	    CanPunch = true;

		this.groundMask = LayerMask.NameToLayer ("Ground");

		this.health = 20;
		score = 0;
		victory = false;

        rb = gameObject.GetComponent<Rigidbody2D>();
        playerBounds = gameObject.GetComponent<Collider2D>();
	}

	private float scale
	{
		set
		{
			//float minYOld = this.GetComponent<Collider2D>().bounds.min.y;
			this.transform.localScale = new Vector3((this.FacingRight ? 1.0f : -1.0f) * value, value, 1.0f);
			//float minYNew = this.GetComponent<Collider2D>().bounds.min.y;
			//Vector3 v = this.transform.position;
			//this.transform.position = new Vector3(v.x, v.y + minYOld - minYNew, v.z);
		}
		get
		{
			return this.transform.localScale.y;
		}
	}

	void OnGUI()
	{
		//this.DrawHUD(this.HUDPosition);
	}

	void SetDoubleJumpAllowed()
	{
		if (this.EnableDoubleJump)
		{
			this.CanDoubleJump = true;
		}
	}

	void DrawHUD(Vector2 position)
	{
		float iconSizeWidth = 50;
		float heartSizeWidth = 35;

		float xPosition = position.x;

		/*Texture badge = (Texture)Resources.Load(string.Format("p{0}_badge", this.PlayerIndex), typeof(Texture));
		GUI.DrawTexture(new Rect(xPosition / 1920.0f * Screen.width, (position.y - iconSizeWidth * 0.5f) / 1080.0f * Screen.height, iconSizeWidth / 1920.0f * Screen.width, iconSizeWidth / 1920.0f * Screen.width), badge);
		xPosition += (iconSizeWidth * 1.5f);*/

		bool drawHearts = false;
		if (drawHearts)
		{
			Texture heart = (Texture)Resources.Load("heart_full", typeof(Texture));
			GUI.DrawTexture(new Rect(xPosition / 1920.0f * Screen.width, (position.y - heartSizeWidth * 0.5f) / 1080.0f * Screen.height, heartSizeWidth / 1920.0f * Screen.width, heartSizeWidth / 1920.0f * Screen.width), heart);
			xPosition += (heartSizeWidth * 1.1f);

			GUI.DrawTexture(new Rect(xPosition / 1920.0f * Screen.width, (position.y - heartSizeWidth * 0.5f) / 1080.0f * Screen.height, heartSizeWidth / 1920.0f * Screen.width, heartSizeWidth / 1920.0f * Screen.width), heart);
			xPosition += (heartSizeWidth * 1.1f);

			GUI.DrawTexture(new Rect(xPosition / 1920.0f * Screen.width, (position.y - heartSizeWidth * 0.5f) / 1080.0f * Screen.height, heartSizeWidth / 1920.0f * Screen.width, heartSizeWidth / 1920.0f * Screen.width), heart);
			xPosition += (iconSizeWidth * 1.5f);
		}

		GUIStyle style = new GUIStyle("label");
		style.font = this.HUDText.font;
		style.fontSize = (int)(Screen.width * 0.027027f);
		style.alignment = TextAnchor.UpperLeft;

		/*string displayString = "Flawless!";
		if (this.RespawnTimeLeft > 0)
		{
			displayString = string.Format("Back in {0}s!", ((int)Math.Ceiling(this.RespawnTimeLeft)).ToString());
		}
		else if (this.NumDeaths == 1)
		{
 			displayString = string.Format("{0} Death", 1);
		}
		else if (this.NumDeaths > 0)
		{
			displayString = string.Format("{0} Deaths", this.NumDeaths);
		}*/
		string displayString = string.Format("Score: {0}", score);
		healthBar.transform.localScale = new Vector3(health/20.0f, 1.0f, 1.0f);

		this.DrawOutlineText(new Rect(20, 30, Screen.width, Screen.height), displayString, style, Color.black, Color.white, 1);

        if (this.health <= 0)
        {
            StartCoroutine(BlobDie());
            style.fontSize = (int)(Screen.width * 0.1f);
            style.alignment = TextAnchor.MiddleCenter;
            string winningText = string.Format("HUMANITY IS SAVED!");
            this.DrawOutlineText(new Rect(0, 0, Screen.width, Screen.height), winningText, style, Color.black, Color.green, 1);
        }
        if (this.victory)
        {
            style.fontSize = (int)(Screen.width * 0.1f);
            style.alignment = TextAnchor.MiddleCenter;
            string winningText = string.Format("THE MONSTER WINS!");
            this.DrawOutlineText(new Rect(0, 0, Screen.width, Screen.height), winningText, style, Color.black, Color.green, 1);
        }
    }

    bool CanJumpOffGround()
    {
        return (this.grounded || this.JumpForgivenessTimeLeft > 0.0f);
    }

    IEnumerator BlobAttack()
    {
        anim.SetBool("BlobAttack", true);
        yield return new WaitForSeconds(.2f);
        anim.SetBool("BlobAttack", false);
    }

    IEnumerator BlobDie()
    {
        anim.SetBool("BlobDie", true);
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

    void Update()
    {
        if (grounded && !climbing)
            anim.SetBool("BlobJump", false);
        else
            anim.SetBool("BlobJump", true);

        if (climbing)
            anim.SetBool("BlobClimb", true);
        else
            anim.SetBool("BlobClimb", false);

        float HoriAxis = this.HeroController.HorizontalMovementAxis;
        if (HoriAxis != 0)
            anim.SetBool("BlobWalk", true);
        else
            anim.SetBool("BlobWalk", false);
        /*
		if (this.RespawnTimeLeft > 0.0f)
		{
			this.transform.position = new Vector3(0.0f, -20.0f, 0.0f);

			this.RespawnTimeLeft -= Time.deltaTime;
			if (this.RespawnTimeLeft < 0.0)
			{
				this.Respawn ();
			}
		}
        */


        this.JumpForgivenessTimeLeft -= Time.deltaTime;

		bool canAct = !this.IsChanneling && !this.Stomping && !this.IsStunned();
		if (canAct)
		{
			if (this.HeroController.Shooting && CanPunch && this.TimeUntilNextProjectile < 0.0f)
			{
                StartCoroutine(BlobAttack());

                this.TimeUntilNextProjectile = this.ProjectileDelay;
				//GameObject projectile = (GameObject)GameObject.Instantiate(this.Projectile, this.ProjectileEmitLocator.transform.position, Quaternion.identity);
				GameObject projectile = (GameObject)GameObject.Instantiate(this.Punch, this.ProjectileEmitLocator.transform.position, Quaternion.identity);
				projectile.GetComponent<SpriteRenderer>().sprite = this.ProjectileSprite;
				projectile.GetComponent<Punch>().OwnerHero = this;
				projectile.transform.localScale = this.transform.localScale;
				float launchVelocity = (this.FacingRight ? 1.0f : -1.0f) * this.ProjectileLaunchVelocity;
				projectile.GetComponent<Punch>().Velocity = new Vector2(launchVelocity, 0.0f);
				SoundFX.Instance.OnHeroFire(this);
				Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), this.GetComponent<Collider2D>());
			}


			//no stomping
			/*bool controllerIssuedStomp = (this.HeroController.Jump && !this.CanDoubleJump);
			if (controllerIssuedStomp && !this.CanJumpOffGround() && this.canStomp)
			{
				this.canStomp = false;
				this.Stomping = true;
				this.velocity = new Vector2(0.0f, this.StompSpeed);
				SoundFX.Instance.OnHeroStompStart(this);
			}*/
		}

        /*
		if (this.HeroController.GetResetGame)
		{
			GameObject scoreKeeper = GameObject.Find("ScoreKeeper");
			scoreKeeper.GetComponent<ScoreKeeper>().ResetGame();
		}
        */
		if (this.grounded)
		{
			this.SetDoubleJumpAllowed();
		}
        /*
		bool canMove = !this.IsChanneling && !this.Stomping && !this.IsStunned();

		if (canMove)
		{
			this.velocity = new Vector2 (this.HeroController.HorizontalMovementAxis * this.MaxNewSpeed, this.velocity.y);
		//}
		else
		{
			this.velocity = new Vector2 (this.velocity.x * (1.0f - Mathf.Clamp01 (Time.deltaTime)), this.velocity.y);
		}
        */
		if (canAct)
		{
			if (this.HeroController.Jump)
			{
				bool isJumpingOffGround = this.CanJumpOffGround();
				if (isJumpingOffGround || this.CanDoubleJump)
				{
					bool doubleJumped = false;

					if (!isJumpingOffGround)
					{
						this.CanDoubleJump = false;
						doubleJumped = true;
                        
                        // wonky double jump ????

					    if (DoubleJumpAudio) DoubleJumpAudio.Play();
                        rb.velocity = new Vector2(rb.velocity.x, this.Jump);
                    }

                    /*
					if (doubleJumped)
					{
						SoundFX.Instance.OnHeroDoubleJumped(this);
					}
					else
					{
						SoundFX.Instance.OnHeroJumped(this);
					}
                    */
				}
			}
		}
        /*
		this.canChannelGrow = !this.falling && Physics2D.Linecast(this.transform.position, this.GroundDetector.transform.position, 1 << LayerMask.NameToLayer ("Ground"));

		if (this.canChannelGrow)
		{
			this.canStomp = true;
		}

		if (this.IsChanneling && (this.HeroController.GetBiggerEnd || !this.canChannelGrow))
		{
			this.StopChannelGrow();
		}
		else if (this.HeroController.GetBiggerHold)
		{
			if (this.IsChanneling)
			{
				this.TimeSpentChanneling += Time.deltaTime;

				if (this.TimeSpentChanneling > this.ChannelTime)
				{
					this.StopChannelGrow();
					this.Grow();
				}
			}
			else if (canAct && this.CanGrow ())
			{
				this.StartChannelGrow();
				this.velocity = new Vector2 (0.0f, this.velocity.y);
			}
		}
        */

        ////////////// My fucking amazing code ////////////////
        float vertaxis = this.HeroController.VerticalMovementAxis;
        if (vertaxis > 0){
            if (climbing) {
                transform.Translate(0, climbSpeed * Time.deltaTime, 0);
            } else {
                if (nexttowall){
                    rb.velocity = new Vector2(0,0);
                    gameObject.layer = LayerMask.NameToLayer("Hero Climbing");
                } else if (grounded){
                    grounded = false;
                    rb.AddForce(new Vector2(0, 500000000/2));
                }
            }
        }
        if (climbing && vertaxis < 0)
        {
            transform.Translate(0, -climbSpeed * Time.deltaTime, 0);
        }
        
        //Disable gravity for climbing
        if (climbing)
            rb.gravityScale = 0;
        else
            rb.gravityScale = 3;

        ///////////////////////////////////////////////////////
	}

	public float StaticMargin = 0.2f;
	public float FallingMargin = 0.5f;
	public float Gravity = 6.0f;
	public float MaxFall = 200.0f;
	public float StompSpeed;
	public float StompGravity = 6.0f;
	public float MaxStompFall;
	public float Jump = 200.0f;
	public float Acceleration = 4.0f;
	public float MaxNewSpeed = 150.0f;
	public float GrowPopSpeed = 1.0f;
	private bool canChannelGrow;

	private Rect box;
	private Vector2 velocity = Vector2.zero;
	private bool falling = true;
	private bool canStomp = true;
	private int groundMask;

	void FixedUpdate ()
	{
		var bounds = this.GetComponent<Collider2D>().bounds;
		this.box = Rect.MinMaxRect (bounds.min.x, bounds.min.y, bounds.max.x, bounds.max.y);

		if (this.TimeLeftStunned > 0.0f)
		{
			this.TimeLeftStunned -= Time.fixedDeltaTime;

			if (this.TimeLeftStunned <= 0.0f)
            {
                this.StopStun();
            }
        }


        /*
		if (!this.grounded)
		{
			if (this.Stomping)
			{
				this.velocity = new Vector2(this.velocity.x, Mathf.Max (this.velocity.y - this.StompGravity, -this.MaxStompFall));
			}
			else
			{
				this.velocity = new Vector2(this.velocity.x, Mathf.Max (this.velocity.y - this.Gravity, -this.MaxFall));
			}
		}

		this.falling = this.velocity.y < 0;
        */

		bool hitSomething = false;
		RaycastHit2D raycastHit;
        /*
		if (grounded || falling)
		{
			Vector3 startPoint = new Vector3(this.box.xMin + this.StaticMargin, this.box.yMin + this.StaticMargin, this.transform.position.z);
			Vector3 endPoint   = new Vector3(this.box.xMax - this.StaticMargin, startPoint.y, startPoint.z);

            float distance = this.StaticMargin + (this.grounded ? this.StaticMargin : Mathf.Abs (this.velocity.y * this.FallingMargin * Time.fixedDeltaTime));
			int verticalRays = Mathf.Max (3, Mathf.CeilToInt ((endPoint.x - startPoint.x) / this.StartWidth));

			for (int i = 0; i < verticalRays; ++i)
			{
				Vector2 origin = Vector2.Lerp (startPoint, endPoint, (float)i / (float)(verticalRays - 1));

				for (int mask = 0; mask < 2; ++mask)
				{
					if (mask == 0)
					{
						int oldLayer = this.gameObject.layer;
						this.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
						raycastHit = Physics2D.Linecast(origin, origin - new Vector2(0.0f, distance), (1<< LayerMask.NameToLayer("Default")));
						this.gameObject.layer = oldLayer;
					}
					else
					{
						raycastHit = Physics2D.Linecast(origin, origin - new Vector2(0.0f, distance), (1 << this.groundMask));
					}


					if (raycastHit.collider != null)
					{
						bool bounce = false;
						hitSomething = true;
						if (!grounded)
						{
							Hero hero = raycastHit.collider.gameObject.GetComponent<Hero>();

							if (Stomping)
							{
								if (null == hero)
								{
									SoundFX.Instance.OnHeroStompLand(this);
								}
								else if (this.GetGrowStage() > hero.GetGrowStage())
								{
									SoundFX.Instance.OnHeroStompLandSquish(this);
									hero.Die();
								}
								else
								{
									SoundFX.Instance.OnHeroStompLandStun(this);
									hero.Stun();
									bounce = true;
								}
							}
							else
							{
								if (hero)
								{
									bounce = true;
									SoundFX.Instance.OnHeroJumped(this);
								}
								else
								{
									SoundFX.Instance.OnHeroLanded(this);
								}
							}
						}
						Stomping = false;
						this.JumpForgivenessTimeLeft = this.JumpForgivenessTimeAmount;
						grounded = true;

						if (falling)
						{
							this.transform.Translate (Vector3.down * (raycastHit.distance - this.StaticMargin));
						}
						falling = false;
						if (bounce)
						{
							this.CanDoubleJump = false;
							velocity = new Vector2 (velocity.x, this.Jump);
						}
						else
						{
							this.SetDoubleJumpAllowed();
							velocity = new Vector2 (velocity.x, Mathf.Max (0.0f, velocity.y));
						}

						i = verticalRays;
						break;
					}
				}
			}
		}

		if (!hitSomething)
		{
			grounded = false;
		}
        */


        float HoriAxis = this.HeroController.HorizontalMovementAxis;
        if ((HoriAxis > 0 && !this.FacingRight)
            || (HoriAxis < 0 && this.FacingRight))
        {
            if (!climbing) {
                this.Flip();
            } else {
                transform.Translate(
                    HoriAxis * speed * Time.deltaTime, 0, 0);
            }
        } else if (!climbing){
            transform.Translate(
                HoriAxis * speed * Time.deltaTime, 0, 0);
        }
		this.TimeUntilNextProjectile -= Time.fixedDeltaTime;

		//this.transform.Translate (this.velocity * Time.fixedDeltaTime);
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


	void LateUpdate ()
	{
	}

	void Flip ()
	{
		this.FacingRight = !this.FacingRight;
		this.scale = this.scale;
	}

	public bool IsAlive()
	{
		return (this.RespawnTimeLeft <= 0.0f);
	}

	public void Hit ()
	{
		if (!this.IsAlive())
		{
			return;
		}

		GameObject projectileExplosion = (GameObject)GameObject.Instantiate(this.ProjectileExplosion, this.transform.position, Quaternion.identity);
		projectileExplosion.GetComponent<SpriteRenderer>().sprite = this.ProjectileExplosionSprite;
		projectileExplosion.transform.localScale *= this.scale / this.StartScale;

		if (this.GetComponent<ShieldBuff>().enabled)
		{
			this.GetComponent<ShieldBuff>().enabled = false;
		}
		else
		{
			//lose health?
			this.health--;
			//this.Die(attackingHero);
		}
	}

	void Die()
	{
		if (!this.IsAlive())
		{
			return;
		}

		AudioSourceExt.StopClipOnObject(this.MaxSizeSound);
		Destroy(this.MaxSizeSound);

		SoundFX.Instance.OnHeroDies(this);
		this.RespawnTimeLeft = this.RespawnTimeCalculated;
		this.RespawnTimeCalculated += this.RespawnTimeIncreasePerDeath;
		this.NumDeaths++;

		this.SetGrowStage(0);
		this.StopChannelGrow();
		this.Stomping = false;

		this.TimeAtMaxSize = 0;
		this.RemoveMaxSizeVisual();
	}

	bool IsStunned()
	{
		return this.TimeLeftStunned > 0.0f;
	}

	void Stun()
	{
		this.TimeLeftStunned = this.StunTime;

		if (this.StunVisualInstance == null)
		{
			this.StunVisualInstance = (GameObject)GameObject.Instantiate(this.StunVisual, this.ChannelLocator.transform.position, Quaternion.identity);
			this.StunVisualInstance.GetComponent<StunVisual>().Hero = this;
			this.StunVisualInstance.transform.localScale = new Vector3(this.StunVisualInstance.transform.localScale.x * this.scale, this.StunVisualInstance.transform.localScale.y * this.scale, this.StunVisualInstance.transform.localScale.z * this.scale);
			this.StunVisualInstance.transform.parent = this.transform;
		}
	}

	void StopStun()
	{
		this.TimeLeftStunned = 0.0f;

		if (this.StunVisualInstance)
		{
			Destroy(this.StunVisualInstance);
		}
	}

	void StartChannelGrow()
	{
		this.TimeSpentChanneling = 0.0f;
		this.IsChanneling = true;
		this.ChannelVisualInstance = (GameObject)GameObject.Instantiate(this.ChannelVisual, this.ChannelLocator.transform.position, Quaternion.identity);
		this.ChannelVisualInstance.GetComponent<ChannelVisual>().ChannelTime = this.ChannelTime;
		this.ChannelVisualInstance.GetComponent<ChannelVisual>().Hero = this;
		this.ChannelVisualInstance.transform.localScale = new Vector3(this.ChannelVisualInstance.transform.localScale.x * this.scale, this.ChannelVisualInstance.transform.localScale.y * this.scale, this.ChannelVisualInstance.transform.localScale.z * this.scale);
	}

	void StopChannelGrow()
	{
		this.TimeSpentChanneling = 0.0f;
		this.IsChanneling = false;

		if (this.ChannelVisualInstance)
		{
			this.ChannelVisualInstance.GetComponent<ChannelVisual>().Stop();
			Destroy(this.ChannelVisualInstance);
		}
	}

	void AddMaxSizeVisual()
	{
		if (this.MaxGrowthVisual == null)
		{
			return;
		}

		this.MaxVisualInstance = (GameObject)GameObject.Instantiate(this.MaxGrowthVisual, this.ChannelLocator.transform.position, Quaternion.identity);
		this.MaxVisualInstance.transform.localScale = new Vector3(this.MaxVisualInstance.transform.localScale.x * this.scale, this.MaxVisualInstance.transform.localScale.y * this.scale, this.MaxVisualInstance.transform.localScale.z * this.scale);
		this.MaxVisualInstance.transform.parent = this.transform;
	}

	void RemoveMaxSizeVisual()
	{
		Destroy(this.MaxVisualInstance);
	}

	bool CanGrow()
	{
		return this.IsAlive() && this.GetGrowStage() < this.ScaleIterations && this.grounded && !this.IsStunned ();
	}

	bool CanGrowByPickup()
	{
		return this.IsAlive() && this.GetGrowStage() < this.ScaleIterations;
	}

	public void Grow(bool growByPickup = false)
	{
		if ((growByPickup && this.CanGrowByPickup()) || this.CanGrow())
		{
			SetGrowStage(this.GetGrowStage() + 1);
			if (!growByPickup)
			{
				this.velocity = new Vector2 (0.0f, this.GrowPopSpeed);
			}

			if (this.GetGrowStage() == this.ScaleIterations)
			{
				this.AddMaxSizeVisual();
				this.MaxSizeSound = SoundFX.Instance.OnHeroReachedMaxSize(this);
			}
			else
			{
				SoundFX.Instance.OnHeroGrowComplete(this);
			}
		}
	}

	public void Reset()
	{
		this.Die();
		this.Respawn();
		this.transform.localPosition = Vector3.zero;
		this.RespawnTimeCalculated = this.RespawnTime;
		this.NumDeaths = 0;

        DoubleJumpScore = 2;
        LaserScore = 5;
    }

	void Respawn()
	{
		this.transform.position = new Vector3(0,0,0);

		this.velocity = new Vector2(0.0f, 1.0f) * this.SpawnMagnitude;

		SoundFX.Instance.OnHeroRespawn(this);
		this.RespawnTimeLeft = -1.0f;
    }

	void SetGrowStage(int growStage)
	{
		//hardcoded growStage to #
		this.scale = (this.ScaleAdjustment * 3 * this.StartScale) + this.StartScale;
		Rigidbody2D rb = GetComponent<Rigidbody2D>();
		rb.mass = 500000/*(this.StartScale / this.scale)*/;
	}

	public int GetGrowStage()
	{
		return (int)((this.scale - this.StartScale) / (ScaleAdjustment * this.StartScale));
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

	void OnTriggerEnter2D(Collider2D other)
	{
		//this.gameObject.layer = LayerMask.NameToLayer ("Hero Platforms");
        if (other.tag == "Wall"){
            nexttowall = true;
        } else if (other.tag == "Platform" && rb.velocity.y < 0){
            gameObject.layer = LayerMask.NameToLayer("Hero Platform");
            onplatform = true;
        }
	}

	void OnTriggerExit2D(Collider2D other)
	{
		//this.gameObject.layer = LayerMask.NameToLayer ("Default");
        if (other.tag == "Wall"){
            nexttowall = false;
        } else if (other.tag == "Platform"){
            onplatform = false;
        }
	}
}
