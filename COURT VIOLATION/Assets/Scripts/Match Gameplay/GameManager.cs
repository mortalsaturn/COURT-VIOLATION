using TMPro;
using UnityEngine;

public enum GameState
{
    PlayerServe,
    EnemyServe,
    Playing,
    PlayerScored,
    EnemyScored,
}

public class GameManager : MonoBehaviour
{
    public GameState state;
    public Ball ball;
    public int playerScore;
    public int enemyScore;
    public TextMeshProUGUI playerScoreTMP;
    public TextMeshProUGUI enemyScoreTMP;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        if (ball.bounces == 2 && ball.lastHitter == "Player")
        {
            UpdateScore(playerScore, playerScoreTMP);
        }
        else if (ball.bounces == 2 && ball.lastHitter == "Enemy")
        {
            UpdateScore(enemyScore, enemyScoreTMP);
        }
    }

    public void UpdateScore(int score, TextMeshProUGUI scoreTMP)
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
