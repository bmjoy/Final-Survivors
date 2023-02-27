using Final_Survivors.Audio;
using UnityEngine;

namespace Final_Survivors.Weapons
{
    public class Shotgun : Weapon
    {
        [SerializeField] private int pelletAmmount;
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
            // Debug.Log("Shooting Shotgun");

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
            SoundManager.PlaySound(soundBank.shotgun, audioSource, 0.33f);
        }

        private void PlaySoundPumpAction()
        {
            SoundManager.PlaySound(soundBank.shotgunPump, audioSource, 0.33f);
        }

        private void SpawnProjectile()
        {
            Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(new Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, Vector3.Distance(Camera.main.transform.position, transform.position)));        
            Vector3 shootDir =  (cursorPosition - player.transform.position).normalized;

            for (int i = 0; i < pelletAmmount; i++)
            {
                // Debug.Log("Pellet# " + i);
                
                float refAngle = 45 / pelletAmmount * i - 45 / 2;
                MasterProjectile bullet = ObjectPooling.instance.TakeProjectilesFromPool(weaponType);
                bullet.transform.up = player.forward;
                bullet.transform.Rotate(new Vector3(0, refAngle,0), Space.World);
               
                bullet.AddForce(bullet.transform.up, transform.position);
            }
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
