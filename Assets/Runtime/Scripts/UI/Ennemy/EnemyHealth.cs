using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Final_Survivors.Observer;
using Final_Survivors.Core;
using Final_Survivors.Enemies;
using Final_Survivors.Environment;

namespace Final_Survivors
{
    public class EnemyHealth : MonoBehaviour, IObserver
    {
        [Header("Health Bar Settings")]
        [SerializeField] private bool displayHealthBarAfterDeath = true;
        [SerializeField] private float hideHealthBarAfterXSeconds = 0f;
        
        [Header("Health Bar References")]
        [SerializeField] GameObject healthBar;
        [SerializeField] Slider healthSlider;
        [SerializeField] Image fillArea;

        //[Header("Enemy")]
        private Enemy enemy;

        //[Header("Subject")]
        Subject _enemySubject;

        // Is the enemy in the flashlight ?
        private bool isEnlighted = false;

        private void Start()
        {
            enemy = GetComponentInParent<Enemy>(); 
        }

        private void Update()
        {
            healthSlider.transform.forward = Vector3.up;
            HealthBarManagement();
        }

        private void OnEnable()
        {
            _enemySubject = GetComponentInParent<Enemy>();
            _enemySubject.AddObserver(this);
        }

        private void OnDisable()
        {
            _enemySubject.RemoveObserver(this);
        }

        public void OnNotify(Events action)
        {
            if (action == Events.TAKE_DAMAGE)
            {
                DecreaseHealth();
            }

            if (action == Events.TIME_WARP_DAY_TO_NIGHT)
            {
                healthBar.SetActive(false);
                isEnlighted = false;
            }

            if (action == Events.INSIDE_FLASHLIGHT)
            {
                isEnlighted = true;
            }
            else if (action == Events.OUTSIDE_FLASHLIGHT)
            {
                healthBar.SetActive(false);
                isEnlighted = false;
            }
            
            if (action == Events.RESET)
            {
                healthSlider.value = 1;
                fillArea.color = Color.green;
                healthBar.SetActive(false);
            }
        }

        private void HealthBarManagement()
        {
            if (enemy.Health < enemy.MaxHealth) // healthBar is shown only when enemy has taken damages
            {
                if (enemy.Health <= 0)
                {
                    healthBar.SetActive(displayHealthBarAfterDeath);
                }
                else
                {
                    if (EnvironmentState.GetIsDay())
                    {
                        healthBar.SetActive(true);
                    }
                    else
                    {
                        if (isEnlighted)
                        {
                            healthBar.SetActive(true);
                        }
                        else
                        {
                            healthBar.SetActive(false);
                        }
                    }
                }

                if (hideHealthBarAfterXSeconds != 0f)
                {
                    StartCoroutine(nameof(StartHideHealthTimer));
                }
            }
        }

        private IEnumerator StartHideHealthTimer()
        {
            yield return new WaitForSecondsRealtime(hideHealthBarAfterXSeconds);

            healthBar.SetActive(false);
        }

        private void DecreaseHealth()
        {
            healthSlider.value = enemy.Health / enemy.MaxHealth;
            fillArea.color = Color.Lerp(Color.red, Color.green, healthSlider.value);

            if (healthSlider.value <= 0)
            {
                fillArea.color = Color.black;
            }
        }
    }
    
}
