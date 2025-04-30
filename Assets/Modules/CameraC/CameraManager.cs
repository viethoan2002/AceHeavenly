using UnityEngine;

namespace Modules.CameraC
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance;
        [SerializeField] private UnityEngine.Camera cameraUI;
        [SerializeField] private UnityEngine.Camera cameraMain;
        
        public UnityEngine.Camera CameraUI => cameraUI;
        public UnityEngine.Camera CameraMain => cameraMain;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }
}
