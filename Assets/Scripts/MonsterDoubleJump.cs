using UnityEngine;

namespace Assets.Scripts
{
    public class MonsterDoubleJump : Powerup
    {
        protected Hero Hero;

        public override void Start()
        {
            base.Start();

            Hero = GetComponent<Hero>();
            if (!Hero)
            {
                Debug.LogError("Monster double jump must be given to a Hero component.");
                Destroy(this);
            }

            if (Hero) Hero.EnableDoubleJump = true;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (Hero) Hero.EnableDoubleJump = false;
        }
    }
}
