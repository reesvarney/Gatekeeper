using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[System.Serializable]
public class Defence : MonoBehaviour
{
    public float range = 3f;
    
    public int cost = 10;
    public int baseHealth = 100;

    public float effectRate = 3f;

    public TileBase tile;
    public string displayName = "";
    public string displayHint = "";
    public string keySelector = "";
    public bool buildsOnWalls = true;
    public GameObject healthBar;
    public GameObject magicPrefab;

    public Dictionary<Vector3, gameInstance> gameInstances = new Dictionary<Vector3, gameInstance>();

    public class gameInstance {
        public int level = 1;
        public int health;
        public int defaultHealth;
        public int levelHealth;

        public Vector3Int tilePos;
        public Vector3 worldPos;
        public GameObject gameObject;
        public CircleCollider2D triggerCollider;
        public Rigidbody2D rigidbody;

        public defenceHealthBar instanceUI;

        public bool isOnWall = false;
        public Defence prefab;

        public gameInstance(Vector3Int temp_tilePos, Vector3 temp_worldPos, Defence defencePrefab, bool tileIsOnWall){
            isOnWall = tileIsOnWall;
            tilePos = temp_tilePos;
            worldPos = temp_worldPos;
            prefab = defencePrefab;
            defaultHealth = defencePrefab.baseHealth;
            levelHealth = defaultHealth;
            health = defaultHealth;
            gameObject = new GameObject();
            gameObject.transform.parent = prefab.gameObject.transform;
            gameObject.transform.position = worldPos;
            gameObject.tag = "Defence";
            gameObject.name = $"{prefab.displayName} Instance ({tilePos.x}, {tilePos.y})";

            var defenceCanvas = new GameObject();
            defenceCanvas.transform.SetParent(gameObject.transform, false);
            defenceCanvas.AddComponent<Canvas>();
            defenceCanvas.AddComponent<CanvasScaler>();
            defenceCanvas.AddComponent<GraphicRaycaster>();
            defenceCanvas.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            defenceCanvas.GetComponent<Canvas>().sortingLayerName = "UI";
            
            var instanceHealthBar = Instantiate(defencePrefab.healthBar, new Vector2(0, 0.2f), Quaternion.identity);
            instanceHealthBar.transform.SetParent(defenceCanvas.transform, false);
            instanceUI = instanceHealthBar.GetComponent<defenceHealthBar>();
            instanceUI.setHealth(health);

            triggerCollider = gameObject.AddComponent(typeof(CircleCollider2D)) as CircleCollider2D;
            triggerCollider.radius = prefab.range;
            triggerCollider.isTrigger = true;

            rigidbody = gameObject.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;
            rigidbody.isKinematic = true;

            var behaviour = gameObject.AddComponent<gameInstanceBehaviour>();
            behaviour.setDefence(prefab);
        }

        public bool dealDamage(int damage){
            health -= damage;
            instanceUI.setHealth(health);
            if(health <= 0){
                prefab.onDestroy(tilePos);
                return true;
            }
            return false;
        }

        public void heal(int addHealth){
            health += addHealth;
            if(health > levelHealth) {
                health = levelHealth;
            }
            instanceUI.setHealth(health);
        }

        public void repair(){
            health = levelHealth;
            instanceUI.setHealth(health);
        }

        public void upgrade(){
            level += 1;
            levelHealth += 10;
            health = levelHealth;
            triggerCollider.radius += 0.1f;
            instanceUI.setHealth(health);
            instanceUI.setLevel(level);
        }

        public void sell(){
            var magicParticles = (int)Mathf.Round((float)prefab.cost / (float)20.0);
            for(int i = 0; i < magicParticles; i++){
                Vector2 spawnPos = (Vector2)gameObject.transform.position + (Random.insideUnitCircle * 1);
                Instantiate(prefab.magicPrefab, spawnPos, Quaternion.identity);
            }
            prefab.onDestroy(tilePos);
        }
    }


    private void Awake(){

    }

    public virtual void onUpgrade(){

    }

    public virtual void defenceEffect(gameInstance detected){

    }

    public virtual void playerEffect(GameObject detected){

    }

    public virtual void enemyEffect(GameObject detected){

    }

    public virtual void genericEffect(){

    }

    public virtual void playerEnterEffect(GameObject detected){

    }

    public virtual void enemyEnterEffect(GameObject detected){

    }

    public virtual void playerLeaveEffect(GameObject detected){

    }

    public virtual void enemyLeaveEffect(GameObject detected){

    }

    public void onDestroy(Vector3 location){
        if(gameInstances.ContainsKey(location)){
            var destroyedInstance = gameInstances[location];
            var instanceBehaviour = gameInstances[location].gameObject.GetComponent<gameInstanceBehaviour>();
            StopCoroutine(instanceBehaviour.constantLoop);

            foreach(KeyValuePair<string, IEnumerator> entry in instanceBehaviour.targets)
            {
                // welp we cant actually use this key because you cant search for gameobjects with it
                var affectedObject = GameObject.Find(entry.Key);
                StopCoroutine(entry.Value);
                switch(affectedObject.tag){
                    case "Enemy":
                        enemyLeaveEffect(affectedObject);
                        break;
                    case "Player":
                        playerLeaveEffect(affectedObject);
                        break;
                }
            }
            Destroy(destroyedInstance.gameObject);
            build.Controller.removeDefenceTile(location);
            gameInstances.Remove(location);
        }
    }

    public gameInstance _onBuild(Vector3Int tilePos, Vector3 worldPos, bool isOnWall){
        worldPos.x += 0.5f;
        worldPos.y += 0.5f;
        var newInstance = new gameInstance(tilePos, worldPos, this, isOnWall);
        gameInstances.Add(tilePos, newInstance);
        return newInstance;
    }
    
    // Update is called once per frame
    public void Update()
    {
        // Toggle Build mode on keypress
        if (Input.GetKeyDown(keySelector) && !gameController.isPaused){
            build.Controller.toggleBuildMode(this);
        }
    }
}
