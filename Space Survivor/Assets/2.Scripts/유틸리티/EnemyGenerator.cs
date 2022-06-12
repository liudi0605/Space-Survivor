using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public Vector2 SpawnArea;

    public float spawnTime = 0.5f;

    public Transform parent;

    public List<WaveObject> waveObjects = new List<WaveObject>();

    [Space]
    public List<EnemyObject> enemyPools = new List<EnemyObject>();
    public List<GameObject> SpawnedEnemy = new List<GameObject>();

    [Space]
    [SerializeField] private Transform player;

    public static EnemyGenerator instance;

    private Coroutine enemySpawnCoroutine;

    private void Awake()
    {
        instance = this;
    }

    public void CheckWave()
    {
        for(int i = 0; i < waveObjects.Count; i++)
        {
            if(waveObjects[i].StartWaveTime == GameManager.instance.getCurrentTime())
            {
                StartWave(waveObjects[i]);
            }

            if (waveObjects[i].StopWaveTime == GameManager.instance.getCurrentTime())
            {
                StopWave(waveObjects[i]);
            }
        }
    }

    private void StartWave(WaveObject wave)
    {
        switch (wave.waveType)
        {
            case waveType.summonedPeriodically:
                wave.waveCoroutine = StartCoroutine(wave.SummonPreiodically());
                break;

            case waveType.summonBoss:

                break;
        }
    }

    private void StopWave(WaveObject wave)
    {
        if (wave.waveCoroutine == null)
            return;

        StopCoroutine(wave.waveCoroutine);
        wave.waveCoroutine = null;
    }

    public void StopAllWave()
    {
        for(int i = 0; i < waveObjects.Count; i++)
        {
            StopWave(waveObjects[i]);
        }
    }


    public void StartEnemySpawn()
    {
        if(enemySpawnCoroutine != null)
        {
            StopCoroutine(enemySpawnCoroutine);
        }

        enemySpawnCoroutine = StartCoroutine(StartSpawnEnemy());
    }

    public void StopEnemySpawn()
    {
        if (enemySpawnCoroutine == null)
            return;

        StopCoroutine(enemySpawnCoroutine);
        enemySpawnCoroutine = null;
    }

    private IEnumerator StartSpawnEnemy()
    {
        while(true)
        {
            GenerateEnemy(EnemyType.Scout1, GenerateSpawnPosition());

            yield return new WaitForSeconds(spawnTime);
        }
    }

    public void EnQueueEnemy(EnemyStat stat)
    {
        foreach(EnemyObject pool in enemyPools)
        {
            if(pool.type == stat.GetType())
            {
                pool.EnQueue(stat.gameObject);
            }

            break;
        }
    }

    public void GenerateEnemy(EnemyType type, Vector2 position)
    {
        foreach (EnemyObject pool in enemyPools)
        {
            if (pool.type == type)
            {
                var success = pool.DeQueue(position);

                if(success != null)
                {
                    var enemy = Instantiate(success, position, Quaternion.identity, parent);

                    enemy.GetComponent<EnemyStat>().SetTarget(player);

                    SpawnedEnemy.Add(enemy);
                }

                //Debug.Log("spawn enemy");
            }
        }
    }

    public void GenerateEnemy2(EnemyObject enemy)
    {
        if (!GameManager.instance.gameStart)
            return;

        var success = enemy.DeQueue(GenerateSpawnPosition());

        if (success != null)
        {
            var spawnEnemy = Instantiate(success, GenerateSpawnPosition(), Quaternion.identity, parent);

            spawnEnemy.GetComponent<EnemyStat>().SetTarget(player);

            SpawnedEnemy.Add(spawnEnemy);
        }

        //Debug.Log("spawn enemy");
    }

    private GameObject GetEnemyPrefab(EnemyType type)
    {
        foreach (EnemyObject enemyPool in enemyPools)
        {
            if (enemyPool.type == EnemyType.Attacker1)
            {
                return enemyPool.Object;
            }
        }

        return null;
    }

    private Vector3 GenerateSpawnPosition()
    {
        Vector3 position = new Vector3();

        float f = Random.value > 0.5f ? -1f : 1f;
        if(Random.value > 0.5f)
        {
            position.x = Random.Range(-SpawnArea.x, SpawnArea.x);
            position.y = SpawnArea.y * f;
        }
        else
        {
            position.y = Random.Range(-SpawnArea.y, SpawnArea.y);
            position.x = SpawnArea.x * f;
        }

        position.z = 0;

        return player.position + position;
    }

    public void DeleteAllEnemy()
    {
        for(int i = 0; i < SpawnedEnemy.Count; i++)
        {
            if(SpawnedEnemy[i].gameObject.activeSelf)
            {
                SpawnedEnemy[i].GetComponent<EnemyStat>().EnQueueThisEnemy();
            }
        }
    }
}

/*[System.Serializable]
public class EnemyPool
{
    public Stack<GameObject> enemyStack = new Stack<GameObject>();

    public GameObject Object;
    public Transform parent;
    public EnemyType type;

    public void EnQueue(GameObject enemy)
    {
        enemyStack.Push(enemy);
    }

    public GameObject DeQueue(Vector2 position)
    {
        if (enemyStack.Count > 0)
        {
            var enemy = enemyStack.Pop();

            enemy.transform.position = position;
            enemy.SetActive(true);
            enemy.GetComponent<EnemyStat>().ResetStat();

            return null;
        }
        else
        {
            return Object;
        }
    }
}*/


