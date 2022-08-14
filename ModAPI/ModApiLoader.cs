using HutongGames.PlayMaker;
using MSCLoader;
using System;
using System.Collections;
using System.Linq;

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
        private static bool pickedPartSet = false;
        private static bool inherentyPickedPartsSet = false;
        private static Action partLeaveAction;
        /// <summary>
        /// Reps all parts that were set in <see cref="partCheckFunction"/>. Reps all <see cref="Part"/> children in root <see cref="pickedPart"/>.
        /// </summary>
        private static Part[] inherentlyPickedParts;
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
                    pickedPartSet = true;
                    pickedPart.pickedUp = true;
                    pickedPart.invokePickedUpEvent();                    
                }
                
                inherentlyPickedParts = pickedObject.getBehavioursInChildren<Part>();

                if (inherentlyPickedParts != null && inherentlyPickedParts.Length > 0)
                {
                    inherentyPickedPartsSet = true;
                    foreach (Part p in inherentlyPickedParts)
                    {
                        p.gameObject.sendToLayer(LayerMasksEnum.Wheel);
                        p.inherentlyPickedUp = true;
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

            if (!Camera.main) // game not set up yet.
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
            loadedParts.Clear();
            loadedBolts.Clear();

            setUpPart();
            setUpBolt();
            if (devMode)
            {
                devModeBehaviour = modapiGo.AddComponent<DevMode>();
            }
            Print($"modapi v{VersionInfo.version}: Loaded");

            if (actionCallback != null)
            {
                int index = Array.IndexOf(activateGameState.Actions, actionCallback);
                if (index == -1)
                    return;
                activateGameState.RemoveAction(index);
                Print("modapi: Cleaned up");
            }
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


            if (pickedPart)
                partLeaveAction = pickedPart.invokeDroppedEvent;
            else
                partLeaveAction = null;
            objectLeaveHand(partLeaveAction, invokeDropEvent);
        }
        private static void partThrown()
        {
            // Written, 11.06.2022

            if (pickedPart)
                partLeaveAction = pickedPart.invokeThrownEvent;
            else
                partLeaveAction = null;
            objectLeaveHand(partLeaveAction, invokeThrowEvent);
        }
        private static void objectLeaveHand(Action partLeaveEvent, Action<GameObject> objectLeaveEvent) 
        {
            // Written, 14.06.2022

            if (pickedPartSet)
            {
                partLeaveEvent?.Invoke();
                pickedPartReset();
            }
            if (inherentyPickedPartsSet)
            {
                inherentlyPickedPartReset();
            }
            objectLeaveEvent(pickedObject);
            resetObject();
        }
        private static void inherentlyPickedPartReset()
        {
            // Written, 09.06.2022

            foreach (Part p in inherentlyPickedParts)
            {
                p.inherentlyPickedUp = false;
                p.makePartPickable(!p.installed);
            }
            inherentlyPickedParts = null;
            inherentyPickedPartsSet = false;
        }
        private static void pickedPartReset()
        {
            // Written, 09.06.2022

            pickedPart.pickedUp = false;
            pickedPart = null;
            pickedPartSet = false;
        }
        private static void resetObject() 
        {
            pickedObject = null;
        }
        private static void setUpBolt()
        {
            // Written, 02.07.2022

            Bolt.tryLoadBoltAssets();

            if (ModClient.getPlayerCamera)
            {
                if (!raycaster)
                {
                    raycaster = ModClient.getPOV.gameObject.AddComponent<PhysicsRaycaster>();
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
