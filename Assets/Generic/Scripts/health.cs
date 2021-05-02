using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class health : MonoBehaviour
{
    public int baseHealth = 100;
    public int currentHealth;

    public void setHealth(int healthSet){
        if(healthSet <= 0){
            destroy();
        }
        if(healthSet != currentHealth){
            onHealthChange(healthSet);
        }
        currentHealth = healthSet;
    }

    public bool dealDamage(int damageDealt){
        var newHealth = currentHealth - damageDealt;
        setHealth(newHealth);
        return (newHealth <= 0);
    }

    public bool heal(int healthAdded){
        var newHealth = currentHealth += healthAdded;
        setHealth(newHealth);
        return (newHealth <= 0);
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
        setHealth(baseHealth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
