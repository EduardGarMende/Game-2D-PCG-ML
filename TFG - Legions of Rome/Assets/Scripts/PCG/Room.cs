using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public enum RoomState
    {
        WaitingForPlayer,
        LookedInCombat,
        Cleared
    }

    public RoomState currentState = RoomState.WaitingForPlayer;
    public Transform playerSpawnPoint;
    public int activeEnemies = 0;

    public Transform[] spawnPoints;
    public ExitDoor[] doors;

    public EnemyWaveData waveData;
    public GameObject telegraphWarningPrefab;

    public static event Action<Room> OnRoomCleared;

    private void Start()
    {
        foreach (var door in doors)
        {
            door.CloseDoor();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (currentState == RoomState.WaitingForPlayer && other.CompareTag("Player"))
        {
            Debug.Log("¡Jugador detectado! Iniciando combate...");
            StartCombat();
        }
    }

    private void StartCombat()
    {
        currentState = RoomState.LookedInCombat;
        StartCoroutine(SpawnWaveRoutine());
    }

    private IEnumerator SpawnWaveRoutine()
    {
        if (waveData == null) yield break;

        int enemiesToSpawn = UnityEngine.Random.Range(waveData.minEnemies, waveData.maxEnemies + 1);
        enemiesToSpawn = Mathf.Min(enemiesToSpawn, spawnPoints.Length); // Asegura que no se intente spawnear más enemigos que puntos disponibles

        activeEnemies = enemiesToSpawn;

        List<Transform> availableSpawnPoints = new List<Transform>(spawnPoints);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, availableSpawnPoints.Count);
            Transform randomNode = availableSpawnPoints[randomIndex];

            availableSpawnPoints.RemoveAt(randomIndex); // Evita reutilizar el mismo punto de spawn

            if (telegraphWarningPrefab != null)
            {
                GameObject warning = Instantiate(telegraphWarningPrefab, randomNode.position, Quaternion.identity);
                Destroy(warning, 1f);
            }

            yield return new WaitForSeconds(1f);

            GameObject enemyPrefab = waveData.allowedEnemies[UnityEngine.Random.Range(0, waveData.allowedEnemies.Length)];
            Instantiate(enemyPrefab, randomNode.position, Quaternion.identity);
        }
    }

    public void EnemyKilled()
    {
        activeEnemies--;
        if (activeEnemies <= 0 && currentState == RoomState.LookedInCombat)
        {
            RoomCleared();
        }
    }

    private void RoomCleared()
    {
        currentState = RoomState.Cleared;
        OnRoomCleared?.Invoke(this);
    }
}
