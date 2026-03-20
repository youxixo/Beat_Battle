using System;
using System.Collections;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField] private ZombieDate[] zombiesInRoom;
    [SerializeField] private bool isPlayerInRoom = false;

    private EnemyManager enemyManager => EnemyManager.Instance;


    public void PlayEnterRoom()
    {
        if (isPlayerInRoom) return;

        isPlayerInRoom = true;
        foreach (var zombie in zombiesInRoom)
        {
            enemyManager.RegisterEnemy(zombie);
            zombie.SetCanMove(true);
        }
    }

    #if UNITY_EDITOR
    /// <summary>
    /// Called when the script is loaded or a value is changed in the
    /// inspector (Called in the editor only).
    /// </summary>
    private void OnValidate()
    {
        if (zombiesInRoom == null || zombiesInRoom.Length == 0)
        {
            zombiesInRoom = GetComponentsInChildren<ZombieDate>();
        }
    }
    #endif
}
