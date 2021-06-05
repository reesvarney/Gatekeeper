using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class gameController : MonoBehaviour
{

    public static bool isPaused = false;
    private int oldHighScore;

    public GameObject gameOverCanvas;
    public GameObject UICanvas;
    public GameObject pauseCanvas;
    public TextMeshProUGUI survivedText;
    public TextMeshProUGUI highScoreText;

    void Start(){
        Time.timeScale = 1;
        oldHighScore = PlayerPrefs.GetInt("highScore", 0);
        gameOverCanvas.SetActive(false);
        pauseCanvas.SetActive(false);
        if(isPaused){
            unpauseGame();
        }
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape)){
            if(isPaused){
                unpauseGame();
            } else {
                pauseGame();
            }
        }
    }

    void goToMenu(){
        SceneManager.LoadScene("mainMenu");   
    }

    void pauseTime(){
        Time.timeScale = 0;
        isPaused = true;
    }

    void unpauseTime(){
        Time.timeScale = 1;
        isPaused = false;
    }

    public void pauseGame(){
        pauseTime();
        pauseCanvas.SetActive(true);
    }

    public void unpauseGame(){
        unpauseTime();
        UICanvas.SetActive(true);
        pauseCanvas.SetActive(false);
    }

    public void endGame(){
        pauseTime();
        UICanvas.SetActive(false);

        // save high score
        var highScore = oldHighScore;
        var currentWave = GetComponent<waveController>().currentWave;
        if(currentWave > highScore){
            PlayerPrefs.SetInt("highScore", currentWave);
            highScore = currentWave;
            // Display new high score text
        }

        survivedText.text = $"Score: {currentWave.ToString()}";
        highScoreText.text =  $"High Score: {currentWave.ToString()}";
        gameOverCanvas.SetActive(true);
    }
}
