using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerMagic : MonoBehaviour
{
    public int level { get; private set; } = 0;
    public int max = 150;
    public Slider slider;
    public List<GameObject> defences = new List<GameObject>();
    public List<GameObject> defenceIcons = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        slider.value = level;
        slider.maxValue = max;
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.tag == "Magic"){
            if(level < max){
                var particle = other.gameObject.GetComponent<magicParticle>();
                add(particle.value);
                Destroy(other.gameObject);
            }
        }
    }

    void set(int levelSet){
        if(levelSet > max){
            levelSet = max;
        }
        if(levelSet > slider.maxValue){
            slider.maxValue = levelSet;
        }
        slider.value = levelSet;
        level = levelSet;
        sendMagicEvent();
    }

    public bool canAfford(int cost){
        if(cost > level){
            return false;
        }
        return true;
    }

    void sendMagicEvent(){
        foreach(GameObject icon in defenceIcons){
            icon.SendMessage("onManaChange", level);
        }
    }

    public int add(int num){
        set(level + num);
        return level;
    }

    public int spend(int num){
        set(level - num);
        return level;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
