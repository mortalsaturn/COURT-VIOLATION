using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreTMP;
    public int score;
    public Ball ball;

    void Update()
    {
        if (ball.bounces == 2)
        {
            UpdateScore();
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
