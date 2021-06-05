using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class abilityIcon : MonoBehaviour
{
    public Sprite icon;
    public Image image;
    public Image border;
    public string abilityName;
    public string key;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI keyText;
    
    // Start is called before the first frame update
    void Start()
    {
        image.sprite = icon;
        nameText.text = abilityName;
        keyText.text = key;
    }


    public void setActive(bool active){
        if(active){
            border.color = new Color32(255,255,255,100);
        } else {
            border.color = new Color32(0,0,0,100);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
