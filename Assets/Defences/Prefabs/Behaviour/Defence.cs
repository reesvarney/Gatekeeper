using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[System.Serializable]
public class Defence : MonoBehaviour
{
    public float range = 3f;
    public float enemyTargettingRange = 3f;
    public float enemyTargettingChance = 1f;
    public int cost = 10;
    public int maxLevel = 3;
    public int baseHealth = 100;
    public TileBase tile;
    public string displayName = "";
    public string displayHint = "";
    public string keySelector = "";
    public bool buildsOnWalls = true;
    public GameObject healthBar;
    [HideInInspector] public GameObject gameObj;

    public Dictionary<Vector3, gameInstance> gameInstances = new Dictionary<Vector3, gameInstance>();

    private void Awake(){
        gameObj = gameObject;
    }

    public class gameInstance{
        public static int level = 1;
        public int health;
        public Vector3Int tilePos;
        public Vector3 worldPos;
        public GameObject gameObject;
        public CircleCollider2D triggerCollider;
        public GameObject instanceHealthBar;
        public Slider instanceHealthBarSlider;
        public GameObject defenceCanvas;
        public bool isOnWall = false;
        public Defence prefab;

        public gameInstance(Vector3Int temp_tilePos, Vector3 temp_worldPos, Defence defencePrefab, bool tileIsOnWall){
            isOnWall = tileIsOnWall;
            tilePos = temp_tilePos;
            worldPos = temp_worldPos;
            prefab = defencePrefab;
            health = defencePrefab.baseHealth;
            gameObject = new GameObject();
            gameObject.transform.parent = prefab.gameObject.transform;
            gameObject.transform.position = worldPos;
            gameObject.tag = "Defence";

            defenceCanvas = new GameObject();
            defenceCanvas.transform.SetParent(gameObject.transform, false);
            defenceCanvas.AddComponent<Canvas>();
            defenceCanvas.AddComponent<CanvasScaler>();
            defenceCanvas.AddComponent<GraphicRaycaster>();
            defenceCanvas.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            defenceCanvas.GetComponent<Canvas>().sortingLayerName = "UI";
            
            instanceHealthBar = Instantiate(defencePrefab.healthBar, new Vector2(0, 0.2f), Quaternion.identity);
            instanceHealthBar.transform.SetParent(defenceCanvas.transform, false);
            instanceHealthBarSlider = instanceHealthBar.GetComponent<Slider>();
            instanceHealthBarSlider.maxValue = health;
            instanceHealthBarSlider.value = health;

            triggerCollider = gameObject.AddComponent(typeof(CircleCollider2D)) as CircleCollider2D;
            triggerCollider.radius = prefab.enemyTargettingRange;
            triggerCollider.isTrigger = true;
            triggerCollider.name = $"{prefab.displayName} Instance ({tilePos.x}, {tilePos.y})";
        }

        public bool dealDamage(int damage){
            health -= damage;
            instanceHealthBarSlider.value = health;
            if(health <= 0){
                prefab.onDestroy(worldPos);
                return true;
            }
            return false;
        }
    }

    public virtual void onDefence(GameObject detected){

    }

    public virtual void onUpgrade(GameObject detected){

    }

    public virtual void onPlayer(GameObject detected){

    }

    public virtual void onEnemy(GameObject detected){

    }

    public void onDestroy(Vector3 location){
        var destroyedInstance = gameInstances[location];
        Destroy(destroyedInstance.gameObject);
        build.Controller.removeDefenceTile(destroyedInstance.tilePos);
        gameInstances.Remove(location);
    }

    public void _onBuild(Vector3Int tilePos, Vector3 worldPos, bool isOnWall){
        worldPos.x += 0.5f;
        worldPos.y += 0.5f;
        gameInstances.Add(worldPos, new gameInstance(tilePos, worldPos, this, isOnWall));
    }
    
    // Update is called once per frame
    public void Update()
    {
        // Detect enemies/ friendly objects within range and call functions
        foreach(var defence in gameInstances){
            
        }
        // Toggle Build mode on keypress
        if (Input.GetKeyDown(keySelector)){
            build.Controller.toggleBuildMode(this);
        }
    }
}
