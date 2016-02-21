using UnityEngine;

namespace Assets.Scripts
{
    public class MonsterLaser : Powerup
    {
        protected Hero Hero;

        public AudioClip Clip;
        public GameObject LaserObject;

        public float Range;
        public float Width;

        public override void Reset()
        {
            base.Reset();
            Range = 100f;
            Width = .5f;
        }

        public override void Start()
        {
            base.Start();

            Duration = 9999f;
            if (Range == default(float)) Range = 100f;
            if (Width == default(float)) Width = .5f;

            Hero = GetComponent<Hero>();

            if (!Hero)
                Destroy(this);
            else
            {
                Hero.CanPunch = false;
            }
        }

        public override void Update()
        {
            base.Update();

            Hero.CanPunch = false;

            if (Hero.HeroController.Shooting)
            {
                Fire();
            }
        }

        public void NotifyDamage(GameObject target)
        {
            var block = target.GetComponent<BlockScript>();
            if (block)
            {
                block.DamageBuilding();
            }
            
            // TODO damage player here
        }

        public void Fire()
        {
            RaycastHit2D[] results = new RaycastHit2D[64];

            Physics2D.queriesHitTriggers = false;

            var amount = Physics2D.BoxCastNonAlloc(transform.position, new Vector2(1f, Width), 0f,
                Hero.FacingRight ? Vector2.right : Vector2.left, results, Range);

            for(var i = 0; i < amount; ++i)
            {
                var result = results[i];
                NotifyDamage(results[i].collider.gameObject);
            }

            Physics2D.queriesHitTriggers = true;

            if(Clip) AudioSource.PlayClipAtPoint(Clip, transform.position);
            if (LaserObject)
            {
                var newLaserObject = Instantiate(LaserObject);
                newLaserObject.transform.position = transform.position;
                Destroy(newLaserObject, 0.25f);
            }

            Destroy(this);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Hero.CanPunch = true;
        }
    }
}
