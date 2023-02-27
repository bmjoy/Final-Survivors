using Final_Survivors.Audio;
using Final_Survivors.Observer;
using Final_Survivors.Core;
using UnityEngine;

namespace Final_Survivors.Weapons
{
    public enum WeaponType {PISTOL, MACHINEGUN, SHOTGUN, SNIPER, ROCKET_LAUNCHER, EXPLOSION, SWORD};

    public class Weapon : Subject, IObserver
    {
        [SerializeField] private Subject _playerSubject;
        [SerializeField] protected WeaponType weaponType;
        [SerializeField] protected float cycleTime;
        protected float cycleTimer = 0f;
        [SerializeField] protected float ammoCapacity;
        protected float ammoCount = 0f;
        protected SoundBank soundBank;
        protected AudioSource audioSource;
        protected bool isCycled = true;

        private void Start()
        {
            soundBank = FindObjectOfType<SoundBank>();
            audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            if (weaponType != WeaponType.SWORD && weaponType != WeaponType.PISTOL)
            {
                _playerSubject.AddObserver(this);
            }
        }

        private void OnDisable()
        {
            if (weaponType != WeaponType.SWORD && weaponType != WeaponType.PISTOL)
            {
                _playerSubject.RemoveObserver(this);
            }
        }

        public void OnNotify(Events action)
        {
            if (action == Events.RELOAD)
            {
                if (weaponType != WeaponType.SWORD && weaponType != WeaponType.PISTOL)
                {
                    RefillAmmo();
                }
            }
        }

        public virtual void Shoot()
        {
        }

        protected void DecreaseAmmo(float value)
        {
            if (ammoCount >= value)
            {
                ammoCount -= value;
            }
            else
            {
                ammoCount = 0f;
            }

            NotifyObservers(Events.AMMO);
        }

        protected void IncrementAmmo(float value)
        {
            ammoCount = value;
            NotifyObservers(Events.AMMO);
        }

        public float GetAmmoCount()
        {
            return ammoCount;
        }

        private void RefillAmmo()
        {
            IncrementAmmo(ammoCapacity);
        }
    }
}
