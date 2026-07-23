using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Playing,
    GameOver
}

public sealed class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState State { get; private set; } = GameState.Playing;
    public float SurvivalTime { get; private set; }

    public event Action GameOverStarted;

    public bool IsPlaying => State == GameState.Playing;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (IsPlaying)
            SurvivalTime += Time.deltaTime;
    }

    public void EndRun()
    {
        if (!IsPlaying)
            return;

        State = GameState.GameOver;
        GameOverStarted?.Invoke();
    }

    public void RestartRun()
    {
        Time.timeScale = 1f;

        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.buildIndex);
    }
}
