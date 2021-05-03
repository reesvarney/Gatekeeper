using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameInstanceBehaviour : MonoBehaviour
{

    public Dictionary<string, IEnumerator> targets = new Dictionary<string, IEnumerator>();
    public IEnumerator constantLoop;
    public bool ready = false;
    private Defence defencePrefab;

    private IEnumerator onEnemy(GameObject detected){
        while (true){
            yield return new WaitForSeconds(defencePrefab.effectRate);
            defencePrefab.enemyEffect(detected);
        }
    }

    private IEnumerator onDefence(GameObject detected){
        while (true){
            yield return new WaitForSeconds(defencePrefab.effectRate);
            var defence = detected.transform.parent.gameObject.GetComponent<Defence>();
            Vector3 defencePos = detected.transform.position;
            var defenceInstance = defence.gameInstances[defencePos];
            defencePrefab.defenceEffect(defenceInstance);
        }
    }

    private IEnumerator onPlayer(GameObject detected){
        while (true){
            yield return new WaitForSeconds(defencePrefab.effectRate);
            defencePrefab.playerEffect(detected);
        }
    }

    private IEnumerator onConstant(){
        while (true){
            yield return new WaitForSeconds(defencePrefab.effectRate);
            defencePrefab.genericEffect();
        }
    }

    public void setDefence(Defence temp_defencePrefab){
        defencePrefab = temp_defencePrefab;
        ready = true;
        constantLoop = onConstant();
        StartCoroutine(onConstant());
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        checkAction(other);
    }

    void OnTriggerStay2D (Collider2D other)
    {
        checkAction(other);
    }

    void OnTriggerExit2D (Collider2D other){
        var key = other.gameObject.name;
        if(targets.ContainsKey(key)){
            StopCoroutine(targets[key]);
            targets.Remove(key);
            if(other.gameObject != null){
                switch (other.gameObject.tag){
                    case "Enemy":
                        defencePrefab.enemyLeaveEffect(other.gameObject);
                        break;
                    case "Player":
                        defencePrefab.playerLeaveEffect(other.gameObject);
                        break;
                }
            }
        }
    }

    public void checkAction(Collider2D other){
        var otherId = other.gameObject.name;
        if(ready && !targets.ContainsKey(otherId)){
            switch (other.gameObject.tag){
                case "Enemy":
                    defencePrefab.enemyEnterEffect(other.gameObject);
                    targets.Add(otherId, onEnemy(other.gameObject));
                    break;
                case "Player":
                    defencePrefab.playerEnterEffect(other.gameObject);
                    targets.Add(otherId, onPlayer(other.gameObject));
                    break;
                case "Defence":
                    targets.Add(otherId, onDefence(other.gameObject));
                    break;
            }

            if(targets.ContainsKey(otherId)){
                StartCoroutine(targets[otherId]);
            }
        }
    }

}
