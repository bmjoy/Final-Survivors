using UnityEngine;
using Final_Survivors.Weapons;
using Final_Survivors.Player;

namespace Final_Survivors
{
    public class BallisticCrate : Crate
    {
        int i = 0; // Debug variable to check how many times the weapon crate is picked up (should always be 1)

        public override void SetPowerUp()
        {
            // Prevents the player from picking up a weapon more than once at the same time.
            i += 1;
            //Debug.Log("Weapons picked up " + i);

            if(i == 2)
            {
                //Debug.Log("ATTENTION : You have already picked up a weapon !");
                return;
            }

            PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            int randomPick = Random.Range(0, 4);

            Debug.Log(randomPick);

            switch (randomPick)
            {
                case 0:
                    playerController.RandomPickWeapon(WeaponType.MACHINEGUN);
                    break;
                case 1:
                    playerController.RandomPickWeapon(WeaponType.SHOTGUN);
                    break;
                case 2: playerController.RandomPickWeapon(WeaponType.SNIPER);
                    break;
                case 3: playerController.RandomPickWeapon(WeaponType.ROCKET_LAUNCHER);
                    break;
            }

            DestroyCrate();
        }
    }
}
