using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneControler : MonoBehaviour
{
    public bool fadeIn = true;
    public float fadeDuration = 1f;
    public Image image;
    [SerializeField] InputActionAsset inputActions;
    
    private void Start()
    {
        GameManager.Instance.sceneControler = this;

        if (fadeIn)
        {
            StartCoroutine(FadeOut("MainMenu",fadeDuration));
        }
    }

    public void LoadGameScene()
    {
        inputActions.FindActionMap("Player").Enable();
        GameManager.Instance.StartNewGame();
        StartCoroutine(FadeIn("Game", fadeDuration));
    }

    public void GoToMainMenu()
    {
        StartCoroutine(FadeIn("MainMenu",fadeDuration));
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }

    IEnumerator FadeIn(string sceneName, float duration)
    {
        float t = 0;
        Color c = image.color;
        while(t < duration)
        {
            t += Time.deltaTime;
            c.a = t / duration;
            image.color = c;
            yield return null;
        }
        
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
    
    IEnumerator FadeOut(string sceneName, float duration)
    {
        float t = 0;
        Color c = image.color;
        while(t < duration)
        {
            t += Time.deltaTime;
            c.a = 1f - (t / duration);
            image.color = c;
            yield return null;
        }
    }
}
