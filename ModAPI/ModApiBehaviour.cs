using MSCLoader;

using UnityEngine;

namespace TommoJProductions.ModApi
{
    /// <summary>
    /// used for mod api loader starting coroutine.
    /// </summary>
    public class ModApiBehaviour : MonoBehaviour
    {
        // Written, 11.09.2022

        private void OnLevelWasLoaded(int level)
        {
            if (level == 1)
            {
                ModClient.deleteCache();
                Destroy(gameObject);
            }
            Debug.Log("Level was loaded: " + level);
        }
    }
}