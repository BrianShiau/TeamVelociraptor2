using UnityEngine;

namespace Assets.Scripts
{
    public abstract class Powerup : MonoBehaviour
    {
        public float Duration;

        public virtual void Reset()
        {
            Duration = 10f;
        }

        public virtual void Awake()
        {
            if (Duration == default(float))
            {
                Duration = 10f;
            }
        }

        public virtual void Start()
        {

        }

        public virtual void Update()
        {
            if ((Duration -= Time.deltaTime) < 0f)
            {
                Destroy(this);
            }
        }

        public virtual void OnDestroy()
        {

        }
    }

    public static class PowerupExtensions
    {
        public static TPowerup AddPowerup<TPowerup>(this GameObject gameObject, float duration = 999f) 
            where TPowerup : Powerup
        {
            var powerup = gameObject.AddComponent<TPowerup>();
            powerup.Duration = duration;
            return powerup;
        }

        public static TPowerup AddPowerup<TPowerup>(this Component component, float duration = 999f) 
            where TPowerup : Powerup
        {
            return AddPowerup<TPowerup>(component.gameObject, duration);
        }
    }
}
