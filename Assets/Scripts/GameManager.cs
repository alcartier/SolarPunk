using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int score;
    
    [SerializeField] private int amountHarvested = 0;
    
    [SerializeField] private int harvestObjective = 15;
    
    public static event Action<int> OnScoreChange;

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

    public void Harvest()
    {
        amountHarvested++;
        
        if (amountHarvested >= harvestObjective)
        {
            Win();
        }
    }

    public void GameOver()
    {
        Debug.Log("Lose Game Score : " + score);
    }

    public void Win()
    {
        Debug.Log("Win Game Score : " + score);
    }
}
