using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class defenceIcon : MonoBehaviour
{
    public GameObject defencePrefab;
    private Defence defence;
    public Sprite defenceImage;
    public Image icon;
    public TextMeshProUGUI cost;
    public TextMeshProUGUI key;
    public GameObject disabledOverlay;

    // Start is called before the first frame update
    void Start()
    {
        defence = defencePrefab.GetComponent<Defence>();
        icon.sprite = defenceImage;
        cost.text = defence.cost.ToString();
        key.text = defence.keySelector;
    }

    void enable(){
        disabledOverlay.GetComponent<Image>().enabled = false;
    }

    void disable(){
        disabledOverlay.GetComponent<Image>().enabled = true;
    }

    void onManaChange(int newMana){
        if(newMana >= defence.cost){
            enable();
        } else if(newMana < defence.cost){
            disable();
        }
    }
}
