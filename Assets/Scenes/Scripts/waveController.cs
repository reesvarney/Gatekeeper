using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waveController : MonoBehaviour
{
    public List<GameObject> spawners = new List<GameObject>();
    public List<GameObject> enemyTypes = new List<GameObject>();
    public List<GameObject> waveEnemies = new List<GameObject>();
    public GameObject objective;
    public GameObject enemyContainer;
    public Transform player;
    public int postWaveTime = 20;
    public int waveSpawnTime = 10;
    public int baseSpawnRate = 0;
    public float spawnRateMultiplier = 2;
    public int currentWave = 1;

    // Start is called before the first frame update
    void Start()
    {
        startWave();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator addEnemy(GameObject enemy, Vector3 position, float waitTime){
        yield return new WaitForSeconds(waitTime);
        var enemyInstance = Instantiate(enemy, position, Quaternion.identity);
        enemyInstance.transform.parent = enemyContainer.transform;
        waveEnemies.Add(enemyInstance);
    }

    void startWave(){
        var enemyNumber = Mathf.Floor(baseSpawnRate + (spawnRateMultiplier * currentWave));
        float waitTime = (waveSpawnTime / enemyNumber);
        foreach(GameObject enemyType in enemyTypes){
            var currentEnemyAI = enemyType.GetComponent<EnemyAI>();
            currentEnemyAI.ultimateTarget = objective;
            currentEnemyAI.player = player;
            
            foreach(GameObject spawner in spawners){
                for(int i = 0;i < enemyNumber; i++){
                    StartCoroutine(addEnemy(enemyType, spawner.transform.position, i * waitTime));
                }
            }
        }
    }
}
