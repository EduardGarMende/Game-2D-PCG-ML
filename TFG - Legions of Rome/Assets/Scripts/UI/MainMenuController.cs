using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MainMenuController : MonoBehaviour
{
    public string gameSceneName = "MainGameScene";

    public GameObject firstButton;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.delta.ReadValue().sqrMagnitude > 0.5)
        {
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }

        if (EventSystem.current.currentSelectedGameObject == null)
        {
            bool keyboardUsed = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
            bool gamepadUsed = Gamepad.current != null && (Gamepad.current.leftStick.ReadValue().sqrMagnitude > 0.1f ||
                Gamepad.current.dpad.ReadValue().sqrMagnitude > 0.1f ||
                Gamepad.current.buttonSouth.wasPressedThisFrame);

            if (keyboardUsed || gamepadUsed) 
            {
                EventSystem.current.SetSelectedGameObject(firstButton);
            }
        }
    }

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
