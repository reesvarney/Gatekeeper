using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class objectiveHealth : health
{
    public Slider slider;

    public override void onHealthChange(int healthSet){
        if(healthSet > slider.maxValue){
            slider.maxValue = healthSet;
        }
        slider.value = healthSet;
    }

    public override void onDestroy(){

    }
}
