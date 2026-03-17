using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Wave", menuName = "Procedural/Enemy Wave")]
public class EnemyWaveData : ScriptableObject
{
    public GameObject[] allowedEnemies;
    public int minEnemies = 2;
    public int maxEnemies = 6;
}
