using System;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreTMP;
    public int score;
    public Ball ball;

    void Start()
    {
        ball.OnPointEnded += HandlePointEnded;
    }

    void HandlePointEnded(string lastHitter)
    {
        if (lastHitter == "Player")
        {
            UpdateScore();
        }
        else
        {
            Debug.Log("Enemy Scored");
        }
    }

    public void UpdateScore()
    {
        if (score < 30)
        {
            score += 15;
        }
        else if (score < 40)
        {
            score += 10;
        }
        else if (score >= 40)
        {
            score = 0;
        }

        scoreTMP.text = score.ToString();
    }
}
