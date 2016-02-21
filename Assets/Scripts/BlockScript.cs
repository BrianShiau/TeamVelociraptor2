using UnityEngine;
using System.Collections;

public class BlockScript : MonoBehaviour {

    [SerializeField]
    Sprite sprite_fine, sprite_damaged;

    int health = 2;

    public bool damaged = false;

	// Use this for initialization
	void Start () {
        tag = "Wall";
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void DamageBuilding() {
        if (--health <= 0){
            GetComponent<SpriteRenderer>().sprite = sprite_damaged;
            GetComponent<BoxCollider2D>().enabled = false;
            damaged = true;
        }
    }

	void OnCollisionEnter2D(Collision2D collision)
	{
		Punch punch = collision.gameObject.GetComponent<Punch>();
		if (punch != null)
		{
            DamageBuilding();
			SoundFX.Instance.OnHeroHit(punch.OwnerHero);
		}
	}
}
