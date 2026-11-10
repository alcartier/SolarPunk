using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class WinLoseUI : MonoBehaviour
{
    [SerializeField] InputActionAsset inputActions;
    
    public TextMeshProUGUI winLoseText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    
    public GameObject winLosePanel;
    public GameObject backGround;

    private void OnEnable()
    {
        GameManager.OnGameEnd += ActiveMenu;
    }

    private void OnDisable()
    {
        GameManager.OnGameEnd -= ActiveMenu;
    }

    private void ActiveMenu(bool win, int score, int hScore)
    {
        inputActions.FindActionMap("Player").Disable();
        
        winLosePanel.SetActive(true);
        backGround.SetActive(true);
        
        if(win)
        {
            winLoseText.text = "WIN";
        }
        else
        {
            winLoseText.text = "LOSE";
        }
        
        scoreText.text = score.ToString();
        
        highScoreText.text = hScore.ToString();
    }
}
