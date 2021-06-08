using UnityEngine;

namespace ARWall.Scripts.Setup
{
    public class PreventFromSleeping : MonoBehaviour
    {
        private void Awake()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}
