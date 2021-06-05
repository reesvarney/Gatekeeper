using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDamage : Defence
{
    public int damage = 5;

    public override void enemyEffect(GameObject detected){
      detected.GetComponent<health>().dealDamage(damage);
    }

    public override void onUpgrade(){
      damage += 1;
    }

}
