using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSlow : Defence
{
  public override void enemyEffect(GameObject detected){
    var enemy = detected.GetComponent<EnemyAI>();
    enemy.currentSpeed = enemy.baseSpeed / 2;
  }

  public override void enemyLeaveEffect(GameObject detected){
    var enemy = detected.GetComponent<EnemyAI>();
    enemy.currentSpeed = enemy.baseSpeed;
  }
}
