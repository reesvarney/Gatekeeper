using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waveController : MonoBehaviour
{
    public List<GameObject> spawners = new List<GameObject>();
    public List<GameObject> enemyTypes = new List<GameObject>();
    public int postWaveTime = 20;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void startWave(){
        foreach(GameObject spawner in spawners){
            
        }
    }
}
