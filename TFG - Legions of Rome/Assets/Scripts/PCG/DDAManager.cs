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
        healthMultiplier = 1.0f;
        speedMultiplier = 1.0f;
        damageMultiplier = 1.0f;
        animSpeedMultiplier = 1.0f;

        switch (profile)
        {
            case "Berserker":
                minEnemies = 5;
                maxEnemies = 6;
                damageMultiplier = 1.5f;
                healthMultiplier = 1.2f;
                break;
            case "Tactic":
                minEnemies = 5;
                maxEnemies = 6;
                speedMultiplier = 1.4f;
                break;
            case "Unskilful":
                minEnemies = 2;
                maxEnemies = 2;
                healthMultiplier = 0.8f;
                damageMultiplier = 0.5f;
                speedMultiplier = 0.8f;
                animSpeedMultiplier = 0.8f;
                break;
            case "Expert":
                minEnemies = 6;
                maxEnemies = 6;
                healthMultiplier = 1.2f;
                damageMultiplier = 1.2f;
                speedMultiplier = 1.2f;
                animSpeedMultiplier = 1.2f;
                break;
            case "Speedrunner":
                minEnemies = 4;
                maxEnemies = 4;
                healthMultiplier = 1.8f;
                damageMultiplier = 0.8f;
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

        Debug.Log("[DDA] El jugador ha muerto/reiniciado. Dificultad devuelta a los valores base.");
    }
}
