using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneControler : MonoBehaviour
{
    
    public Image image;

    private void Start()
    {
        GameManager.Instance.sceneControler = this;
        StartCoroutine(FadeOut("MainMenu",1f));
    }

    public void LoadGameScene()
    {
        StartCoroutine(FadeIn("Game", 1f));
    }

    public void GoToMainMenu()
    {
        StartCoroutine(FadeIn("MainMenu",1f));
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
            c.a = 1f - (t / 1);
            image.color = c;
            yield return null;
        }
    }
}
