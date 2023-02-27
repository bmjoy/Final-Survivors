using UnityEngine;
using Final_Survivors.Core;
using Final_Survivors.Observer;
using Final_Survivors.Environment;
using System.Collections;
using UnityEngine.AI;

namespace Final_Survivors.Enemies
{
    public enum EnemyName {DRAGON, NOSE, RAZOR, ROBOT, SPIDER}
    public enum EnemyLevel {NORMAL, ELITE, BOSS}
    public enum EnemyType {RANGED, MELEE}

    public class Enemy : Subject
    {
        [Header("Health")]
        [SerializeField] private float health;
        [SerializeField] private float maxHealth;

        [Header("Attack")]
        [SerializeField] public float attackSpeed;
        [SerializeField] public float attackRange;

        [Header("Movement")]
        [SerializeField] NavMeshAgent agent;
        [SerializeField] public float moveSpeed;

        [Header("Death")]
        [SerializeField] private float deathTime;
        [SerializeField] private bool isDead;
        private float deathTimer;
        
        [Header("Collider")]
        [SerializeField] private Collider _collider;

        [Header("Alert / Sleep HUD")]
        [SerializeField] public MeshRenderer alertMesh;
        [SerializeField] public MeshRenderer[] sleepMeshs;

        public Animator animator { get; private set; }
        private EnemyType type;
        private EnemyLevel level;
        [SerializeField] private EnemyName enemyName;
        private bool isSleepMode;
        private float attackTimer;
        private int awakeTimer;
        private bool isAlertMode = false;
        private const int awake = 50;
        private const float spreadRadius = 12;
        public Transform player { get; private set; }
        public bool isAttacking { get; set; }
        private const float slowDamageCD = 0.4f;
        private float speed = 0f;

        // Properties (Getters and Setters)
        public float Health { get { return health; } }
        public float MaxHealth { get { return maxHealth; } }
        public bool IsDead { get { return isDead; } }
        public EnemyType Type { get { return type; } protected set { type = value; } }
        public EnemyLevel Level { get { return level; } protected set { level = value; } }
        public EnemyName Name { get { return enemyName; } protected set { enemyName = value; } }
        public bool IsSleepMode { get { return isSleepMode; } private set { isSleepMode = value; } }
        public float AttackTimer { get { return attackTimer; } protected set { attackTimer = value; } }
        public int AwakeTimer { get { return awakeTimer; } protected set { awakeTimer = value; } }
        public NavMeshAgent Agent { get { return agent; } }
        private SpawnManager spawnManager;
        private ScoreManager scoreManager;

        private void Start()
        {
            health = maxHealth;
            isSleepMode = false;
            isAttacking = false;
            animator = gameObject.GetComponent<Animator>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
            spawnManager = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();
            scoreManager = GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManager>();
        }

        private void Update()
        {
            if (isAlertMode && !EnvironmentState.GetIsDay())
            {
                AlertOthers();
            }
            else if (EnvironmentState.GetIsDay())
            {
                SetAlertMode(false);
                SetSleepIcons(false);
            }
            if (!isDead && !isAttacking)
            {
                transform.LookAt(player.position);
            }
        }

        private void OnEnable()
        {
            agent.enabled = true;
            speed = moveSpeed;
            health = maxHealth;
            isDead = false;
            _collider.enabled = true;
            attackTimer = attackSpeed;
            deathTimer = deathTime;
        }

        public void SetSleepMode(bool value)
        {
            isSleepMode = value;
        }

        public void SetAwakeTimer(int value)
        {
            awakeTimer = value;
        }

        public void TakeDamage(float damage)
        {
            if (health > damage)
            {
                animator.Play("Take Damage", 0);
                health -= damage;

                if (!EnvironmentState.GetIsDay())
                {
                    SetAlertMode(true);
                }

                moveSpeed = 0;
                StartCoroutine(nameof(DamageSlowCooldown));
            }
            else
            {
                health = 0;
                isDead = true;
                _collider.enabled = false;
                scoreManager.AddScore(Level);
                spawnManager.CheckTriggersBiggerEnemies();

                /*if (Level == EnemyLevel.ELITE) // Relaunch Spawn when Elite dies
                {
                    spawnManager.PlaySpawn();
                }*/
                
                InvokeRepeating(nameof(Dying), 0, Time.fixedDeltaTime);

                if (!EnvironmentState.GetIsDay())
                {
                    SetAlertMode(false);
                    SetSleepIcons(false);
                }
            }

            if (!EnvironmentState.GetIsDay())
            {
                SetSleepIcons(false);
                SetSleepMode(false);
            }

            NotifyObservers(Events.TAKE_DAMAGE);
        }

        public IEnumerator DamageSlowCooldown()
        {
            yield return new WaitForSeconds(slowDamageCD);

            moveSpeed = speed;
        }

        public void AttackCooldown()
        {
            attackTimer = 0;
            InvokeRepeating(nameof(ResetAttackTimer), 0, Time.fixedDeltaTime);
        }

        private void ResetAttackTimer()
        {
            if (attackTimer >= attackSpeed)
            {
                CancelInvoke(nameof(ResetAttackTimer));
            }

            attackTimer += Time.fixedDeltaTime;
        }

        private void Dying()
        {
            if (deathTimer <= 0)
            {
                CancelInvoke(nameof(Dying));
                NotifyObservers(Events.RESET);
                ObjectPooling.instance.ReturnObjToPool(this, enemyName);
            }

            deathTimer -= Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("FL_Collider"))
            {
                NotifyObservers(Events.INSIDE_FLASHLIGHT);

                if (isSleepMode)
                {
                    SetAwakeTimer(0);
                    SetAlertMode(false);
                    SetSleepIcons(true);
                }
                else
                {
                    SetAlertMode(true);
                    SetSleepIcons(false);
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("FL_Collider"))
            {
                if (isSleepMode)
                {
                    SetAwakeTimer(++awakeTimer);

                    if (awakeTimer >= awake)
                    {
                        SetAlertMode(true);
                        SetSleepIcons(false);
                    }
                }
                else
                {
                    SetAlertMode(true);
                    SetSleepIcons(false);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("FL_Collider"))
            {
                SetAwakeTimer(0);
                SetAlertMode(false);
                SetSleepIcons(false);
                NotifyObservers(Events.OUTSIDE_FLASHLIGHT);
            }
        }

        public void AlertOthers()
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, spreadRadius, (1 << 6));

            foreach (var hit in cols)
            {
                if (hit.CompareTag("Enemy"))
                {
                    Enemy enemy = hit.gameObject.GetComponent<Enemy>();

                    if (enemy.isSleepMode)
                    {
                        enemy.SetSleepMode(false);
                        enemy.SetAlertMode(true);
                    }
                }
            }
        }

        private void SetSleepIcons(bool value) 
        {
            foreach (MeshRenderer meshRenderer in sleepMeshs)
            {
                meshRenderer.enabled = value;
            }
        }

        public void SetAlertMode(bool value)
        {
            alertMesh.enabled = value;
            isAlertMode = value;
        }

        public void ClearUI()
        {
            NotifyObservers(Events.TIME_WARP_DAY_TO_NIGHT);
        }

        public void StopAttack()
        {
            isAttacking = false;
        }
    }
}
