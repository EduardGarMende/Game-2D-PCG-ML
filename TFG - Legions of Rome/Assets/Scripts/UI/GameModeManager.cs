using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance { get; private set; }

    public enum GameMode
    {
        Normal,
        NoDDA,
        GodMode
    }

    public GameMode currentMode = GameMode.Normal;

    public bool isDDAActive = true;
    public string currentSessionID = "NONE";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartNewSession()
    {
        currentSessionID = System.DateTime.Now.ToString("yyyyMMddHHmmss");
    }
}
