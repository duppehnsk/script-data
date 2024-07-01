using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleporter_spawnpoint : MonoBehaviour
{
    public Transform teleportDestination;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TeleportPlayer(other.transform);
        }
    }

    void TeleportPlayer(Transform playerTransform)
    {
        Rigidbody playerRigidbody = playerTransform.GetComponent<Rigidbody>();

        if (teleportDestination != null && playerRigidbody != null)
        {
            playerRigidbody.MovePosition(teleportDestination.position);
            Debug.Log("Игрок телепортирован к точке назначения.");
        }
        else
        {
            Debug.LogError("Точка телепортации или Rigidbody не установлены!");
        }
    }
}