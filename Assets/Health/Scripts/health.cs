using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class health : MonoBehaviour
{
    public int baseHealth = 100;
    public int currentHealth;
    public UnityEvent destroyEvent;

    public void setHealth(int healthSet){
        if(healthSet <= 0){
            destroy();
        }
        if(healthSet != currentHealth){
            gameObject.SendMessage("onHealthChange", healthSet);
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

    public bool healToMax(){
        setHealth(baseHealth);
        return (currentHealth <= 0);
    }

    public void destroy(){
        gameObject.SendMessage("onDestroy");
        destroyEvent.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {
        setHealth(baseHealth);
    }
}
