using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class defenceHealthBar : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    private Slider healthBar;
    // Start is called before the first frame update
    void Start()
    {
        healthBar = GetComponent<Slider>();
    }

    public void setLevel(int lvl){
        levelText.text = lvl.ToString();
    }

    public void setHealth(int newHealth){
        if(healthBar != null){
            if(newHealth > healthBar.maxValue){
                healthBar.maxValue = newHealth;
            }
            healthBar.value = newHealth;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
