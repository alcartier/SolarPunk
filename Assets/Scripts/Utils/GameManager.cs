using System;
using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int score;
    
    [SerializeField] private int amountHarvested = 0;
    
    [SerializeField] private int harvestObjective = 15;
    
    public int highScore = 0;
    
    public SceneControler sceneControler;

    public float gameDuration = 120f;
    private float timeLeft;

    public static event Action<int> OnScoreChange;

    public static event Action<int> OnHarvestChange;

    public static event Action<bool,int,int> OnGameEnd;
    public static event Action OnGameCleanup;
    
    private static GameManager instance = null;
    public static GameManager Instance => instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        Load();
    }

    public void ChangeScore(int amount)
    {
        score += amount;
        if(score < 0)
        {
            score = 0;
        }
        OnScoreChange?.Invoke(score);
    }

    public void SetScore(int newScore)
    {
        score = newScore;
        OnScoreChange?.Invoke(score);
    }

    public int GetScore()
    {
        return score;
    }

    public int getHarvestObjective()
    {
        return harvestObjective;
    }

    public void Harvest()
    {
        amountHarvested++;
        OnHarvestChange?.Invoke(amountHarvested);
        if (amountHarvested >= harvestObjective)
        {
            Win();
        }
    }

    [ContextMenu("RESTART_GAME")]
    public void StartNewGame()
    {
        score = 0;
        amountHarvested = 0;
    }
    
    public void GameOver()
    {
        OnGameCleanup?.Invoke();
        OnGameEnd?.Invoke(false,score,highScore);
        Debug.Log("Lose Game Score : " + score);
    }

    public void Win()
    {
        Debug.Log("Win Game Score : " + score);
        if (score  > highScore)
        {
            highScore = score;
        }
        Save();
        OnGameCleanup?.Invoke();
        OnGameEnd?.Invoke(true,score,highScore);
    }

    private void Save()
    {
        PlayerPrefs.SetInt("HighScore", highScore);
    }

    void Load()
    {
        if (PlayerPrefs.HasKey("HighScore"))
        {
            int highScoreSaved = PlayerPrefs.GetInt("HighScore");
            highScore = highScoreSaved;
        }
        else
        {
            highScore = 0;
        }
    }
}
