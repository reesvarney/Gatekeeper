using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class menuController : MonoBehaviour
{
    public TextMeshProUGUI highScoreText;
    
    // Start is called before the first frame update
    void Start()
    {
        highScoreText.text = $"High Score: {PlayerPrefs.GetInt("highScore", 0)}";
    }

    void play(){
        SceneManager.LoadScene("level1");   
    }

    void exit(){
        Application.Quit();
    }
}
