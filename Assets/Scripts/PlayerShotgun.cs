using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerShotgun : Powerup
    {
        protected PlayerArm PlayerArm;

        public override void Start()
        {
            base.Start();
            PlayerArm = GetComponentInChildren<PlayerArm>();

            if (!PlayerArm)
            {
                Debug.LogError("Could not find PlayerArm on which to enable machine gun.");
                Destroy(this);
            }
            else
            {
                PlayerArm.shotgun = true;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (PlayerArm) PlayerArm.shotgun = false;
        }
    }
}
