using UnityEngine;

public class EnvironmentSpawner : MonoBehaviour
{
    public GameObject trapPrefab;
    public GameObject obstaclePrefab;

    public Transform[] spawnNodes;

    public void SpawnEnvironment()
    {
        float trapProb = 0f;
        float obsProb = 0f;

        if (DDAManager.Instance != null &&
           (GameModeManager.Instance == null || GameModeManager.Instance.currentMode != GameModeManager.GameMode.DataCollection))
        {
            trapProb = DDAManager.Instance.trapProbability;
            obsProb = DDAManager.Instance.obstacleProbability;
        }

        int trapsSpawned = 0;
        int obstaclesSpawned = 0;

        foreach (Transform node in spawnNodes)
        {
            float randomVal = Random.value;

            if (randomVal <= trapProb)
            {
                if (trapPrefab != null)
                {
                    Instantiate(trapPrefab, node.position, Quaternion.identity, node);
                    trapsSpawned++;
                }
            }
            else if (randomVal <= (trapProb + obsProb))
            {
                if (obstaclePrefab != null)
                {
                    Instantiate(obstaclePrefab, node.position, Quaternion.identity, node);
                    obstaclesSpawned++;
                }
            }
        }

        Debug.Log($"ENTORNO: Sala generada. {trapsSpawned} trampas y {obstaclesSpawned} obstáculos creados.");
    }
}
