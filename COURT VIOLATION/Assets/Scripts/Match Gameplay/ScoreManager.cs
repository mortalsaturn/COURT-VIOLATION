using System;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public const string setsText = "Sets: ";
    public TextMeshProUGUI playerScoreTMP;
    public TextMeshProUGUI playerSetsTMP;
    public TextMeshProUGUI playerWinTMP;
    public TextMeshProUGUI enemyScoreTMP;
    public TextMeshProUGUI enemySetsTMP;
    public TextMeshProUGUI enemyWinTMP;

    public int playerScore;
    public int enemyScore;

    public int playerSets;
    public int enemySets;

    public Ball ball;

    void Start()
    {
        ball.OnPointEnded += HandlePointEnded;
        playerSetsTMP.text = $"{setsText}{playerSets}";
    }

    void HandlePointEnded(string lastHitter)
    {
        UpdateScore(lastHitter);
    }

    public void UpdateScore(string lastHitter)
    {
        if (lastHitter == "Player")
        {
            if (playerScore < 30)
            {
                playerScore += 15;
            }
            else if (playerScore < 40)
            {
                playerScore += 10;
            }
            else if (playerScore >= 40)
            {
                UpdateSet(lastHitter);
                playerScore = 0;
            }
            playerScoreTMP.text = playerScore.ToString();
        }
        else
        {
            if (enemyScore < 30)
            {
                enemyScore += 15;
            }
            else if (enemyScore < 40)
            {
                enemyScore += 10;
            }
            else if (enemyScore >= 40)
            {
                UpdateSet(lastHitter);
                enemyScore = 0;
            }
            enemyScoreTMP.text = enemyScore.ToString();
        }
    }

    public void UpdateSet(string lastHitter)
    {
        if (lastHitter == "Player")
        {
            playerSets++;
            playerSetsTMP.text = $"{setsText}{playerSets}";
            if (playerSets == 2)
            {
                playerWinTMP.gameObject.SetActive(true);
            }
        }
        else
        {
            enemySets++;
            enemySetsTMP.text = $"{setsText}{enemySets}";
            if (enemySets == 2)
            {
                enemyWinTMP.gameObject.SetActive(true);
            }
        }
    }
}
