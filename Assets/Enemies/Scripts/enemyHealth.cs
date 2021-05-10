using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enemyHealth : health
{
    public Slider slider;
    public GameObject magicPrefab;
    public int magicParticles;
    public Transform playerTransform;
    public float particleSpawnRange = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        magicParticles = Random.Range(1,4);
        playerTransform = (GetComponent<EnemyAI>()).player;
        var particle = magicPrefab.GetComponent<magicParticle>();
        particle.playerTransform = playerTransform;
    }

    void onHealthChange(int healthSet){
        if(healthSet > slider.maxValue){
            slider.maxValue = healthSet;
        }
        slider.value = healthSet;
    }

    void onDestroy(){
        for(int i = 0; i < magicParticles; i++){
            Vector2 spawnPos = (Vector2)transform.position + (Random.insideUnitCircle * particleSpawnRange);
            Instantiate(magicPrefab, spawnPos, Quaternion.identity);
        }
        Destroy(this.gameObject);
    }   
}
