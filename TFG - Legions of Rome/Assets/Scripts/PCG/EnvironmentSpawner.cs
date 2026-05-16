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

        trapProb = DDAManager.Instance.trapProbability;
        obsProb = DDAManager.Instance.obstacleProbability;

        int trapsSpawned = 0;
        int obstaclesSpawned = 0;

        foreach (Transform node in spawnNodes)
        {
            float randomVal = Random.value;

            if (trapProb > 0f && randomVal <= trapProb)
            {
                if (trapPrefab != null)
                {
                    Instantiate(trapPrefab, node.position, Quaternion.identity, node);
                    trapsSpawned++;
                }
            }
            else if (obsProb > 0f && randomVal <= (trapProb + obsProb))
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
