using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerMachineGun : Powerup
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
                PlayerArm.machinegun = true;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (PlayerArm) PlayerArm.machinegun = false;
        }
    }
}
