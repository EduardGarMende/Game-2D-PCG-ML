using UnityEngine;

[CreateAssetMenu(fileName = "New Reward", menuName = "Procedural/Room Reward")]
public class RoomRewardData : ScriptableObject
{
    public enum RewardType
    {
        Health,
        MaxHealth,
        Shield,
        Armor,
        Bow_Damage,
        Sword_Damage
    }

    public RewardType type;
    public string rewardName;
    public Sprite rewardIcon;

    public float value;
}
