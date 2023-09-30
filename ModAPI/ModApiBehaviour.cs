using System;
using System.Runtime.InteropServices;
using System.Security.Policy;

using MSCLoader;

using TommoJProductions.ModApi.Attachable;

using UnityEngine;

namespace TommoJProductions.ModApi
{
    /// <summary>
    /// used for mod api loader starting coroutine.
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        // Written, 11.09.2022

        private void OnLevelWasLoaded(int level)
        {
            Debug.Log("[ModAPI.Level] A level was loaded: " + Application.loadedLevelName);

            switch (level)
            {
                case 3: //GAME
                    ModClient.refreshCache();

                    ES2.Init();

                    StartCoroutine(ModApiLoader.loadModApi());
                    break;
                case 1: // MAIN MENU
                    break;
            }
        }
    }
}