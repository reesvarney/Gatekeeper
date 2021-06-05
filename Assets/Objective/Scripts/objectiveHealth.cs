using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class objectiveHealth : health
{
    public Slider slider;

    void onHealthChange(int healthSet){
        if(healthSet > slider.maxValue){
            slider.maxValue = healthSet;
        }
        slider.value = healthSet;
    }

    void onDestroy(){
        FindObjectOfType<gameController>().endGame();
    }
}
