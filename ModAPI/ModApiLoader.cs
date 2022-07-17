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
        /// <summary>
        /// Reps all parts that were set in <see cref="partCheckFunction"/>. Reps all <see cref="Part"/> children in root <see cref="pickedPart"/>.
        /// </summary>
        private static Part[] setParts;
        /// <summary>
        /// Reps the root picked part. the part that the player is currently holding.
        /// </summary>
        private static Part pickedPart;
        /// <summary>
        /// Reps the picked object. the object that the player is currently holding.
        /// </summary>
        private static GameObject pickedObject;

        // bolt stuff
        internal static PhysicsRaycaster raycaster = null;

        #region IEnumerators

        private static IEnumerator partCheckFunction()
        {
            // Written, 09.06.2022

            yield return null;
            pickedObject = getPickedUpGameObject.Value;
            if (pickedObject)
            {
                pickedPart = pickedObject.GetComponent<Part>();
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
                invokePickUpEvent(pickedObject);
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

            if (!Camera.main)
            {
                activateGameState = GameObject.Find("Setup Game").GetPlayMakerState("Activate game");
                actionCallback = activateGameState.appendNewAction(setUpModApi);
                Print("modapi: Injected");
            }
            else
                setUpModApi();
        }
        private static void setUpModApi()
        {
            // Written, 11.07.2022

            ConsoleCommand.Add(new ConsoleCommands());
            parts.Clear();
            bolts.Clear();

            setUpPart();
            setUpBolt();
            if (devMode)
            {
                raycaster.StartCoroutine(devModeFunc());
            }
            Print("modapi: Loaded");

            int index = Array.IndexOf(activateGameState.Actions, actionCallback);
            if (index == -1)
                return;
            activateGameState.RemoveAction(index);
            Print("modapi: Cleaned up");
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
            invokeDropEvent(pickedObject);
            resetObject();
        }
        private static void partThrown()
        {
            // Written, 11.06.2022

            if (partCheckSet)
            {
                pickedPart.invokeThrownEvent();
                partCheckReset();
            }
            invokeThrowEvent(pickedObject);
            resetObject();
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
        private static void resetObject() 
        {
            pickedObject = null;
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
