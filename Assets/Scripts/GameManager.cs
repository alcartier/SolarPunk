using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int score;

    //Faire un event pour l'ui
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
}
