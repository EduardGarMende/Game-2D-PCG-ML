using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GameObject player;
    public GameObject[] roomPrefabs;

    public RoomRewardData[] possibleRewards;

    private Room currentRoom;

    private void OnEnable()
    {
        Room.OnRoomCleared += HandleRoomCleared;
        ExitDoor.OnDoorEntered += HandleDoorEntered;
    }

    private void OnDisable()
    {
        Room.OnRoomCleared -= HandleRoomCleared;
        ExitDoor.OnDoorEntered -= HandleDoorEntered;
    }

    void Start()
    {
        LoadNewRoom();
    }

    private void LoadNewRoom()
    {
        if (currentRoom != null)
        {
            Destroy(currentRoom.gameObject);
        }

        int randomIndex = Random.Range(0, roomPrefabs.Length);
        GameObject roomPrefab = roomPrefabs[randomIndex];

        GameObject newRoomObj = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);
        currentRoom = newRoomObj.GetComponent<Room>();
        
        if (player != null && currentRoom.playerSpawnPoint != null)
        {
            player.transform.position = currentRoom.playerSpawnPoint.position;

            // IMPORTANTE: Reseteamos la velocidad del jugador para que no entre derrapando
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }
    }

    private void HandleRoomCleared(Room clearedRoom)
    {
        Debug.Log("¡Sala completada! Generando puertas de salida...");
        TelemetryManager.Instance.StopCombatRecording();

        List<RoomRewardData> rewardsList = new List<RoomRewardData>(possibleRewards);

        // Cuando mueren los enemigos, recorremos todas las puertas de salida
        foreach (ExitDoor door in clearedRoom.doors)
        {
            // Elegimos una recompensa aleatoria de nuestra lista
            int randomRewardIndex = Random.Range(0, rewardsList.Count);
            RoomRewardData randomReward = rewardsList[randomRewardIndex];

            rewardsList.RemoveAt(randomRewardIndex); // Evitamos repetir recompensas en otras puertas

            // Le decimos a la puerta que se abra y muestre esa recompensa
            door.OpenDoor(randomReward);
        }
    }

    private void HandleDoorEntered(RoomRewardData chosenReward)
    {
        string rewardString = chosenReward.rewardName;
        TelemetryManager.Instance.SaveToCSV(rewardString);

        if (GameModeManager.Instance != null && GameModeManager.Instance.currentMode == GameModeManager.GameMode.DataCollection)
        {
            PlayerHealth pHealth = player.GetComponent<PlayerHealth>();
            if (pHealth != null) pHealth.ResetToDefault();
        }
        else
        {
            ApplyRewardToPlayer(chosenReward);
        }

        LoadNewRoom();
    }

    private void ApplyRewardToPlayer(RoomRewardData chosenReward)
    {
        if (player == null) return;

        PlayerHealth health = player.GetComponent<PlayerHealth>();
        PlayerCombat combat = player.GetComponent<PlayerCombat>();
        PlayerMovement movement = player.GetComponent<PlayerMovement>();

        switch (chosenReward.type)
        {
            case RoomRewardData.RewardType.Health:
                if (health != null) health.Heal(chosenReward.value); 
                break;
            case RoomRewardData.RewardType.MaxHealth:
                if (health != null) health.IncreaseMaxHelath(chosenReward.value);
                break;
            case RoomRewardData.RewardType.Armor:
                if (health != null) health.AddArmor(chosenReward.value);
                break;
            case RoomRewardData.RewardType.Shield:
                if (health != null) health.ImproveShield(chosenReward.value);
                break;
            case RoomRewardData.RewardType.Sword_Damage:
                if (combat != null) combat.IncreaseSwordDamage(chosenReward.value);
                break;
            case RoomRewardData.RewardType.Bow_Damage:
                if (combat != null) combat.IncreaseRangeDamage(chosenReward.value);
                break;
            case RoomRewardData.RewardType.Velocity:
                if (combat != null) movement.IncreaseSpeed(chosenReward.value);
                break;
        }
    }
}
