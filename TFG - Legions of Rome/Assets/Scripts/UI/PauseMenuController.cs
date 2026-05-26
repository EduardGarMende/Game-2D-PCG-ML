using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenuPanel;

    public string mainMenuSceneName = "MenuScene";

    public static bool isGamePaused = false;

    private GameControls controls;

    public GameObject pauseFirstButton;

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

        if (Mouse.current != null && Mouse.current.delta.ReadValue().sqrMagnitude > 0.5)
        {
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }

        if (EventSystem.current.currentSelectedGameObject == null && isGamePaused)
        {
            bool keyboardUsed = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
            bool gamepadUsed = Gamepad.current != null && (Gamepad.current.leftStick.ReadValue().sqrMagnitude > 0.1f ||
                Gamepad.current.dpad.ReadValue().sqrMagnitude > 0.1f ||
                Gamepad.current.buttonSouth.wasPressedThisFrame);
            if (keyboardUsed || gamepadUsed) 
            {
                if (pauseFirstButton != null)
                    EventSystem.current.SetSelectedGameObject(pauseFirstButton);
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
        if (pauseFirstButton != null)
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
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
