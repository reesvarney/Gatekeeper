using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerHealth : health
{
    public Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void onHealthChange(int healthSet){
        if(healthSet > slider.maxValue){
            slider.maxValue = healthSet;
        }
        slider.value = healthSet;
    }

    void onDestroy(){
        // End game
    }
}
