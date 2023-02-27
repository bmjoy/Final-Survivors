using Final_Survivors.Enemies;
using Final_Survivors.Weapons;
using UnityEngine;

namespace Final_Survivors
{
    public class Sword : Weapon
    {
        [SerializeField] private float damage;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                other.GetComponent<Enemy>().TakeDamage(damage);
            }
        }
    }
}
