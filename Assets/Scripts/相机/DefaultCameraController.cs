using Unity.Cinemachine;
using UnityEngine;

public class DefaultCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera defaultCamera;

    private CameraManager cameraManager => CameraManager.Instance;
    private void Awake()
    {
        cameraManager.SetDefaultCamera(defaultCamera);
    }
}
