using System;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public const string setsText = "Sets: ";
    public TextMeshProUGUI scoreTMP;
    public TextMeshProUGUI setsTMP;

    public int score;
    public int sets;
    public Ball ball;

    void Start()
    {
        ball.OnPointEnded += HandlePointEnded;
        setsTMP.text = $"{setsText}{sets}";
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
            UpdateSet();
            score = 0;
        }

        scoreTMP.text = score.ToString();
    }

    public void UpdateSet()
    {
        sets++;

        setsTMP.text = $"{setsText}{sets}";
    }
}
