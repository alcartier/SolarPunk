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
    }

    private void OnEnable()
    {
        GameTimer.OnTimerChange += UpdateTimer;
        GameManager.OnGameCleanup += EndTimer;
    }

    private void OnDisable()
    {
        GameTimer.OnTimerChange -= UpdateTimer;
        GameManager.OnGameCleanup -= EndTimer;
    }

    private void UpdateTimer(float newTime)
    {
        timerText.text = (newTime+1).ToString();

        currentMotion = LMotion.Create(timerFill.fillAmount, newTime / gameDuration, 1f)
        .Bind(value => timerFill.fillAmount = value)
        .AddTo(timerFill);
    }

    private void EndTimer()
    {
        timerText.text = "0";
        if (timerFill.fillAmount > 0) {
            currentMotion.Cancel();
            currentMotion = LMotion.Create(timerFill.fillAmount, 0 / gameDuration, 0.25f)
    .Bind(value => timerFill.fillAmount = value)
    .AddTo(timerFill);
        }
    }
}
