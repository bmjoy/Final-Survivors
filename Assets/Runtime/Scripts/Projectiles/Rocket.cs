using Final_Survivors.Enemies;
using UnityEngine;

namespace Final_Survivors.Projectile
{
    public class Rocket : Bullet
    {
        [SerializeField] private float explosionRadius;

        private void Update()
        {
            if (lifeTime <= 0)
            {
                NotifyObservers(Core.Events.RETURN_BULLET);
            }
            lifeTimer -= Time.deltaTime;
        }

        private void OnEnable()
        {
            transform.forward = direction;
            transform.localScale = Vector3.one;
        }

        public override void AddForce(Vector3 direction, Vector3 position)
        {
            transform.position = position;
            transform.forward = -direction;
            rb.AddForce(projectileSpeed * direction, ForceMode.Impulse);
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (enabled)
            {
                if (!collision.gameObject.CompareTag("FL_Collider") && !collision.gameObject.CompareTag("Bullet") && !collision.gameObject.CompareTag("Player"))
                {
                    Collider[] hitColliders = Physics.OverlapSphere(collision.transform.position, explosionRadius, (1 << 6));
                    Debug.Log(hitColliders);
                    foreach(Collider col in hitColliders)
                    {
                        col.GetComponent<Enemy>().TakeDamage(damageAmount);
                    }

                    transform.localScale = Vector3.one * 5;
                    NotifyObservers(Core.Events.RETURN_BULLET);
                }
                    
            }
        }
    }
}
