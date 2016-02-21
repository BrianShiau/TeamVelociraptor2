using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
	public float Lifetime;
	private float LifetimeRemaining;
	public Hero OwnerHero;
	public Vector2 Velocity;

	void Start ()
	{
		this.LifetimeRemaining = this.Lifetime;
	}

	void Update ()
	{
		this.LifetimeRemaining -= Time.deltaTime;

		if (this.LifetimeRemaining < 0.0f)
		{
			Destroy (this.gameObject);
		}
	}

	void LateUpdate ()
	{
		this.transform.Translate (this.Velocity * Time.deltaTime);
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		Hero hero = collision.gameObject.GetComponent<Hero>();
		if (this.OwnerHero && hero == this.OwnerHero)
		{
			return;
		}

		Player player = collision.gameObject.GetComponent<Player>();
		if (player)
			return;
		

		if (hero != null)
		{
			hero.Hit();
			SoundFX.Instance.OnHeroHit(hero);
		}

		Destroy(this.gameObject);
    }
}
