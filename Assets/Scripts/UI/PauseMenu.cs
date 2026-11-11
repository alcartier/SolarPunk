using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    
    [SerializeField] private InputActionReference PauseAction;
    
    public GameObject pauseMenuUI;
    public GameObject darkBackground;

    
    private bool isPaused = false;

    private void OnEnable()
    {
        PauseAction.action.performed += Pause;
    }

    private void OnDisable()
    {
        PauseAction.action.performed -= Pause;
    }
    
    public void Pause(InputAction.CallbackContext context)
    {
        if (isPaused)
        {
            Time.timeScale = 1;
            isPaused = false;
            pauseMenuUI.SetActive(false);
            darkBackground.SetActive(false);
        }
        else
        {
            Time.timeScale = 0;
            isPaused = true;
            pauseMenuUI.SetActive(true);
            darkBackground.SetActive(true);
        }
    }

    public void Resume()
    {
        Time.timeScale = 1;
        isPaused = false;
        pauseMenuUI.SetActive(false);
        darkBackground.SetActive(false);
    }
    
}
