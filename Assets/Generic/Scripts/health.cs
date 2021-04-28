using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class health : MonoBehaviour
{
    public int currentHealth = 100;

    public void setHealth(int healthSet){
        if(healthSet <= 0){
            destroy();
        }
        if(healthSet != currentHealth){
            onHealthChange(healthSet);
        }
        currentHealth = healthSet;
    }

    public void dealDamage(int damageDealt){
        var newHealth = currentHealth - damageDealt;
        setHealth(newHealth);
    }

    public void destroy(){
        onDestroy();
    }

    public virtual void onHealthChange(int newHealth){

    }

    public virtual void onDestroy(){

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
