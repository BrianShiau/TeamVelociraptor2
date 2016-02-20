using UnityEngine;
using System.Collections;

public class BlockScript : MonoBehaviour {

    [SerializeField]
    Sprite sprite_fine, sprite_damaged;

    public bool damaged;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void DamageBuilding() {
        GetComponent<SpriteRenderer>().sprite = sprite_damaged;
        damaged = true;
        transform.parent.gameObject.GetComponent<BuildingScript>().CheckBuilding();
    }

	void OnCollisionEnter2D(Collision2D collision)
	{
		Punch punch = collision.gameObject.GetComponent<Punch>();
		if (punch != null)
		{
            DamageBuilding();
			SoundFX.Instance.OnHeroHit(punch.OwnerHero);
		}

		Destroy(this.gameObject);
	}
}
