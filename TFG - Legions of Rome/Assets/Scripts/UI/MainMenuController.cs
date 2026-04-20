using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string gameSceneName = "MainGameScene";

    public void StartNormalMode()
    {
        StartGame(GameModeManager.GameMode.Normal);
    }

    public void StartDataCollectionMode()
    {
        StartGame(GameModeManager.GameMode.DataCollection);
    }

    public void StartGodMode()
    {
        StartGame(GameModeManager.GameMode.GodMode);
    }

    private void StartGame(GameModeManager.GameMode mode)
    {
        GameModeManager.Instance.currentMode = mode;
        SceneManager.LoadScene(gameSceneName);
    }
}
