using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GameObject player;
    public GameObject roomPrefab;

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

        // Cuando mueren los enemigos, recorremos todas las puertas de salida
        foreach (ExitDoor door in clearedRoom.doors)
        {
            // Elegimos una recompensa aleatoria de nuestra lista
            RoomRewardData randomReward = possibleRewards[Random.Range(0, possibleRewards.Length)];

            // Le decimos a la puerta que se abra y muestre esa recompensa
            door.OpenDoor(randomReward);
        }
    }

    private void HandleDoorEntered(RoomRewardData chosenReward)
    {
        Debug.Log("El jugador ha cruzado la puerta. Recompensa elegida: " + chosenReward.rewardName);

        // Aquí, en el futuro, aplicaremos la recompensa al jugador (ej. curarlo, darle daño...)
        // TODO: ApplyRewardToPlayer(chosenReward);

        // Cargamos la siguiente sala inmediatamente
        LoadNewRoom();
    }
}
