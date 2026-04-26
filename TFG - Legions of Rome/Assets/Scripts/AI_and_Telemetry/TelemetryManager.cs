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

    private int totalMeleeAttacks;
    private int totalRangedAttacks;
    private int totalHits;

    private float totalDamageTaken;
    private float totalDamageDealt
        ;
    private int dashCount;
    private int shieldCount;

    private float distanceSum;
    private int distanceSamples;
    private int riskZoneTiks;
    private const float RISK_ZONE_RADIUS = 3.0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (GameModeManager.Instance != null && GameModeManager.Instance.currentMode == GameModeManager.GameMode.DataCollection)
        {
            filePath = Application.dataPath + "/telemetria_entrenamiento.csv";
        }
        else
        {
            filePath = Application.dataPath + "/telemetria_pruebas_normales.csv";
        }

        CheckAndCreateCSVHeader();
    }

    private void OnEnable()
    {
        PlayerHealth.OnDamageTaken += RegisterDamageTaken;
        PlayerSkills.OnDashUsed += RegisterDash;
        PlayerSkills.OnShieldUsed += RegisterShield;
    }

    private void OnDisable()
    {
        PlayerHealth.OnDamageTaken -= RegisterDamageTaken;
        PlayerSkills.OnDashUsed -= RegisterDash;
        PlayerSkills.OnShieldUsed -= RegisterShield;
    }

    private void CheckAndCreateCSVHeader()
    {
        if (!File.Exists(filePath))
        {
            string header = "APM,Precision,AvgDistance,DamageTakenPerMin,DamageDealtPerMin,RangedRatio,DashPerMin,ShieldPerMin,TimeInRiskZone,RewardChosen\n";
            File.WriteAllText(filePath, header);
        }
    }

    public void StartRoomRecording()
    {
        if (isRecording) return;

        totalMeleeAttacks = 0;
        totalRangedAttacks = 0;
        totalDamageTaken = 0f;
        totalDamageDealt = 0f;
        totalHits = 0;
        dashCount = 0;
        shieldCount = 0;

        distanceSamples = 0;
        distanceSum = 0f;
        riskZoneTiks = 0;
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

        if (finalCombatDuration <= 0.1f) finalCombatDuration = 0.1f;
    }

    public void SaveToCSV(string rewardChosen)
    {
        if (!isRecording) return;
        isRecording = false;

        int totalAttacks = totalMeleeAttacks + totalRangedAttacks;

        if (totalHits > totalAttacks) totalHits = totalAttacks;

        float apm = (totalAttacks / finalCombatDuration) * 60f;
        float precision = totalAttacks > 0 ? (float)totalHits / totalAttacks : 0f;
        float avgDistance = distanceSamples > 0 ? distanceSum / distanceSamples : 0f;

        float damageTakenPerMin = (totalDamageTaken / finalCombatDuration) * 60f;
        float damageDealtPerMin = (totalDamageDealt / finalCombatDuration) * 60f;

        float rangedRatio = totalAttacks > 0 ? (float)totalRangedAttacks / totalAttacks : 0f;

        float dashPerMin = (dashCount / finalCombatDuration) * 60f;
        float shieldPerMin = (shieldCount / finalCombatDuration) * 60f;

        float timeInRiskZone = distanceSamples > 0 ? (float)riskZoneTiks / distanceSamples : 0f;

        string newDataRow = string.Format(CultureInfo.InvariantCulture, "{0:F2},{1:F2},{2:F2},{3:F2},{4:F2},{5:F2},{6:F2},{7:F2},{8:F2},{9}\n",
            apm, precision, avgDistance, damageTakenPerMin, damageDealtPerMin, rangedRatio, dashPerMin, shieldPerMin, timeInRiskZone, rewardChosen);

        File.AppendAllText(filePath, newDataRow);

        MLClient.PlayerMetricsData metrics = new MLClient.PlayerMetricsData()
        {
            APM = apm,
            Precision = precision,
            AvgDistance = avgDistance,
            DamageTakenPerMin = damageTakenPerMin,
            DamageDealtPerMin = damageDealtPerMin,
            RangedRatio = rangedRatio,
            DashPerMin = dashPerMin,
            ShieldPerMin = shieldPerMin,
            TimeInRiskZone = timeInRiskZone,
            RewardChosen = rewardChosen
        };

        MLClient.Instance.RequestProfilePrediction(metrics);
    }

    public void RegisterMeleeAttack()
    {
        if (isCombatActive) totalMeleeAttacks++;
    }

    public void RegisterRangedAttack()
    {
        if (isCombatActive) totalRangedAttacks++;
    }

    public void RegisterHit()
    {
        if (isCombatActive) totalHits++;
    }

    public void RegisterDamageTaken(float amount)
    {
        if (isCombatActive) totalDamageTaken += amount;
    }

    public void RegisterDamageDealt(float amount)
    {
        if (isCombatActive) totalDamageDealt += amount;
    }

    public void RegisterDash(float cooldownTime)
    {
        if (isCombatActive) dashCount++;
    }

    public void RegisterShield(float cooldownTime)
    {
        if (isCombatActive) shieldCount++;
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

                if (closestDistance <= RISK_ZONE_RADIUS)
                {
                    riskZoneTiks++;
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}
