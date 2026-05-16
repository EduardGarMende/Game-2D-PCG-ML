using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public static GameOverController Instance { get; private set; }

    public GameObject gameOverPanel;
    public GameObject victoryPanel;

    public string mainMenuSceneName = "MenuScene";

    private void Awake()
    {
        if (Instance != null && Instance != this) { 
            Destroy(gameObject); return; 
        }
        Instance = this;
    }

    private void Start()
    {
        gameOverPanel.SetActive(false);
    }

    public void StartGameOverSequence(float delay)
    {
        StartCoroutine(DelayedGameOverRoutine(delay));
    }

    private System.Collections.IEnumerator DelayedGameOverRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowGameOver();
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);

        Time.timeScale = 0f; // Pause the game

        DDAManager.Instance.ResetToDefault(); // Reset DDA modifiers to default
        ValidationTelemetryManager.Instance.ResetTelemetryState();
    }

    public void ShowVictory()
    {
        if (victoryPanel != null) victoryPanel.SetActive(true);
        Time.timeScale = 0f;

        if (DDAManager.Instance != null) DDAManager.Instance.ResetToDefault();
        ValidationTelemetryManager.Instance.ResetTelemetryState();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Resume the game
        GameModeManager.Instance.StartNewSession(); // Start a new game session
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload current scene
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Resume the game
        SceneManager.LoadScene(mainMenuSceneName); // Load main menu scene
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}
