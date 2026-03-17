using System;
using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    public SpriteRenderer rewardIconRender;
    public GameObject doorBlocker;

    public RoomRewardData assignedReward;
    private bool isOpen = false;

    public static event Action<RoomRewardData> OnDoorEntered;

    public void OpenDoor(RoomRewardData reward)
    {
        assignedReward = reward;

        if (rewardIconRender != null && doorBlocker != null)
        {
            rewardIconRender.sprite = reward.rewardIcon;
            rewardIconRender.enabled = true;
        }
        
        if (doorBlocker != null)
        {
            doorBlocker.SetActive(false);
        }

        isOpen = true;
    }

    public void CloseDoor()
    {
        isOpen = false;
        if (rewardIconRender != null)
        {
            rewardIconRender.enabled = false;
        }
        if (doorBlocker != null)
        {
            doorBlocker.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isOpen && other.CompareTag("Player"))
        {
            OnDoorEntered?.Invoke(assignedReward);
        }
    }
}
