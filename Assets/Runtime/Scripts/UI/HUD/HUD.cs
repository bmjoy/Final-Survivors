using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Final_Survivors.Player;
using Final_Survivors.Core;
using Final_Survivors.Observer;
using Final_Survivors.Weapons;

namespace Final_Survivors.UI.HUD
{
    public class HUD : MonoBehaviour, IObserver
    {
        [Header("PLAYER")]
        private PlayerManager playerManager;

        [Header("WEAPON")]
        [SerializeField] private Subject[] _subjects;
        [SerializeField] private Weapon[] weapons;
        private Weapon selectedWeapon;

        [Header("HEALTH")]
        private Slider healthBar; 
        private TextMeshProUGUI currentHealth;
        private TextMeshProUGUI maxHealth;

        [Header("SHIELD")]
        private Slider shieldBar;
        private TextMeshProUGUI currentShield;
        private TextMeshProUGUI maxShield;

        [Header("AMMOS")]
        private TextMeshProUGUI ammos;

        [Header("SCORE")]
        private Subject _scoreSubject;
        private ScoreManager scoreScript;
        private TextMeshProUGUI score;

        private void Awake()
        {
            playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>(); // Find the player
            _scoreSubject = GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManager>();
            scoreScript = GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManager>();

            // HEALTH
            healthBar = GameObject.Find("HUD/Health").GetComponent<Slider>();
            currentHealth = GameObject.Find("HUD/Health/Ressources/Current (Text)").GetComponent<TextMeshProUGUI>();
            maxHealth = GameObject.Find("HUD/Health/Ressources/Max (Text)").GetComponent<TextMeshProUGUI>();

            // SHIELD
            shieldBar = GameObject.Find("HUD/Shield").GetComponent<Slider>();
            currentShield = GameObject.Find("HUD/Shield/Ressources/Current (Text)").GetComponent<TextMeshProUGUI>();
            maxShield = GameObject.Find("HUD/Shield/Ressources/Max (Text)").GetComponent<TextMeshProUGUI>();

            // AMMOS
            ammos = GameObject.Find("HUD/Ammos (Text)").GetComponent<TextMeshProUGUI>();
            ammos.outlineWidth = 0.1f; // Set the outline width
            ammos.outlineColor = Color.black; // Set the outline color

            // SCORE
            score = GameObject.Find("HUD/Score (Text)").GetComponent<TextMeshProUGUI>();
            score.outlineWidth = 0.1f; // Set the outline width
            score.outlineColor = Color.black; // Set the outline color
        }

        private void OnEnable()
        {
            foreach (Subject subject in _subjects)
            {
                subject.AddObserver(this);
            }

            _scoreSubject.AddObserver(this);
        }

        private void OnDisable()
        {
            foreach (Subject subject in _subjects)
            {
                subject.RemoveObserver(this);
            }

            _scoreSubject.RemoveObserver(this);
        }

        public void OnNotify(Events action)
        {
            if (action == Events.AMMO)
            {
                int ammoCount = (int)selectedWeapon.GetAmmoCount();
                ammos.text = ammoCount.ToString();
            }
            
            if (action == Events.PISTOL)
            {
                SetSelectedWeaponWithName("Pistol");
                ammos.text = "\u221E"; // Infinity symbol
            }
            else if (action == Events.SWORD)
            {
                SetSelectedWeaponWithName("Sword");
                ammos.text = "\u221E"; // Infinity symbol
            }
            else if (action == Events.MACHINE_GUN)
            {
                SetSelectedWeaponWithName("MachineGun");
                int ammoCount = (int)selectedWeapon.GetAmmoCount();
                ammos.text = ammoCount.ToString();
            }
            else if (action == Events.SHOTGUN)
            {
                SetSelectedWeaponWithName("Shotgun");
                int ammoCount = (int)selectedWeapon.GetAmmoCount();
                ammos.text = ammoCount.ToString();
            }
            else if (action == Events.SNIPER)
            {
                SetSelectedWeaponWithName("SniperRifle");
                int ammoCount = (int)selectedWeapon.GetAmmoCount();
                ammos.text = ammoCount.ToString();
            }
            else if (action == Events.ROCKET_LAUNCHER)
            {
                SetSelectedWeaponWithName("RocketLauncher");
                int ammoCount = (int)selectedWeapon.GetAmmoCount();
                ammos.text = ammoCount.ToString();
            }

            if (action == Events.SCORE)
            {
                score.text = "SCORE : " + scoreScript.GetScore().ToString();
            }
        }

        private void SetSelectedWeaponWithName(string name)
        {
            foreach(Weapon weapon in weapons)
            {
                if (weapon.gameObject.name == name)
                {
                    selectedWeapon = weapon;
                    break;
                }
            }
        }

        private void Update()
        {
            // Update Health
            healthBar.value = (int)playerManager.Health;

            int healthValue = (int)playerManager.Health;
            currentHealth.text = healthValue.ToString();

            int healthMaxValue = (int)playerManager.HealthMaxValue;
            maxHealth.text = healthMaxValue.ToString();

            // Update Shield
            shieldBar.value = (int)playerManager.ShieldValue;

            int shieldValue = (int)playerManager.ShieldValue;
            currentShield.text = shieldValue.ToString();

            int shieldMaxValue = (int)playerManager.ShieldMaxValue;
            maxShield.text = shieldMaxValue.ToString();
        }
    }
}