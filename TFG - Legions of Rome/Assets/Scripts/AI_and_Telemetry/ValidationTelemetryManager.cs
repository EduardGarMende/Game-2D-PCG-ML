using System;
using System.IO;
using System.Globalization;
using UnityEngine;

public class ValidationTelemetryManager : MonoBehaviour
{
    public static ValidationTelemetryManager Instance { get; private set; }

    private string filePath;

    private int currentRoomIndex = 1;
    private float roomStartTime;
    private float damageTakenInRoom;
    private bool isSavedForThisRoom = false;

    private string currentRoomProfile = "Default";
    private string pendingProfile = "Default";


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        filePath = Path.Combine(Application.persistentDataPath, "validation_results.csv");
        CheckAndCreateCSVHeader();

        Debug.Log($"El archivo CSV está en:\n{filePath}");
    }

    private void OnEnable()
    {
        MLClient.OnProfileReceived += HandleProfileReceived;
        PlayerHealth.OnDamageTaken += HandleDamageTaken;
    }

    private void OnDisable()
    {
        MLClient.OnProfileReceived -= HandleProfileReceived;
        PlayerHealth.OnDamageTaken -= HandleDamageTaken;
    }

    private void CheckAndCreateCSVHeader()
    {
        if (!File.Exists(filePath))
        {
            string header = "Session_ID,DDA_Active,Room_Index,Active_Profile,Time_In_Room,Damage_Taken,Remaining_Health_Percentage,Room_Cleared\n";
            File.WriteAllText(filePath, header);
        }
    }

    private void HandleProfileReceived(string newProfile)
    {
        pendingProfile = newProfile;
    }

    private void HandleDamageTaken(float damageAmount)
    {
        damageTakenInRoom += damageAmount;
    }

    public void StartValidationTimer(int roomIndex)
    {
        currentRoomIndex = roomIndex;
        roomStartTime = Time.time;
        damageTakenInRoom = 0f;
        isSavedForThisRoom = false;

        currentRoomProfile = pendingProfile;
    }

    public void SaveValidationData(PlayerHealth playerHealth, bool roomCleared)
    {
        if (isSavedForThisRoom) return;
        isSavedForThisRoom = true;

        float timeInRoom = Time.time - roomStartTime;

        string sessionID = "UNKNOWN";
        bool isDDA = false;

        if (GameModeManager.Instance != null)
        {
            sessionID = GameModeManager.Instance.currentSessionID;
            isDDA = GameModeManager.Instance.isDDAActive;
        }

        float healthPct = 0f;
        if (playerHealth != null && roomCleared)
        {
            healthPct = (playerHealth.currentHealth / playerHealth.maxHealth);
        }

        string dataRow = string.Format(CultureInfo.InvariantCulture,
            "{0},{1},{2},{3},{4:F2},{5:F2},{6:F2},{7}\n",
            sessionID, isDDA, currentRoomIndex, currentRoomProfile, timeInRoom, damageTakenInRoom, healthPct, roomCleared);

        File.AppendAllText(filePath, dataRow);
    }

    public void ResetTelemetryState()
    {
        currentRoomProfile = "Default";
        pendingProfile = "Default";
    }
}
