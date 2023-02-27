using Final_Survivors.Enemies;
using UnityEngine;

namespace Final_Survivors.Projectile
{
    public class ShotgunAmmo : Bullet
    {
        private void Update()
        {
            if (lifeTime <= 0)
            {
                NotifyObservers(Core.Events.RETURN_BULLET);
            }
            lifeTimer -= Time.deltaTime;
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (enabled)
            {
                if (collision.gameObject.CompareTag("Enemy"))
                {
                    collision.gameObject.GetComponent<Enemy>().TakeDamage(damageAmount);
                }

                if (!collision.gameObject.CompareTag("FL_Collider") && !collision.gameObject.CompareTag("Bullet") && !collision.gameObject.CompareTag("Player"))
                {
                    NotifyObservers(Core.Events.RETURN_BULLET);
                }
            }
        }
    }
}
