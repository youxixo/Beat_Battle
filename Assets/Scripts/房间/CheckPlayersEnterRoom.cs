using UnityEngine;

public class CheckPlayersEnterRoom : MonoBehaviour
{
    [SerializeField] private RoomController roomController; // 引用房间控制器
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            roomController.PlayEnterRoom();
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (roomController == null)
        {
            roomController = GetComponentInParent<RoomController>();
        }
    }
#endif
}
