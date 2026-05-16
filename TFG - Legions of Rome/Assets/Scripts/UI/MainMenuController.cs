using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string gameSceneName = "MainGameScene";

    public void StartNormalMode()
    {
        StartGame(GameModeManager.GameMode.Normal, true);
    }

    public void StartNoDDAMode()
    {
        StartGame(GameModeManager.GameMode.NoDDA, false);
    }

    public void StartGodMode()
    {
        StartGame(GameModeManager.GameMode.GodMode, true);
    }

    private void StartGame(GameModeManager.GameMode mode, bool isDDAActive)
    {
        GameModeManager.Instance.currentMode = mode;
        GameModeManager.Instance.isDDAActive = isDDAActive;
        GameModeManager.Instance.StartNewSession();
        SceneManager.LoadScene(gameSceneName);
    }
}
