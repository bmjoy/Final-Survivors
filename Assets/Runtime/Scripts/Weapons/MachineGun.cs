using Final_Survivors.Audio;
using UnityEngine;

namespace Final_Survivors.Weapons
{
    public class MachineGun : Weapon
    {
        private Transform player;

        private void Awake()
        {
            IncrementAmmo(ammoCapacity);

            if(player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player").transform;
            }
        }

        public override void Shoot()
        {
            // Debug.Log("Shooting Machine Gun");

            if (isCycled && ammoCount > 0)
            {
                // Debug.Log("Ammo left: " + ammoCount);

                DecreaseAmmo(1);
                isCycled = false;
                cycleTimer = cycleTime;
                PlaySoundShoot();
                SpawnProjectile();
                InvokeRepeating("Cooldown", 0, Time.fixedDeltaTime);
            }
        }

        private void PlaySoundShoot()
        {
            // If you read this : go to line 247-252 in PlayerController.cs
            // The NullReference that send you here come from weapon "SetActive" failure and is not related to audio in any way.
            SoundManager.PlaySound(soundBank.submachineGun, audioSource, 0.33f);
        }

        private void SpawnProjectile()
        {
            MasterProjectile bullet = ObjectPooling.instance.TakeProjectilesFromPool(weaponType);
            bullet.AddForce(player.forward, transform.position);
        }

        private void Cooldown()
        {
            cycleTimer -= Time.fixedDeltaTime;

            if (cycleTimer < 0)
            {
                isCycled = true;
                CancelInvoke("Cooldown");
            }
        }
    }
}
