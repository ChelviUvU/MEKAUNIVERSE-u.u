using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LivesManager : MonoBehaviour
{
    public int currentLives = 3;
    public GameObject gameOverPanel;
    public TMP_Text livesText;

    public ScoreManager scoreManager;

    public void LoseLife()
    {
        currentLives -= 1;
        livesText.text = currentLives.ToString();
        if(currentLives <= 0)
        {
            Time.timeScale = 0;
            gameOverPanel.SetActive(true);

            scoreManager.HighScoreUpdate();

        }
    }
}
