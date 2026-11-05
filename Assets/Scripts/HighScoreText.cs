using System;
using TMPro;
using UnityEngine;

public class HighScoreText : MonoBehaviour
{
    public TextMeshProUGUI text;

    private void Start()
    {
        text.text = GameManager.Instance.highScore.ToString();
    }
}
