using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaHeal : Defence
{
    public int healAmount = 3;

    public override void playerEffect(GameObject detected){
      detected.GetComponent<health>().heal(healAmount);
    }

    public override void defenceEffect(gameInstance detected){
      if(detected.health != detected.levelHealth){
        detected.heal(healAmount);
      }
    }
}
