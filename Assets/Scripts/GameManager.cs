using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int score;
    
    [SerializeField] private int amountHarvested = 0;
    
    [SerializeField] private int harvestObjective = 15;
    
    public int highScore = 0;
    
    public SceneControler sceneControler;
    
    public static event Action<int> OnScoreChange;

    public static event Action<int> OnHarvestChange;
    
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
        if((score += amount) < 0)
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
        sceneControler.LoadGameScene();
    }
    
    public void GameOver()
    {
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
    }

    private void Save()
    {
        PlayerPrefs.SetInt("HighScore", highScore);
    }

    void Load()
    {
        int highScoreSaved = PlayerPrefs.GetInt("HighScore");
        if (highScoreSaved == null)
        {
            highScore = 0;
        }
        else
        {
            highScore = highScoreSaved;
        }

    }
}   
