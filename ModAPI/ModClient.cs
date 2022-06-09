using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using TommoJProductions.ModApi.Attachable;
using UnityEngine;
using MSCLoader;

namespace TommoJProductions.ModApi
{
    /// <summary>
    /// Represents useful properties for interacting with My Summer Car and PlayMaker.
    /// </summary>
    public static class ModClient
    {
        // Written, 10.08.2018 | Modified 25.09.2021

        #region Constraints

#if DEBUG
        /// <summary>
        /// Represents that the complied runtime is fact debug configuration.
        /// </summary>
        public const bool IS_DEBUG_CONFIG = true;
#else
        /// <summary>
        /// Represents that the complied runtime is fact release configuration.
        /// </summary>
        public const bool IS_DEBUG_CONFIG = false;
#endif

#if x64
        /// <summary>
        /// Represents that the complied runtime is infact x64
        /// </summary>
        public const bool IS_X64 = true;
#else
        /// <summary>
        /// Represents that the complied runtime is infact x86
        /// </summary>
        public const bool IS_x64 = false;
#endif
        /// <summary>
        /// Represents the complied runtime version of the api.
        /// </summary>
        public const string version = "0.1.4.2";

        #endregion

        #region fields

        private static PlayMakerFSM pickUp;
        private static FsmGameObject pickedUpGameObject;
        private static FsmGameObject raycastHitGameObject;
        private static AudioSource _assembleAudio;
        private static AudioSource _disassembleAudio;
        private static FsmBool _guiDisassemble;
        private static FsmBool _guiAssemble;
        private static FsmBool _guiUse;
        private static FsmString _guiInteraction;
        private static FsmString _playerCurrentVehicle;

        #endregion

        #region Properties

        /// <summary>
        /// Represents the assemble assemble audio source.
        /// </summary>
        public static AudioSource assembleAudio
        {
            get
            {
                if (_assembleAudio == null)
                    _assembleAudio = GameObject.Find("MasterAudio/CarBuilding/assemble").GetComponent<AudioSource>();
                return _assembleAudio;
            }
        }
        /// <summary>
        /// Represents the disassemble audio source.
        /// </summary>
        public static AudioSource disassembleAudio
        {
            get
            {
                if (_disassembleAudio == null)
                    _disassembleAudio = GameObject.Find("MasterAudio/CarBuilding/disassemble").GetComponent<AudioSource>();
                return _disassembleAudio;
            }
        }
        /// <summary>
        /// Represents whether the gui disassemble icon is shown or not. (Circle with line through it.)
        /// </summary>
        public static bool guiDisassemble
        {
            get
            {
                if (_guiDisassemble == null)
                    _guiDisassemble = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIdisassemble");
                return _guiDisassemble.Value;
            }
            set
            {
                if (_guiDisassemble == null)
                    _guiDisassemble = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIdisassemble");
                _guiDisassemble.Value = value;
            }
        }
        /// <summary>
        /// Represents whether the gui assemble icon is shown or not. (Tick Symbol)
        /// </summary>
        public static bool guiAssemble
        {
            get
            {
                if (_guiAssemble == null)
                    _guiAssemble = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIassemble");
                return _guiAssemble.Value;
            }
            set
            {
                if (_guiAssemble == null)
                    _guiAssemble = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIassemble");
                _guiAssemble.Value = value;
            }
        }
        /// <summary>
        /// Represents whether or not the gui use icon is shown. (Hand Symbol)
        /// </summary>
        public static bool guiUse
        {
            get
            {
                if (_guiUse == null)
                    _guiUse = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIuse");
                return _guiUse.Value;
            }
            set
            {
                if (_guiUse == null)
                    _guiUse = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIuse");
                _guiUse.Value = value;
            }
        }
        /// <summary>
        /// Represents whether or not to display gui interaction text.
        /// </summary>
        public static string guiInteraction
        {
            get
            {
                if (_guiInteraction == null)
                    _guiInteraction = PlayMakerGlobals.Instance.Variables.FindFsmString("GUIinteraction").Value;
                return _guiInteraction.Value;
            }
            set
            {
                if (_guiInteraction == null)
                    _guiInteraction = PlayMakerGlobals.Instance.Variables.FindFsmString("GUIinteraction").Value;
                _guiInteraction.Value = value;
            }
        }
        /// <summary>
        /// Represents the player current vehicle state.
        /// </summary>
        public static string playerCurrentVehicle
        {
            get
            {
                if (_playerCurrentVehicle == null)
                    _playerCurrentVehicle = PlayMakerGlobals.Instance.Variables.FindFsmString("PlayerCurrentVehicle").Value;
                return _playerCurrentVehicle.Value;
            }
            set
            {
                if (_playerCurrentVehicle == null)
                    _playerCurrentVehicle = PlayMakerGlobals.Instance.Variables.FindFsmString("PlayerCurrentVehicle").Value;
                _playerCurrentVehicle.Value = value;
            }
        }
        /// <summary>
        /// Gets the current player mode, 
        /// </summary>
        public static PlayerModeEnum getMode
        {
            get
            {
                PlayerModeEnum pme;
                switch (playerCurrentVehicle)
                {
                    case "":
                        pme = PlayerModeEnum.OnFoot;
                        break;
                    case "Satsuma":
                        pme = PlayerModeEnum.InSatsuma;
                        break;
                    default:
                        pme = PlayerModeEnum.InOther;
                        break;
                }
                return pme;
            }
        }
        /// <summary>
        /// Represents all parts in the current instance of my summer car. (msc mods that use MODAPI.Attachable.Part
        /// will be listed here)
        /// </summary>
        public static List<Part> parts { get; } = new List<Part>();


