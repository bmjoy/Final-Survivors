using UnityEngine;
using Cinemachine;
using Final_Survivors.Audio;
using Final_Survivors.UI.GameOverMenu;

namespace Final_Survivors.Player
{
    public class PlayerManager : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private float health;
        [SerializeField] private float healthMaxValue = 100f; // default value

        [Header("Shield")]
        [SerializeField] private float shieldValue;
        [SerializeField] private float shieldMaxValue = 100f; // default value

        [Header("Movement Force")]
        [SerializeField] private float force;
        [SerializeField] private float dashForce;

        [Header("UI reference")]
        [SerializeField] private GameObject menuManager;

        // Audio
        AudioSource audioSource;
        SoundBank soundBank;

        // Camera
        private CinemachineVirtualCamera vCam;

        public Rigidbody rb { get; private set; }
        private Vector3 cursorPosition;

        bool playerMovement;

        float timer = 0;
        float timerDuration = 0.5f;

        public int cells = 0;

        // Getters and Setters
        public float Health
        {
            get { return health; }
            set { health = value; }
        }

        public float HealthMaxValue
        {
            get { return healthMaxValue; }
            set { healthMaxValue = value; }
        }

        public float ShieldValue
        {
            get { return shieldValue; }
            set { shieldValue = value; }
        }

        public float ShieldMaxValue
        {
            get { return shieldMaxValue; }
            set { shieldMaxValue = value; }
        }

        void Awake()
        {
            health = healthMaxValue;
            rb = GetComponent<Rigidbody>();
            audioSource = GetComponent<AudioSource>();
            soundBank = FindObjectOfType<SoundBank>();

            vCam = GameObject.FindGameObjectWithTag("Vcam").GetComponent<CinemachineVirtualCamera>();
            vCam.Follow = transform;
            vCam.LookAt = transform;
        }

        public void PlaySoundWalk()
        {
            SoundManager.PlaySound(soundBank.walk, audioSource, 0.33f);
        }

        public void PlaySoundDash()
        {
            SoundManager.PlaySound(soundBank.dash, audioSource, 0.33f);
        }

        public void PlaySoundCac()
        {
            SoundManager.PlaySound(soundBank.laserSword, audioSource, 0.33f);
        }

        public void PlaySoundRangedAttack()
        {
            SoundManager.PlaySound(soundBank.submachineGun, audioSource, 0.33f);
        }

        public void PlaySoundTimeWarp()
        {
            SoundManager.PlaySound(soundBank.timeWarp, audioSource, 0.33f);
        }

        public void Move(Vector2 input)
        {
            RotateToMouse();
            Vector3 direction = new Vector3(input.x, 0, input.y);
            rb.AddForce(direction * force, ForceMode.Force);
        }

        public void RotateToMouse()
        {
            cursorPosition = Camera.main.ScreenToWorldPoint(new Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, Vector3.Distance(Camera.main.transform.position, transform.position)));
            cursorPosition = new Vector3(cursorPosition.x, transform.position.y, cursorPosition.z);

            transform.forward = (cursorPosition - transform.position).normalized;
        }

        public void Dash()
        {
            if (!playerMovement)
            {
                rb.AddForce(transform.forward * dashForce, ForceMode.Force);
            }
            else
            {
                rb.AddForce(rb.velocity.normalized * dashForce, ForceMode.Force);
            }
        }

        public void MeleeAttack()
        {
            if (timer >= timerDuration)
            {
                Debug.Log("Melee attack");
                SoundManager.PlaySound(soundBank.laserSword, audioSource, 0.33f);
            }
            else
            {
                timer = 0;
            }
        }

        public void RangedAttack()
        {
            if (timer >= timerDuration)
            {
                Debug.Log("Pew! Pew!");
                SoundManager.PlaySound(soundBank.submachineGun, audioSource, 0.33f);
            }
            else
            {
                timer = 0;
            }
        }

        public void TimeWarp()
        {
            timer += Time.deltaTime;
            if (timer >= timerDuration)
            {
                Debug.Log("Time Warp");
                SoundManager.PlaySound(soundBank.timeWarp, audioSource, 0.33f);
            }
            else
            {
                timer = 0;
            }
        }

        public void TakeDamage(float damage)
        {
            float damageLeft = damage;

            if (shieldValue >= damage)
            {
                shieldValue -= damage;
            }
            else
            {
                damageLeft = damage - shieldValue;
                shieldValue = 0;

                if (health > damageLeft)
                {
                    health -= damageLeft;
                }
                else
                {
                    health = 0;
                    menuManager.GetComponent<GameOverMenu>().GameOver();
                }
            }
        }
    }
}
