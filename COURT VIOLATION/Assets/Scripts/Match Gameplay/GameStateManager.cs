using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public Ball ball;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ball.OnPointEnded += HandlePointEnded;
    }

    // Update is called once per frame
    void Update() { }

    void HandlePointEnded(string lastHitter)
    {
        if (lastHitter == "Player") { }
    }
}
