using System;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public float timeLeft;

    public static event Action<float> OnTimerChange;

    private void Start()
    {
        timeLeft = GameManager.Instance.gameDuration;
        StartCoroutine(gameTimer());
    }

    private void OnEnable()
    {
        GameManager.OnGameEnd2 += StopAllCoroutines;
    }

    private void OnDisable()
    {
        GameManager.OnGameEnd2 -= StopAllCoroutines;
    }

    IEnumerator<WaitForSeconds> gameTimer()
    {
        while (true)
        {

            timeLeft--;

            if (timeLeft < 0)
            {
                GameManager.Instance.GameOver();
                StopAllCoroutines();
            }
            else
            {
                OnTimerChange?.Invoke(timeLeft);
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
