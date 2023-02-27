using Final_Survivors.Audio;
using UnityEngine;

namespace Final_Survivors.Weapons
{
    public class Pistol : Weapon
    {
        private Transform player;

        private void Awake()
        {
            if(player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player").transform;
            }
        }

        public override void Shoot()
        {
            // Debug.Log("Shooting Pistol");

            if (isCycled)
            {
                isCycled = false;
                cycleTimer = cycleTime;
                PlaySoundShoot();
                SpawnProjectile();
                InvokeRepeating("Cooldown",0,Time.fixedDeltaTime);
            }
        }

        private void PlaySoundShoot()
        {
            SoundManager.PlaySound(soundBank.handGun, audioSource, 0.33f);
        }

        private void SpawnProjectile()
        {
            MasterProjectile bullet = ObjectPooling.instance.TakeProjectilesFromPool(weaponType);
            bullet.AddForce(player.forward, transform.position);
        }

        private void Cooldown()
        {
            cycleTimer -= Time.fixedDeltaTime;

            if(cycleTimer < 0)
            {
                isCycled = true;
                CancelInvoke("Cooldown");
            }
        }
    }
}
