using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenuPanel;

    public string mainMenuSceneName = "MenuScene";

    public static bool isGamePaused = false;

    private GameControls controls;

    private void Awake() => controls = new GameControls();

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Start()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;
    }

    private void Update()
    {
        if (controls.Gameplay.Pause.triggered)
        {
            if (isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;
    }

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isGamePaused = true;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        isGamePaused = false;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
                // Si estamos probando dentro del editor de Unity, apaga el modo Play
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                    // Si ya es el juego compilado final, cierra la aplicación
                    Application.Quit();
        #endif
    }
}
