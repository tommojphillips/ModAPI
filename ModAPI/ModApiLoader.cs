using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSCLoader;
using TommoJProductions.ModApi.Attachable;

namespace TommoJProductions.ModApi
{
    /// <summary>
    /// Represents behaviour to load modapi stuff. eg bolt assets, load physics raycaster behaviour
    /// </summary>
    public static class ModApiLoader
    {
        // Written, 11.07.2022

        internal static bool initialized;

        #region IEnumerators

        internal static IEnumerator loadModApi()
        {
            // Written, 11.07.2022

            while (ModClient.getPOV == null)
            {
                yield return null;
            }

            ModClient.getPartManager.load();
            ModClient.getBoltManager.load();

            if (ModClient.devMode)
            {
                addDevMode();
            }

            initialized = true;
            ModConsole.Print($"[ModApiLoader] modapi v{ModApi.VersionInfo.version}: Loaded");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads modapi.
        /// </summary>
        internal static void injectModApi()
        {
            // Written, 11.09.2023

            bool loaded = ModClient.loaded;
            Debug.Log("MODAPI_MAIN: Load" + (loaded ? "ed" : "ing"));
            if (!loaded)
            {
                initialized = false;
                ModClient.setModApiGo(new GameObject("Mod API Loader"));
                ModClient.levelManager = ModClient.modapiGo.AddComponent<LevelManager>();
                GameObject.DontDestroyOnLoad(ModClient.modapiGo);

                ConsoleCommand.Add(new ConsoleCommands());
            }
        }

        internal static void addDevMode()
        {
            // Written, 25.08.2022

            if (ModClient.devModeBehaviour == null)
            {
                ModClient.devModeBehaviour = ModClient.modapiGo.AddComponent<DevMode>();
            }
        }   

        #endregion
    }
}
