using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Defence
{
    public List<GameObject> enemyTargets = new List<GameObject>();
    public int damage = 20;

    public override void enemyEffect(GameObject enemy){
      if(!enemyTargets.Contains(enemy)){
        enemyTargets.Add(enemy);
      }
    }

    public override void enemyLeaveEffect(GameObject enemy){
      if(enemyTargets.Contains(enemy)){
        enemyTargets.Remove(enemy);
      }
    }

    public override void genericEffect(){
      if(enemyTargets.Count > 0){
        int randomIndex = Random.Range(0, enemyTargets.Count);
        var target = enemyTargets[randomIndex].GetComponent<health>();
        target.dealDamage(damage);
      }
    }
}
