using UnityEngine;

namespace Utils
{
    public class VsyncController : MonoBehaviour
    {
        private void OnEnable()
        {
            Application.targetFrameRate = 1000;
            QualitySettings.vSyncCount = 0;
        }

        private void OnDisable()
        {
            Application.targetFrameRate = 144;
            QualitySettings.vSyncCount = 1;
            print("vsync enabled");
        }
    }
}
