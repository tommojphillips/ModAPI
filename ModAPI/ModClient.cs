﻿using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;
using ModApi.Attachable;

namespace ModApi
{
    /// <summary>
    /// Represents useful properties for interacting with My Summer Car and PlayMaker.
    /// </summary>
    public static class ModClient
    {
        // Written, 10.08.2018

        #region Properties

        /// <summary>
        /// Represents the version of the api.
        /// </summary>
        public static string version = "0.1.2.0";
        /// <summary>
        /// Represents the assemble assemble audio source.
        /// </summary>
        public static AudioSource assembleAudio
        {
            get
            {
                return GameObject.Find("MasterAudio/CarBuilding/assemble").GetComponent<AudioSource>();
            }
        }
        /// <summary>
        /// Represents the disassemble audio source.
        /// </summary>
        public static AudioSource disassembleAudio
        {
            get
            {
                return GameObject.Find("MasterAudio/CarBuilding/disassemble").GetComponent<AudioSource>();
            }
        }
        /// <summary>
        /// Represents whether the gui disassemble icon is shown or not. (Circle with line through it.)
        /// </summary>
        public static bool guiDisassemble
        {
            get
            {
                return PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIdisassemble").Value;
            }
            set
            {
                PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIdisassemble").Value = value;
            }
        }
        /// <summary>
        /// Represents whether the gui assemble icon is shown or not. (Tick Symbol)
        /// </summary>
        public static bool guiAssemble
        {
            get
            {
                return PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIassemble").Value;
            }
            set
            {
                PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIassemble").Value = value;
            }
        }
        /// <summary>
        /// Represents whether or not the gui use icon is shown. (Hand Symbol)
        /// </summary>
        public static bool guiUse
        {
            get
            {
                return PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIuse").Value;
            }
            set
            {
                PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIuse").Value = value;
            }
        }
        /// <summary>
        /// Represents whether or not to display gui interaction text.
        /// </summary>
        public static string guiInteraction
        {
            get
            {
                return PlayMakerGlobals.Instance.Variables.FindFsmString("GUIinteraction").Value;
            }
            set
            {
                PlayMakerGlobals.Instance.Variables.FindFsmString("GUIinteraction").Value = value;
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
                string currentVechicle = FsmVariables.GlobalVariables.FindFsmString("PlayerCurrentVehicle").Value;
                switch (currentVechicle)
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
        public static List<Part> activeParts { get; } = new List<Part>();

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
                changeInteractSymbol(GuiInteractSymbolEnum.None, false);
                guiInteraction = inText;
                return;
            }
            changeInteractSymbol(inGuiInteractSymbol, true);
            guiInteraction = inText;
        }
        /// <summary>
        /// displays or removes a particular symbol from the interaction. if <see cref="GuiInteractSymbolEnum.None"/> is passed,
        /// sets all interactions to the passed boolean value.
        /// </summary>
        /// <param name="inGuiInteractSymbol">The gui symbol to change.</param>
        /// <param name="inValue">The value..</param>
        private static void changeInteractSymbol(GuiInteractSymbolEnum inGuiInteractSymbol, bool inValue)
        {
            // Written, 16.03.2019

            switch (inGuiInteractSymbol)
            {
                case GuiInteractSymbolEnum.Hand:
                    guiUse = inValue;
                    break;
                case GuiInteractSymbolEnum.Disassemble:
                    guiDisassemble = inValue;
                    break;
                case GuiInteractSymbolEnum.Assemble:
                    guiAssemble = inValue;
                    break;
                case GuiInteractSymbolEnum.None:
                    guiUse = inValue;
                    guiDisassemble = inValue;
                    guiAssemble = inValue;
                    break;
            }
        }

        #endregion

        #region ExMethods

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

        #endregion
    }
}
