using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSlow : Defence
{
  public float slowDown = 1.5f;

  public override void enemyEffect(GameObject detected){
    var enemy = detected.GetComponent<EnemyAI>();
    enemy.currentSpeed = enemy.baseSpeed / slowDown;
  }

  public override void enemyLeaveEffect(GameObject detected){
    var enemy = detected.GetComponent<EnemyAI>();
    enemy.currentSpeed = enemy.baseSpeed;
  }

    public override void onUpgrade(){
      slowDown += 0.5f;
    }

}
