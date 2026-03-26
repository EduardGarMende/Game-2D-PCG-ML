using UnityEngine;
using System;
using System.IO;
using System.Globalization;
using System.Collections;

public class TelemetryManager : MonoBehaviour
{
    public static TelemetryManager Instance { get; private set; }

    private string filePath;

    public bool isRecording = false;
    public bool isCombatActive = false;
    private Transform playerTransform;

    // Contadores
    private float combatStartTime;
    private float finalCombatDuration;
    private int totalAttacks;
    private int totalHits;
    private float totalDamageTaken;
    private int totalDefensesUsed;

    private float distanceSum;
    private int distanceSamples;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        filePath = Application.dataPath + "/telemetria_entrenamiento.csv";
        CheckAndCreateCSVHeader();
    }

    private void OnEnable()
    {
        PlayerHealth.OnDamageTaken += RegisterDamageTaken;
        PlayerSkills.OnDashUsed += RegisterDefenseUsed;
        PlayerSkills.OnShieldUsed += RegisterDefenseUsed;
    }

    private void OnDisable()
    {
        PlayerHealth.OnDamageTaken -= RegisterDamageTaken;
        PlayerSkills.OnDashUsed -= RegisterDefenseUsed;
        PlayerSkills.OnShieldUsed -= RegisterDefenseUsed;
    }

    private void CheckAndCreateCSVHeader()
    {
        if (!File.Exists(filePath))
        {
            string header = "APM,Precision,AvgDistance,DamagePerMinute,DefenseUses,RewardChosen\n";
            File.WriteAllText(filePath, header);
            Debug.Log("Archivo CSV creado en: " + filePath);
        }
    }

    public void StartRoomRecording()
    {
        if (isRecording) return;

        totalAttacks = 0;
        totalDamageTaken = 0f;
        totalHits = 0;
        totalDefensesUsed = 0;
        distanceSamples = 0;
        distanceSum = 0f;
        finalCombatDuration = 0f;

        combatStartTime = Time.time;
        isRecording = true;
        isCombatActive = true;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        StartCoroutine(TrackAverageDistanceRoutine());
    }

    public void StopCombatRecording()
    {
        if (!isCombatActive) return;

        isCombatActive = false;
        finalCombatDuration = Time.time - combatStartTime;

        if (finalCombatDuration < 0.1f) finalCombatDuration = 0.1f;
    }

    public void SaveToCSV(string rewardChosen)
    {
        if (!isRecording) return;
        isRecording = false;

        if (totalHits > totalAttacks) totalHits = totalAttacks;

        float apm = (totalAttacks / finalCombatDuration) * 60f;
        float precision = totalAttacks > 0 ? (float)totalHits / totalAttacks : 0f;
        float avgDistance = distanceSamples > 0 ? distanceSum / distanceSamples : 0f;
        float dpm = (totalDamageTaken / finalCombatDuration) * 60f;

        string newDataRow = string.Format(CultureInfo.InvariantCulture, "{0:F2},{1:F2},{2:F2},{3:F2},{4},{5}\n",
            apm, precision, avgDistance, dpm, totalDefensesUsed, rewardChosen);

        File.AppendAllText(filePath, newDataRow);
    }

    public void RegisterAttack()
    {
        if (isCombatActive) totalAttacks++;
    }

    public void RegisterHit()
    {
        if (isCombatActive) totalHits++;
    }

    public void RegisterDamageTaken(float amount)
    {
        if (isCombatActive) totalDamageTaken += amount;
    }

    public void RegisterDefenseUsed(float cooldownTime)
    {
        if (isCombatActive) totalDefensesUsed++;
    }

    private IEnumerator TrackAverageDistanceRoutine()
    {
        while (isCombatActive && playerTransform != null)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            float closestDistance = float.MaxValue;

            foreach (GameObject enemy in enemies)
            {
                float dis = Vector2.Distance(playerTransform.position, enemy.transform.position);
                if (dis < closestDistance)
                {
                    closestDistance = dis;
                }
            }

            if (closestDistance != float.MaxValue)
            {
                distanceSum += closestDistance;
                distanceSamples++;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}
