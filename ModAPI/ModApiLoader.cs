using HutongGames.PlayMaker;
using MSCLoader;
using System;
using System.Collections;
using System.IO;
using System.Linq;

using TommoJProductions.ModApi.Attachable;
using TommoJProductions.ModApi.Attachable.CallBacks;
using TommoJProductions.ModApi.PlaymakerExtentions.Callbacks;
using UnityEngine;
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
        private static ModApiBehaviour m_modapiBehaviour;
        private static FsmState m_activateGameState;
        private static FsmStateActionCallback m_actionCallback;
        private static bool m_activateGameStateInjected = false;

        /// <summary>
        /// Represents the gameobject that holds the dev mode behaviour. gameobject is used to detect if game has been re-loaded/changed. used to inject mod api related stuff.
        /// </summary>
        public static GameObject modapiGo { get; private set; } = null;
        /// <summary>
        /// represents if the user is playing with a pirtated game copy (not through steam).
        /// </summary>
        public static bool piratedGameCopy { get; private set; } = false;

        // part related stuff
        private static Action m_partLeaveAction;

        internal static bool pickedPartSet = false;
        internal static bool inherentyPickedPartsSet = false;
        /// <summary>
        /// Reps all parts that were set in <see cref="partCheckFunction"/>. Reps all <see cref="Part"/> children in root <see cref="pickedPart"/>.
        /// </summary>
        internal static Part[] inherentlyPickedParts;
        /// <summary>
        /// Reps the root picked part. the part that the player is currently holding.
        /// </summary>
        internal static Part pickedPart;
        /// <summary>
        /// Reps the picked object. the object that the player is currently holding.
        /// </summary>
        internal static GameObject pickedObject;

        // bolt related stuff
        /// <summary>
        /// Represents the current detected bolt gameobject. only works in tool mode.
        /// </summary>
        internal static FsmGameObject bolt;
        /// <summary>
        /// represents the current bolt callback that the player is looking at.
        /// </summary>
        internal static BoltCallback currentCallback;
        /// <summary>
        /// represents the set bolt callback that the player is looking at.
        /// </summary>
        internal static BoltCallback lookingAtCallback;

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
                yield return null;
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

        internal static void addDevMode() 
        {
            // Written, 25.08.2022

            devModeBehaviour = modapiGo.AddComponent<DevMode>();
        }
        internal static void loadModApi()
        {
            // Written, 11.07.2022

            if (!m_activateGameStateInjected)
            {
                if (!ModLoader.CheckSteam()) // pirate detection.
                {
                    piratedGameCopy = true;
                    string error = "ModAPI: merirosvo. please report to mod developer!";
                    ModConsole.Error(error);
                    throw new Exception(error);
                }

                if (!Camera.main) // game not set up yet.
                {
                    m_activateGameStateInjected = true;
                    m_activateGameState = GameObject.Find("Setup Game").GetPlayMakerState("Activate game");
                    m_actionCallback = m_activateGameState.appendNewAction(setUpModApi);
                    Print("modapi: Injected");
                }
                else
                    setUpModApi();
            }
        }
        private static void setUpModApi()
        {
            // Written, 11.07.2022

            m_activateGameStateInjected = false;

            if (!modApiSetUp)
            {
                modapiGo = new GameObject("Mod API Loader");
                GameObject.DontDestroyOnLoad(modapiGo);
                m_modapiBehaviour = modapiGo.AddComponent<ModApiBehaviour>();
                ConsoleCommand.Add(new ConsoleCommands());
                loadedParts.Clear();
                loadedBolts.Clear();

                setUpPart();
                setUpBolt();
                if (devMode)
                {
                    addDevMode();
                }
                Print($"modapi v{VersionInfo.version}: Loaded");

                if (m_actionCallback != null)
                {
                    int index = Array.IndexOf(m_activateGameState.Actions, m_actionCallback);
                    if (index == -1)
                    {
                        Print("modapi.setup: action callback was not null but could not find index.");
                        return;
                    }
                    m_activateGameState.RemoveAction(index);
                    Print("modapi: Cleaned up");
                }
            }
            else
            {
                ModConsole.Error("[ModAPI].[setUpModApi] - tried setting up modapi but its already setup!");
            }
        }
        private static void setUpPart()
        {
            // Written, 02.07.2022

            getHandPickUpFsm.GetState("Part picked").insertNewAction(partPickedUp, 5);
            getHandPickUpFsm.GetState("Drop part").prependNewAction(partDropped);
            getHandPickUpFsm.GetState("Throw part").prependNewAction(partThrown);

            ModConsole.Print("[ModApiLoader] - part set up");
        }
        private static void setUpBolt()
        {
            // Written, 02.07.2022

            tryLoadBoltAssets();
            injectBoltChecks();

            ModConsole.Print("[ModApiLoader] - bolt set up");
        }

        private static void partPickedUp()
        {
            // Written, 11.06.2022

            m_modapiBehaviour.StartCoroutine(partCheckFunction());
        }
        private static void partDropped()
        {
            // Written, 11.06.2022

            if (pickedPart)
                m_partLeaveAction = pickedPart.invokeDroppedEvent;
            else
                m_partLeaveAction = null;
            objectLeaveHand(m_partLeaveAction, invokeDropEvent);
        }
        private static void partThrown()
        {
            // Written, 11.06.2022

            if (pickedPart)
                m_partLeaveAction = pickedPart.invokeThrownEvent;
            else
                m_partLeaveAction = null;
            objectLeaveHand(m_partLeaveAction, invokeThrowEvent);
        }

        private static void objectLeaveHand(Action partLeaveEvent, Action<GameObject> objectLeaveEvent) 
        {
            // Written, 14.06.2022

            if (pickedPartSet)
            {
                partLeaveEvent?.Invoke();
                pickedPart.pickedUp = false;
                pickedPart = null;
                pickedPartSet = false;
            }
            if (inherentyPickedPartsSet)
            {
                inherentlyPickedPartReset();
            }
            objectLeaveEvent.Invoke(pickedObject);
            pickedObject = null;
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
        
        private static void injectBoltChecks() 
        {
            // Written, 08.10.22

            injectBoltCheckToolMode();
            injectBoltCheckHandMode();
        }
        private static void injectBoltCheckHandMode()
        {
            // Written, 08.10.2022

            PlayMakerFSM pickUp = ModClient.getHandPickUpFsm;

            pickUp.GetState("Look for object").appendNewAction(handModeBoltCheck, CallbackTypeEnum.onUpdate, true);
        }
        private static void injectBoltCheckToolMode()
        {
            // Written, 25.08.2022

            GameObject toolLogic = getFPS.transform.FindChild("2Spanner/Raycast").gameObject;

            PlayMakerFSM raycast = toolLogic.GetPlayMaker("Raycast");
            bolt = raycast.FsmVariables.FindFsmGameObject("Bolt");

            PlayMakerFSM check = toolLogic.GetPlayMaker("Check");
            check.GetState("Check bolt Name").appendNewAction(toolModeBoltCheck);

            GameObject selectItem = getFPS.transform.FindChild("SelectItem").gameObject;
            PlayMakerFSM selection = selectItem.GetPlayMaker("Selection");
            selection.GetState("Reset tool").appendNewAction(resetBolt);
        }
        /// <summary>
        /// trys to load bolt assets.
        /// </summary>
        /// <returns>returns whether or not the bolt assets were loaded.</returns>
        private static void tryLoadBoltAssets()
        {
            // Written, 02.07.2022

            if (!boltAssetsLoaded)
            {
                AssetBundle ab = AssetBundle.CreateFromMemoryImmediate(Properties.Resources.modapi);
                nutPrefab = ab.LoadAsset("nut.prefab") as GameObject;
                shortBoltPrefab = ab.LoadAsset("short bolt.prefab") as GameObject;
                longBoltPrefab = ab.LoadAsset("long bolt.prefab") as GameObject;
                screwPrefab = ab.LoadAsset("screw.prefab") as GameObject;
                ab.Unload(false);

                Print("[ModApiLoader] bolt assets loaded");
                boltAssetsLoaded = true;
            }
            else
            {
                Print("[ModApiLoader] bolt assets already loaded. :)");
            }

            Print($"nut         prefab: {nutPrefab}");
            Print($"screw       prefab:  {screwPrefab}");
            Print($"short bolt  prefab: {shortBoltPrefab}");
            Print($"long bolt   prefab:  {longBoltPrefab}");
        }
        private static void boltCheck(GameObject raycastGameObject)
        {
            currentCallback = null;

            if (raycastGameObject)
            {
                currentCallback = raycastGameObject.GetComponent<BoltCallback>();
            }
            resetBolt();
            if (currentCallback)
            {
                currentCallback.onBoltEnter();
            }
            lookingAtCallback = currentCallback;
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Occurs when the <see cref="ModClient.getRaycastHitGameObject"/> gameobject reference changes. Handles <see cref="BoltCallback.onBoltEnter"/>/<see cref="BoltCallback.onBoltExit"/> calls. invoked by injected playmaker state.
        /// </summary>
        private static void handModeBoltCheck()
        {
            boltCheck(getRaycastHitGameObject.Value);
        }
        /// <summary>
        /// Occurs when the <see cref="bolt"/> gameobject reference changes. Handles <see cref="BoltCallback.onBoltEnter"/>/<see cref="BoltCallback.onBoltExit"/> calls. invoked by injected playmaker state.
        /// </summary>
        private static void toolModeBoltCheck()
        {
            // Written, 25.08.2022

            boltCheck(bolt.Value);
        }
        /// <summary>
        /// Occurs when the <see cref="bolt"/> gameobject reference changes or when the reset tool check takes place when player has changed to hand mode.. Handles <see cref="BoltCallback.onBoltExit"/> calls. invoked by injected playmaker state.
        /// </summary>
        private static void resetBolt()
        {
            // Written, 25.08.2022

            if (lookingAtCallback)
            {
                lookingAtCallback.onBoltExit();
                lookingAtCallback = null;
            }
        }

        #endregion
    }
}
