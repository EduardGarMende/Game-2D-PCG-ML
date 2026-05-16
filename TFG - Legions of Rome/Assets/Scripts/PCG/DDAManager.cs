using UnityEngine;

public class DDAManager : MonoBehaviour
{
    public static DDAManager Instance { get; private set; }

    public int minEnemies = 3;
    public int maxEnemies = 4;

    public float healthMultiplier = 1.0f;
    public float speedMultiplier = 1.0f;
    public float damageMultiplier = 1.0f;
    public float animSpeedMultiplier = 1.0f;

    public float trapProbability = 0.1f;
    public float obstacleProbability = 0.2f;

    public float rangedEnemyProbability = 0.3f;

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

    private void OnEnable()
    {
        MLClient.OnProfileReceived += ApplyDDAModifiers;
    }

    private void OnDisable()
    {
        MLClient.OnProfileReceived -= ApplyDDAModifiers;
    }

    public void ApplyDDAModifiers(string profile)
    {
        if (GameModeManager.Instance != null && !GameModeManager.Instance.isDDAActive)
        {
            minEnemies = 3;
            maxEnemies = 5;
            healthMultiplier = 1.0f;
            damageMultiplier = 1.0f;
            speedMultiplier = 1.0f;
            animSpeedMultiplier = 1.0f;
            trapProbability = 0.1f;
            obstacleProbability = 0.2f;
            rangedEnemyProbability = 0.5f;

            Debug.Log($"[DDA BYPASS] Modo estático activo. Perfil [{profile}] ignorado en físicas. Valores neutros aplicados.");
            return;
        }

        healthMultiplier = 1.0f;
        speedMultiplier = 1.0f;
        damageMultiplier = 1.0f;
        animSpeedMultiplier = 1.0f;
        trapProbability = 0.1f;
        obstacleProbability = 0.2f;
        rangedEnemyProbability = 0.3f;

        switch (profile)
        {
            case "Berserker":
                minEnemies = 5;
                maxEnemies = 6;
                damageMultiplier = 1.5f;
                healthMultiplier = 1.2f;
                trapProbability = 0.4f;
                obstacleProbability = 0.1f;
                rangedEnemyProbability = 0.7f;
                break;
            case "Tactic":
                minEnemies = 5;
                maxEnemies = 6;
                speedMultiplier = 1.4f;
                trapProbability = 0.1f;
                obstacleProbability = 0.5f;
                rangedEnemyProbability = 0.2f;
                break;
            case "Unskilful":
                minEnemies = 2;
                maxEnemies = 2;
                healthMultiplier = 0.8f;
                damageMultiplier = 0.5f;
                speedMultiplier = 0.8f;
                animSpeedMultiplier = 0.8f;
                trapProbability = 0f;
                obstacleProbability = 0f;
                rangedEnemyProbability = 0f;
                break;
            case "Expert":
                minEnemies = 6;
                maxEnemies = 6;
                healthMultiplier = 1.2f;
                damageMultiplier = 1.2f;
                speedMultiplier = 1.2f;
                animSpeedMultiplier = 1.2f;
                trapProbability = 0.3f;
                obstacleProbability = 0.3f;
                rangedEnemyProbability = 0.5f;
                break;
            case "Speedrunner":
                minEnemies = 4;
                maxEnemies = 4;
                healthMultiplier = 1.8f;
                damageMultiplier = 0.8f;
                trapProbability = 0.1f;
                obstacleProbability = 0.6f;
                rangedEnemyProbability = 0.3f;
                break;
            default:
                Debug.LogWarning($"[DDA] Perfil no reconocido ({profile}). Usando dificultad normal.");
                minEnemies = 3; maxEnemies = 4;
                break;
        }
    }

    public void ResetToDefault()
    {
        minEnemies = 3;
        maxEnemies = 4;
        healthMultiplier = 1f;
        damageMultiplier = 1f;
        speedMultiplier = 1f;
        animSpeedMultiplier = 1f;
        trapProbability = 0.1f;
        obstacleProbability = 0.2f;
        rangedEnemyProbability = 0.3f;

        Debug.Log("[DDA] El jugador ha muerto/reiniciado. Dificultad devuelta a los valores base.");
    }
}
