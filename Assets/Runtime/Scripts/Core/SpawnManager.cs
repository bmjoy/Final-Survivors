using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Final_Survivors.Enemies;
using Final_Survivors.Observer;
using Final_Survivors.Core;
using Final_Survivors.Player;

namespace Final_Survivors
{
    public class SpawnManager : MonoBehaviour, IObserver
    {
        private enum SpawnType { NONE, DRAGONBOSS, NOSEBOSS, RAZORBOSS, ROBOTBOSS, SPIDERBOSS, DRAGONELITE, NOSEELITE, RAZORELITE, ROBOTELITE, SPIDERELITE }
        [Header("Spawning distance from player")]
        [SerializeField] private float maxSpawnDistance = 50f;

        [Header("Spawn Rates for enemies 0 - NOSE, 1 - RAZOR, 2 - SPIDER, 3 - ROBOT, 4 - DRAGON")]
        [SerializeField] private float[] enemiesMinSpawnRate;
        [SerializeField] private float[] enemiesMaxSpawnRate;
        [SerializeField] private int paralellSpawn = 5;
        public int nbNormalDeadBeforeElite = 5;
        public int nbEliteDeadBeforeBoss = 3;
        public int nbBossDeadBeforeNextStage = 1;

        [Header("Choose enemy to spawn (TEST PURPOSE ONLY)")]
        [SerializeField] SpawnType spawnEnemy = 0;
        [SerializeField] bool spawn = false;
        [SerializeField] private GameObject[] enemyElitePrefabs = new GameObject[5];
        [SerializeField] private GameObject[] enemyBossPrefabs = new GameObject[5];
        private Transform enemyParent;

        private GameObject player;
        private Subject _playerSubject;
        private Camera cam;
        private ScoreManager scoreManager;

        private void Start()
        {
            cam = Camera.main;
            player = GameObject.FindGameObjectWithTag("Player");
            scoreManager = GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManager>();
            enemyParent = GameObject.FindGameObjectWithTag("Enemy").transform;

            StartCoroutine(nameof(SpawnEnemies));
        }

        private void OnEnable()
        {
            _playerSubject = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            _playerSubject.AddObserver(this);
        }

        private void OnDisable()
        {
            _playerSubject.RemoveObserver(this);
        }

        public void OnNotify(Events action)
        {
            if (action == Events.TIME_WARP_DAY_TO_NIGHT)
            {
                PauseSpawn();
            }

            if (action == Events.TIME_WARP_NIGHT_TO_DAY)
            {
                PlaySpawn();
            }
        }

        private void SetParent(GameObject child, Transform parent)
        {
            child.transform.SetParent(parent);
        }

        private void Update()
        {
            // ***** TEST PURPOSE ONLY *****/
            SpeedUpSpawn(); // Make the enemies appear more quickly over time
            if (spawnEnemy != 0 && spawn)
            {
                switch(spawnEnemy)
                {
                    case SpawnType.DRAGONBOSS:
                        SpawnBoss(0);
                        break;
                    case SpawnType.DRAGONELITE:
                        SpawnElite(0);
                        break;
                    case SpawnType.NOSEBOSS:
                        SpawnBoss(1);
                        break;
                    case SpawnType.NOSEELITE:
                        SpawnElite(1);
                        break;
                    case SpawnType.RAZORBOSS:
                        SpawnBoss(2);
                        break;
                    case SpawnType.RAZORELITE:
                        SpawnElite(2);
                        break;
                    case SpawnType.ROBOTBOSS:
                        SpawnBoss(3);
                        break;
                    case SpawnType.ROBOTELITE:
                        SpawnElite(3);
                        break;
                    case SpawnType.SPIDERBOSS:
                        SpawnBoss(4);
                        break;
                    case SpawnType.SPIDERELITE:
                        SpawnElite(4);
                        break;
                }
                spawn = false;
            }
            // ***** TEST PURPOSE ONLY *****/
        }

        private void SpeedUpSpawn()
        {
            for (int i = 0; i < enemiesMinSpawnRate.Length; ++i)
            {
                if (enemiesMinSpawnRate[i] > 0.5)
                {
                    enemiesMinSpawnRate[i] -= Time.fixedDeltaTime / 500;
                }
            }
        }

        public void CheckTriggersBiggerEnemies()
        {
            if (scoreManager.normalCounter >= nbNormalDeadBeforeElite)
            {
                Debug.Log("An ELITE has spawn !");
                SpawnElite();
                scoreManager.normalCounter = 0;
            }

            if (scoreManager.eliteCounter >= nbEliteDeadBeforeBoss)
            {
                Debug.Log("A BOSS has spawn !");
                SpawnBoss();
                scoreManager.eliteCounter = 0;
            }

            if (scoreManager.bossCounter >= nbBossDeadBeforeNextStage)
            {
                Debug.Log("YOU WON ! NEXT STAGE");
                // TODO IMPLEMENT NEXT STAGE FUNCTION
                scoreManager.bossCounter = 0;
            }
        }

