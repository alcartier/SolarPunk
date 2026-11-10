using UnityEngine;
using LitMotion;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{

    [Header("References")]
    public TextMeshProUGUI timerText;
    public Image timerFill;
    private MotionHandle currentMotion;

    private float gameDuration;

    private void Start()
    {
        gameDuration = GameManager.Instance.gameDuration;

        //UpdateTimer(gameDuration);
    }

    private void OnEnable()
    {
        GameTimer.OnTimerChange += UpdateTimer;
        GameManager.OnGameEnd2 += EndTimer;
    }

    private void OnDisable()
    {
        GameTimer.OnTimerChange -= UpdateTimer;
        GameManager.OnGameEnd2 -= EndTimer;
    }

    private void UpdateTimer(float newTime)
    {
        timerText.text = (newTime+1).ToString();

        currentMotion = LMotion.Create(timerFill.fillAmount, newTime/gameDuration, 1f)
        .Bind(value => timerFill.fillAmount = value);
    }

    private void EndTimer()
    {
        timerText.text = "0";
        if (currentMotion != null)
        {
            currentMotion.Cancel();
        }
    }
}
