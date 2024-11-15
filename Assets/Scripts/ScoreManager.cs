using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreText;
    public int currentScore;

    public TMP_Text finalScoreText;
    public TMP_Text highScoreText;
    public void ChangeScore(int points)
    {
        currentScore += points;
        scoreText.text = currentScore.ToString();
    }

    public void HighScoreUpdate()
    {
        if(PlayerPrefs.HasKey("SavedHighScore"))
        {
            if(currentScore > PlayerPrefs.GetInt("SavedHighScore"))
            {
                PlayerPrefs.SetInt("SavedHighScore", currentScore);
            }

        }
        else
        {
            PlayerPrefs.SetInt("SavedHighScore", currentScore);
        }

        finalScoreText.text = currentScore.ToString();
        highScoreText.text = PlayerPrefs.GetInt("SavedHighScore").ToString();
    }
}