        public void PauseSpawn()
        {
            StopCoroutine(nameof(SpawnEnemies));
        }

        public void PlaySpawn()
        {
            StartCoroutine(nameof(SpawnEnemies));
        }

        private void SpawnElite(int index = -1)
        {
            //PauseSpawn();

            Enemy enemy;

            if (index != -1)
            {
                GameObject elite = Instantiate(enemyElitePrefabs[index]);
                SetParent(elite, enemyParent);
                elite.SetActive(true);
                enemy = elite.GetComponent<Enemy>();
            }
            else
            {
                enemy = GetRandomEliteEnemy();
            }

            CalculateSpawnPosition(enemy);
        }

        private void SpawnBoss(int index = -1)
        {
            PauseSpawn();

            Enemy enemy;

            if (index != -1)
            {
                GameObject boss = Instantiate(enemyBossPrefabs[index]);
                SetParent(boss, enemyParent);
                boss.SetActive(true);
                enemy = boss.GetComponent<Enemy>();
            }
            else
            {
                enemy = GetRandomBossEnemy();
            }

            CalculateSpawnPosition(enemy);
        }

        private Enemy GetRandomEliteEnemy()
        {
            int rand = Random.Range(0, 5);
            GameObject elite = Instantiate(enemyElitePrefabs[rand]);
            SetParent(elite, enemyParent);
            elite.SetActive(true);
            Enemy enemyElite = elite.GetComponent<Enemy>();
            return enemyElite;
        }

        private Enemy GetRandomBossEnemy()
        {
            int rand = Random.Range(0, 5);
            GameObject boss = Instantiate(enemyBossPrefabs[rand]);
            SetParent(boss, enemyParent);
            boss.SetActive(true);
            Enemy enemyBoss = boss.GetComponent<Enemy>();
            return enemyBoss;
        }

        public void SleepGreaterEnemies()
        {
            foreach (GameObject go in enemyElitePrefabs)
            {
                Enemy enemy = go.GetComponent<Enemy>();
                if (enemy.isActiveAndEnabled)
                    enemy.SetSleepMode(true);
            }

            foreach (GameObject go in enemyBossPrefabs)
            {
                Enemy enemy = go.GetComponent<Enemy>();
                if (enemy.isActiveAndEnabled)
                    enemy.SetSleepMode(true);
            }
        }

        public void WakeUpGreatersEnemies()
        {
            foreach (GameObject go in enemyElitePrefabs)
            {
                Enemy enemy = go.GetComponent<Enemy>();
                if (enemy.isActiveAndEnabled)
                {
                    enemy.SetSleepMode(false);
                    enemy.SetAlertMode(false);
                }
            }

            foreach (GameObject go in enemyBossPrefabs)
            {
                Enemy enemy = go.GetComponent<Enemy>();
                if (enemy.isActiveAndEnabled)
                {
                    enemy.SetSleepMode(false);
                    enemy.SetAlertMode(false);
                }
            }
        }

        private IEnumerator SpawnEnemies()
        {
            while (true)
            {
                for (int i = 0; i < paralellSpawn; ++i)
                {
                    StartCoroutine(SpawnEnemy());
                    yield return new WaitForSeconds(3);
                }
            }
        }

        private IEnumerator SpawnEnemy()
        {
            for (int y = 0; y < ObjectPooling.instance.GetNumberOfPools(); ++y)
            {
                Enemy enemy;
                do
                {
                    enemy = ObjectPooling.instance.TakeEnemyFromPool(y);
                }
                while (enemy == null);

                CalculateSpawnPosition(enemy);

                yield return new WaitForSeconds(Random.Range(enemiesMinSpawnRate[y], enemiesMaxSpawnRate[y]));
            }
        }

        private void CalculateSpawnPosition(Enemy obj)
        {
            float randomNumberX = Random.Range(-maxSpawnDistance, maxSpawnDistance);
            float randomNumberZ = Random.Range(-maxSpawnDistance, maxSpawnDistance);
            Vector3 randomPosition = player.transform.position + new Vector3(randomNumberX, obj.transform.position.y, randomNumberZ);

            if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, maxSpawnDistance, NavMesh.AllAreas))
            {
                Vector3 camViewPos = cam.WorldToViewportPoint(hit.position);
                Vector3 newCamViewPos = new Vector3(camViewPos.x, 1, camViewPos.z);

                if (newCamViewPos.x > 1.5f || newCamViewPos.x < -0.5f && newCamViewPos.z > 1.5f || newCamViewPos.z < -0.5f)
                {
                    obj.transform.position = hit.position;
                }
                else
                {
                    ObjectPooling.instance.ReturnObjToPool(obj, obj.Name);
                }
            }
            else
            {
                Debug.Log("Failed to position object");
            }
        }
    }
}
