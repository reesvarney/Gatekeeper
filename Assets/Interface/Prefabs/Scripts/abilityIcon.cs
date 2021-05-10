using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class abilityIcon : MonoBehaviour
{
    public Sprite icon;
    public Slider slider;

    public bool isCooldown = false;
    public float cooldownValue;


    // Start is called before the first frame update
    void Start()
    {
        slider = gameObject.GetComponent<Slider>();
    }

    public void setCooldown(){
        isCooldown = true;
    }

    public void setAvailable(bool isAvailable){
        if(isAvailable){
            
        } else {

        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isCooldown){
            cooldownValue -= Time.deltaTime;
            slider.value = cooldownValue;

            if (cooldownValue <= 0)
            {       
                cooldownValue = 0;
                isCooldown = false;     
            }
        }
    }
}