        /// <summary>
        /// Gets the pickup playmakerfsm from the hand gameobject.
        /// </summary>
        public static PlayMakerFSM getHandPickUpFsm
        {
            get
            {
                if (pickUp == null)
                    pickUp = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/1Hand_Assemble/Hand").GetPlayMaker("PickUp");
                return pickUp;
            }
        }
        /// <summary>
        /// Gets the gameobject that the player is holding.
        /// </summary>
        public static FsmGameObject getPickedUpGameObject
        {
            get
            {
                if (pickedUpGameObject == null)
                    pickedUpGameObject = getHandPickUpFsm.FsmVariables.GetFsmGameObject("PickedObject");
                return pickedUpGameObject;
            }
        }
        /// <summary>
        /// Gets the gameobject that the player is looking at.
        /// </summary>
        public static FsmGameObject getRaycastHitGameObject
        {
            get
            {
                if (raycastHitGameObject == null)
                    raycastHitGameObject = getHandPickUpFsm.FsmVariables.GetFsmGameObject("RaycastHitObject");
                return raycastHitGameObject;
            }
        }
        /// <summary>
        /// Returns true if in hand mode. determines state by <see cref="getHandPickUpFsm"/>.Active.
        /// </summary>
        public static bool isInHandMode => getHandPickUpFsm.Active;

        #endregion

        #region Constructors

