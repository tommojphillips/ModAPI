using HutongGames.PlayMaker;
using MSCLoader;
using System;
using System.Collections;
using TommoJProductions.ModApi.Attachable;
using TommoJProductions.ModApi.PlaymakerExtentions.Callbacks;
using UnityEngine;
using UnityEngine.EventSystems;
using static MSCLoader.ModConsole;
using static TommoJProductions.ModApi.ModClient;

namespace TommoJProductions.ModApi
{
    /// <summary>
    /// Represents behaviour to load modapi stuff. eg bolt assets, load physics raycaster behaviour
    /// </summary>
    public static class ModApiLoader
    {
        // Written, 11.07.2022

        // loader stuff
        /// <summary>
        /// Represents the gameobject that holds the modapiloader behaviour
        /// </summary>
        public static GameObject modapiGo { get; private set; } = null;
        private static FsmState activateGameState;
        private static FsmStateActionCallback actionCallback;

        // part stuff
        private static bool partCheckSet = false;
        private static Part[] setParts;
        private static Part pickedPart;

        // bolt stuff
        internal static PhysicsRaycaster raycaster = null;

        #region IEnumerators

        private static IEnumerator partCheckFunction()
        {
            // Written, 09.06.2022

            yield return null;
            pickedPart = getPickedUpGameObject.Value?.GetComponent<Part>();
            if (pickedPart)
            {
                pickedPart.invokePickedUpEvent();
                setParts = pickedPart.gameObject.getBehavioursInChildren<Part>();
                if (setParts != null && setParts.Length > 0)
                {
                    partCheckSet = true;
                    foreach (Part p in setParts)
                    {
                        p.gameObject.sendToLayer(LayerMasksEnum.Wheel);
                    }
                }
            }
        }

        #endregion

        #region Methods

        internal static void setUpLoader()
        {
            // Written, 11.07.2022

            modapiGo = new GameObject("Mod API Loader");
            Print("modapi: Setup");
        }
        internal static void loadModApi()
        {
            // Written, 11.07.2022

            activateGameState = GameObject.Find("Setup Game").GetPlayMakerState("Activate game");
            actionCallback = activateGameState.appendNewAction(setUpModApi);
            ConsoleCommand.Add(new ConsoleCommands());
            parts.Clear();
            bolts.Clear();
            Print("modapi: Injected");
        }
        private static void setUpModApi()
        {
            // Written, 11.07.2022

            setUpPart();
            setUpBolt();
            if (devMode)
            {
                raycaster.StartCoroutine(devModeFunc());
            }
            Print("modapi: Loaded");

            int index = Array.IndexOf(activateGameState.Actions, actionCallback);
            activateGameState.RemoveAction(index);
        }
        private static void setUpPart() 
        {
            // Written, 02.07.2022

            if (getHandPickUpFsm != null)
            {
                getHandPickUpFsm.GetState("Part picked").insertNewAction(partPickedUp, 5);
                getHandPickUpFsm.GetState("Drop part").prependNewAction(partDropped);
                getHandPickUpFsm.GetState("Throw part").prependNewAction(partThrown);
            }
        }
        private static void partPickedUp()
        {
            // Written, 11.06.2022

            getHandPickUpFsm?.StartCoroutine(partCheckFunction());
        }
        private static void partDropped()
        {
            // Written, 11.06.2022

            if (partCheckSet)
            {
                pickedPart.invokeDroppedEvent();
                partCheckReset();
            }
        }
        private static void partThrown()
        {
            // Written, 11.06.2022

            if (partCheckSet)
            {
                pickedPart.invokeThrownEvent();
                partCheckReset();
            }
        }
        private static void partCheckReset()
        {
            // Written, 09.06.2022

            partCheckSet = false;
            foreach (Part p in setParts)
            {
                p.makePartPickable(!p.installed);
            }
            setParts = null;
            pickedPart = null;
        }
        private static void setUpBolt()
        {
            // Written, 02.07.2022

            Bolt.tryLoadBoltAssets();

            if (Camera.main)
            {
                if (!raycaster)
                {
                    raycaster = Camera.main.gameObject.AddComponent<PhysicsRaycaster>();
                    raycaster.eventMask = getMask(LayerMasksEnum.Bolts);                    
                    Print("[ModApi.Loader] physics raycaster set up on main camera");
                }
                else
                    Print("[ModApi.Loader] physics raycaster already setup. :)");
            }
            else
            {
                Print("[ModApi.Loader] main camera was null. could not set up physics raycaster. 'ModApi.Bolts' wont work.");
            }
        }

        #endregion
    }
}
