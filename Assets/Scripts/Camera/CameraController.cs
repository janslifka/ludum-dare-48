using Anglerfish;
using Cinemachine;
using UnityEngine;
using Zenject;

namespace Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] CinemachineVirtualCamera cinemachineVirtualCamera;
        [SerializeField] UnityEngine.Camera camera;

        [Inject] AnglerfishController anglerfish;

        public UnityEngine.Camera Camera => camera;
        
        void Start()
        {
            cinemachineVirtualCamera.Follow = anglerfish.transform;
        }
    }
}