        static ModClient()
        {
            print("static constructor hit");
            ConsoleCommand.Add(new ConsoleCommands());
            parts.Clear();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Displays an interaction text and optional hand symbol. If no parameters are passed into the method (default parameters), turns off the interaction.
        /// </summary>
        /// <param name="inText">The text to display.</param>
        /// <param name="inGuiInteractSymbol">Whether or not to display a symbol.</param>
        public static void guiInteract(string inText = "", GuiInteractSymbolEnum inGuiInteractSymbol = GuiInteractSymbolEnum.Hand)
        {
            // Written, 30.09.2018 | Modified, 16.03.2019

            if (inText == "")
            {
                guiInteraction = inText;
                return;
            }
            changeInteractSymbol(inGuiInteractSymbol);
            guiInteraction = inText;
        }
        /// <summary>
        /// displays or removes a particular symbol from the interaction. if <see cref="GuiInteractSymbolEnum.None"/> is passed,
        /// sets all interactions to the passed boolean value.
        /// </summary>
        /// <param name="inGuiInteractSymbol">The gui symbol to change.</param>
        /// <param name="inValue">The value..</param>
        private static void changeInteractSymbol(GuiInteractSymbolEnum inGuiInteractSymbol)
        {
            // Written, 16.03.2019

            switch (inGuiInteractSymbol)
            {
                case GuiInteractSymbolEnum.Hand:
                    guiUse = true;
                    guiAssemble = false;
                    guiDisassemble = false;
                    break;
                case GuiInteractSymbolEnum.Disassemble:
                    guiUse = false;
                    guiAssemble = false;
                    guiDisassemble = true;
                    break;
                case GuiInteractSymbolEnum.Assemble:
                    guiUse = false;
                    guiAssemble = true;
                    guiDisassemble = false;
                    break;
                case GuiInteractSymbolEnum.None:
                    guiUse = false;
                    guiDisassemble = false;
                    guiAssemble = false;
                    break;
            }
        }
        /// <summary>
        /// Gets what the player is currently holding. returns null if player is holding nothing.
        /// </summary>
        public static GameObject getPlayerHolding() 
        {
            // Written, 08.05.2022

            return getPickedUpGameObject.Value;
        }
        /// <summary>
        /// Gets what the player is currently looking at. returns null if player is looking at nothing.
        /// </summary>
        public static GameObject getPlayerLooking()
        {
            // Written, 08.05.2022

            return getRaycastHitGameObject.Value;
        }
        /// <summary>
        /// Reps modapi print-to-console function
        /// </summary>
        public static void print(string format, params object[] args)
        {
            // Written, 09.10.2021

            ModConsole.Log(string.Format("<color=grey>[ModAPI] " + format + "</color>", args));
        }

        #endregion


        #region ExMethods

        /// <summary>
        /// returns whether the player is currently looking at an gameobject.
        /// </summary>
        /// <param name="gameObject">The gameobject to detect againist.</param>
        /// <param name="withinDistance">raycast within distance from main camera.</param>
        public static bool isPlayerLookingAt(this GameObject gameObject, float withinDistance = 1)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, withinDistance, 1 << gameObject.layer))
            {
                if (hit.collider?.gameObject == gameObject)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Checks if player is holding the GameObject, <paramref name="inGameObject"/>.
        /// </summary>
        /// <param name="inGameObject">The gameObject to check on.</param>
        public static bool isPlayerHolding(this GameObject inGameObject)
        {
            // Written, 02.10.2018

            if (inGameObject.isOnLayer(LayerMasksEnum.Wheel))
                return true;
            return false;
        }

        /// <summary>
        /// Checks if the gameobject is on the layer, <paramref name="inLayer"/>.
        /// </summary>
        /// <param name="inGameObject">The gameobject to check layer.</param>
        /// <param name="inLayer">The layer to check for.</param>
        public static bool isOnLayer(this GameObject inGameObject, LayerMasksEnum inLayer)
        {
            // Written, 02.10.2018

            if (inGameObject.layer == inLayer.layer())
                return true;
            return false;
        }
        /// <summary>
        /// Sends all children of game object to layer.
        /// </summary>
        /// <param name="inGameObject">The gameobject to get children from.</param>
        /// <param name="inLayer">The layer to send gameobject children to.</param>
        public static void sendChildrenToLayer(this GameObject inGameObject, LayerMasksEnum inLayer)
        {
            // Written, 27.09.2018

            Transform transform = inGameObject.transform;

            if (transform != null)
                for (int i = 0; i < inGameObject.transform.childCount; i++)
                {
                    inGameObject.transform.GetChild(i).gameObject.sendToLayer(inLayer);
                }
        }
        /// <summary>
        /// Sends a gameobject to the desired layer.
        /// </summary>
        /// <param name="inGameObject">The gameObject.</param>
        /// <param name="inLayer">The Layer.</param>
        /// <param name="inSendAllChildren">[Optional] sends all children to the layer.</param>
        public static void sendToLayer(this GameObject inGameObject, LayerMasksEnum inLayer, bool inSendAllChildren = false)
        {
            // Written, 02.10.2018

            inGameObject.layer = layer(inLayer);
            if (inSendAllChildren)
                inGameObject.sendChildrenToLayer(inLayer);
        }
        /// <summary>
        /// Returns the layer index number.
        /// </summary>
        /// <param name="inLayer">The layer to get index.</param>
        public static int layer(this LayerMasksEnum inLayer)
        {
            // Written, 02.10.2018

            return (int)inLayer;
        }
        /// <summary>
        /// Returns the name of the layer.
        /// </summary>
        /// <param name="inLayer">The layer to get.</param>
        public static string name(this LayerMasksEnum inLayer)
        {
            // Written, 02.10.2018

            string layerName;
            switch (inLayer)
            {
                case LayerMasksEnum.Bolts:
                    layerName = "Bolts";
                    break;
                case LayerMasksEnum.Cloud:
                    layerName = "Cloud";
                    break;
                case LayerMasksEnum.Collider:
                    layerName = "Collider";
                    break;
                case LayerMasksEnum.Collider2:
                    layerName = "Collider2";
                    break;
                case LayerMasksEnum.Dashboard:
                    layerName = "Dashboard";
                    break;
                case LayerMasksEnum.Datsun:
                    layerName = "Datsun";
                    break;
                case LayerMasksEnum.Default:
                    layerName = "Default";
                    break;
                case LayerMasksEnum.DontCollide:
                    layerName = "DontCollide";
                    break;
                case LayerMasksEnum.Forest:
                    layerName = "Forest";
                    break;
                case LayerMasksEnum.Glass:
                    layerName = "Glass";
                    break;
                case LayerMasksEnum.GUI:
                    layerName = "GUI";
                    break;
                case LayerMasksEnum.HingedObjects:
                    layerName = "HingedObjects";
                    break;
                case LayerMasksEnum.IgnoreRaycast:
                    layerName = "IgnoreRaycast";
                    break;
                case LayerMasksEnum.Lifter:
                    layerName = "Lifter";
                    break;
                case LayerMasksEnum.NoRain:
                    layerName = "NoRain";
                    break;
                case LayerMasksEnum.Parts:
                    layerName = "Parts";
                    break;
                case LayerMasksEnum.Player:
                    layerName = "Player";
                    break;
                case LayerMasksEnum.PlayerOnlyColl:
                    layerName = "PlayerOnlyColl";
                    break;
                case LayerMasksEnum.Road:
                    layerName = "Road";
                    break;
                case LayerMasksEnum.Terrain:
                    layerName = "Terrain";
                    break;
                case LayerMasksEnum.Tools:
                    layerName = "Tools";
                    break;
                case LayerMasksEnum.TransparentFX:
                    layerName = "TransparentFX";
                    break;
                case LayerMasksEnum.TriggerOnly:
                    layerName = "TriggerOnly";
                    break;
                case LayerMasksEnum.UI:
                    layerName = "UI";
                    break;
                case LayerMasksEnum.Water:
                    layerName = "Water";
                    break;
                case LayerMasksEnum.Wheel:
                    layerName = "Wheel";
                    break;
                default:
                    layerName = "Blank";
                    break;
            }
            return layerName;
        }
        /// <summary>
        /// Creates and returns a new rigidbody on the gameobject and assigns parameters listed.
        /// </summary>
        /// <param name="gameObject">The gameobject to create a rigidbody on.</param>
        /// <param name="rigidbodyConstraints">Create rigidbody with these constraints.</param>
        /// <param name="mass">The mass of the rb.</param>
        /// <param name="drag">The drag of the rb</param>
        /// <param name="angularDrag">The angular drag of the rb.</param>
        /// <param name="isKinematic">is this rigidbody kinmatic?</param>
        /// <param name="useGravity">is this rigidbody affected by gravity?</param>
        public static Rigidbody createRigidbody(this GameObject gameObject,
         RigidbodyConstraints rigidbodyConstraints = RigidbodyConstraints.None, float mass = 1, float drag = 0, float angularDrag = 0.05f, bool isKinematic = false, bool useGravity = true)
        {
            // Written, 10.09.2021

            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.constraints = rigidbodyConstraints;
            rb.mass = mass;
            rb.drag = drag;
            rb.angularDrag = angularDrag;
            rb.isKinematic = isKinematic;
            rb.useGravity = useGravity;
            return rb;
        }
        /// <summary>
        /// Fixes a transform to another transform.
        /// </summary>
        /// <param name="transform">The transform to fix</param>
        /// <param name="parent">The parent transform to fix <paramref name="transform"/></param>
        /// <param name="onFixedToParent">action to call when fixed to parent.</param>
        public static IEnumerator fixToParent(this Transform transform, Transform parent, Action onFixedToParent = null)
        {
            while (transform.parent != parent)
            {
                yield return new WaitForFixedUpdate();
                transform.parent = parent;
            }
            onFixedToParent?.Invoke();
            yield break;
        }
        /// <summary>
        /// Sets its position and rotation.
        /// </summary>
        /// <param name="transform">The transform to fix</param>
        /// <param name="pos">The pos to set</param>
        /// <param name="eulerAngles">the rot to set</param>
        /// <param name="onFixedTransform">action to call when fixed transform.</param>
        public static IEnumerator fixTransform(this Transform transform, Vector3 pos, Vector3 eulerAngles, Action onFixedTransform = null)
        {
            while (transform.localPosition != pos && transform.localEulerAngles != eulerAngles)
            {
                yield return new WaitForFixedUpdate();
                transform.localPosition = pos;
                transform.localEulerAngles = eulerAngles;
            }
            onFixedTransform?.Invoke();
            yield break;
        }

        public enum injectEnum
        {
            append,
            prepend,
            insert,
            replace
        }
        public static void addNewGlobalTransition(this PlayMakerFSM fsm, FsmEvent _event, string stateName)
        {
            FsmTransition[] fsmGlobalTransitions = fsm.FsmGlobalTransitions;
            List<FsmTransition> temp = new List<FsmTransition>();
            foreach (FsmTransition t in fsmGlobalTransitions)
            {
                temp.Add(t);
            }
            temp.Add(new FsmTransition
            {
                FsmEvent = _event,
                ToState = stateName
            });
            fsm.Fsm.GlobalTransitions = temp.ToArray();
        }
        public static void addNewTransitionToState(this GameObject go, string stateName, string eventName, string toStateName)
        {

            FsmState state = go.GetPlayMakerState(stateName);
            List<FsmTransition> temp = new List<FsmTransition>();
            foreach (FsmTransition t in state.Transitions)
            {
                temp.Add(t);
            }
            temp.Add(new FsmTransition
            {
                FsmEvent = state.Fsm.GetEvent(eventName),
                ToState = toStateName
            });
            state.Transitions = temp.ToArray();
        }
        public static void appendNewAction(this FsmState state, Action action)
        {
            List<FsmStateAction> temp = new List<FsmStateAction>();
            foreach (FsmStateAction v in state.Actions)
            {
                temp.Add(v);
            }
            FsmStateActionCallback callback = new FsmStateActionCallback(action);
            temp.Add(callback);
            state.Actions = temp.ToArray();
        }
        public static void prependNewAction(this FsmState state, Action action)
        {
            List<FsmStateAction> temp = new List<FsmStateAction>();
            FsmStateActionCallback callback = new FsmStateActionCallback(action);
            temp.Add(callback);
            foreach (FsmStateAction v in state.Actions)
            {
                temp.Add(v);
            }
            state.Actions = temp.ToArray();
        }
        public static void insertNewAction(this FsmState state, Action action, int index)
        {
            List<FsmStateAction> temp = new List<FsmStateAction>();
            FsmStateActionCallback callback = new FsmStateActionCallback(action);
            foreach (FsmStateAction v in state.Actions)
            {
                temp.Add(v);
            }
            temp.Insert(index, callback);
            state.Actions = temp.ToArray();
        }
        public static void replaceAction(this FsmState state, Action action, int index)
        {
            // Written, 13.04.2022

            List<FsmStateAction> temp = new List<FsmStateAction>();
            FsmStateActionCallback callback = new FsmStateActionCallback(action);
            for (int i = 0; i < state.Actions.Length; i++)
            {
                if (i == index)
                    temp.Add(callback);
                else
                    temp.Add(state.Actions[i]);
            }
            state.Actions = temp.ToArray();
        }
        public static void injectAction(this PlayMakerFSM playMakerFSM, string stateName, injectEnum injectType, Action callback, int index = 0)
        {
            injectAction(playMakerFSM.gameObject, playMakerFSM.FsmName, stateName, injectType, callback, index);
        }
        public static void injectAction(this GameObject go, string fsmName, string stateName, injectEnum injectType, Action callback, int index = 0)
        {
            PlayMakerFSM fsm = go.GetPlayMaker(fsmName);
            FsmState state = go.GetPlayMakerState(stateName);
            switch (injectType)
            {
                case injectEnum.append:
                    state.appendNewAction(callback);
                    break;
                case injectEnum.prepend:
                    state.prependNewAction(callback);
                    break;
                case injectEnum.insert:
                    state.insertNewAction(callback, index);
                    break;
                case injectEnum.replace:
                    state.replaceAction(callback, index);
                    break;
            }
           print($"Inject Action | {injectType}ing {callback.Method.Name} to {go.name}.{fsmName}.{stateName}" + (injectType == injectEnum.insert ? $" at index:{index}" : ""));
        }

        public static FsmFloat round(this FsmFloat fsmFloat, int decimalPlace = 0)
        {
            return fsmFloat.Value.round(decimalPlace);
        }
        public static float round(this float _float, int decimalPlace = 0)
        {
            // Written, 15.01.2022

            return (float)Math.Round(_float, decimalPlace);
        }
        public static float mapValue(this float mainValue, float inValueMin, float inValueMax, float outValueMin, float outValueMax)
        {
            return (mainValue - inValueMin) * (outValueMax - outValueMin) / (inValueMax - inValueMin) + outValueMin;
        }

        #endregion
    }
}
