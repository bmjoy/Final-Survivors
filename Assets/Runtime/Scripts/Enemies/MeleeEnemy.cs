using Final_Survivors.Player;
using UnityEngine;

namespace Final_Survivors.Enemies
{
    public class MeleeEnemy : Enemy
    {
        [SerializeField] private float meleeDamage;

        public float MeleeDamage { get { return meleeDamage; } }
        
        private void Awake()
        {
            Type = EnemyType.MELEE;
            Level = EnemyLevel.NORMAL;
        }

        public void DealDamage()
        {          
            if (Vector3.Distance(player.position, transform.position) <= attackRange)
            {
                player.GetComponent<PlayerManager>().TakeDamage(MeleeDamage);            
            }

            AttackCooldown();
        }
    }
}
