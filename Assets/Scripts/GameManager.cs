using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int score;

    //Faire un event pour l'ui
    public static event Action<int> OnScoreChange;
    
    public void ChangeScore(int amount)
    {
        score += amount;
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
}
