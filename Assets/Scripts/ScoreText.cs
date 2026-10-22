using TMPro;
using UnityEngine;
using LitMotion;
using LitMotion.Extensions;

public class ScoreText : MonoBehaviour
{
    
    private TextMeshProUGUI text;
    private int score;
    
    public float textAnimationDuration = 2f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    
    private void OnEnable()
    {
        GameManager.OnScoreChange += UpdateScore;
    }

    private void OnDisable()
    {
        GameManager.OnScoreChange -= UpdateScore;
    }
    
    public void UpdateScore(int newScore)
    {
        LMotion.Create(score, newScore, textAnimationDuration)
            .BindToText(text, "{0}");
        score = newScore;
    }
}
