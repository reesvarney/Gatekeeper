using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class waveController : MonoBehaviour
{
    public List<GameObject> spawners = new List<GameObject>();
    public List<GameObject> enemyTypes = new List<GameObject>();
    public List<GameObject> waveEnemies = new List<GameObject>();
    public GameObject objective;
    public GameObject enemyContainer;
    public Transform player;
    public int postWaveTime = 20;
    private int waveCountdown;
    public int waveSpawnTime = 10;
    public int baseSpawnRate = 0;
    public float spawnRateMultiplier = 2;
    public int currentWave = 0;
    public TextMeshProUGUI canvasWave;
    public TextMeshProUGUI waveTimer;
    private int enemiesToSpawn;

    // Start is called before the first frame update
    void Start()
    {
        startNextWave();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void checkEnemies(){
        if(waveEnemies.Count == 0 && enemiesToSpawn <= 0){
            Debug.Log("Wave Complete");
            // Begin Countdown
            waveCountdown = postWaveTime;
            InvokeRepeating("setWaveTimer", 1f, 1f);
        }
    }

    void setWaveTimer(){
        waveCountdown -= 1;
        waveTimer.text = waveCountdown.ToString();
        if(waveCountdown <= 0){
            startNextWave();
        }
    }

    IEnumerator addEnemy(GameObject enemy, Vector3 position, float waitTime){
        yield return new WaitForSeconds(waitTime);
        var enemyInstance = Instantiate(enemy, position, Quaternion.identity);
        enemyInstance.transform.parent = enemyContainer.transform;
        waveEnemies.Add(enemyInstance);

        void removeFromList(){
            waveEnemies.Remove(enemyInstance);
            checkEnemies();
        }
        
        var currentEnemyHealth = enemyInstance.GetComponent<health>();
        currentEnemyHealth.destroyEvent.AddListener(removeFromList);

        enemiesToSpawn -= 1;
    }

    void startNextWave(){
        CancelInvoke("setWaveTimer");
        enemiesToSpawn = 0;
        currentWave += 1;
        int enemyNumber = (int)Mathf.Floor(baseSpawnRate + (spawnRateMultiplier * currentWave));
        float waitTime = (waveSpawnTime / enemyNumber);
        canvasWave.text = $"Wave {currentWave.ToString()}";
        waveTimer.text = "";
        foreach(GameObject enemyType in enemyTypes){
            var currentEnemyAI = enemyType.GetComponent<EnemyAI>();
            currentEnemyAI.ultimateTarget = objective;
            currentEnemyAI.player = player;
            foreach(GameObject spawner in spawners){
                enemiesToSpawn += enemyNumber;
                for(int i = 0;i < enemyNumber; i++){
                    StartCoroutine(addEnemy(enemyType, spawner.transform.position, i * waitTime));
                }
            }
        }
    }
}
