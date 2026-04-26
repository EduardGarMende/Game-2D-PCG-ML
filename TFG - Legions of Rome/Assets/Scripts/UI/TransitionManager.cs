using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    public CanvasGroup canvasGroup;
    public TextMeshProUGUI ddaText;
    public TextMeshProUGUI roomInfoText;

    public float fadeDuration = 0.5f;

    private string lastProfileDetected = "";
    private bool profileRecived = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    private void OnEnable()
    {
        MLClient.OnProfileReceived += HandleProfileReceived;
    }

    private void OnDisable()
    {
        MLClient.OnProfileReceived -= HandleProfileReceived;
    }

    private void HandleProfileReceived(string profileName)
    {
        lastProfileDetected = profileName;
        profileRecived = true;
    }

    public IEnumerator TransitionToNewRoomRoutine(int currentRoomNum, System.Action changeRoomLogic)
    {
        profileRecived = false;

        ddaText.text = "SISTEMA DDA: Analitzant telemetria...";
        roomInfoText.text = $"SALA {currentRoomNum} COMPLETADA - CARREGANT NIVELL {currentRoomNum + 1}";

        canvasGroup.blocksRaycasts = true;

        yield return StartCoroutine(Fade(1));

        changeRoomLogic?.Invoke();

        float minWaitTime = 1.5f;
        float timeout = 5.0f;
        float timer = 0f;

        while (timer < minWaitTime || (!profileRecived && timer < timeout))
        {
            timer += Time.deltaTime;
            yield return null;
        }

        ddaText.text = $"Perfil detectat: <color=#FFD700>{lastProfileDetected}</color>";

        yield return new WaitForSeconds(2.5f);

        yield return StartCoroutine(Fade(0));

        canvasGroup.blocksRaycasts = false;
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
    }
}
