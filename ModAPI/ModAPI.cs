using UnityEngine;
using HutongGames.PlayMaker;

namespace ModAPI
{
    /// <summary>
    /// Represents useful properties for playmaker.
    /// </summary>
    public static class ModAPI
    {
        // Written, 10.08.2018

        #region Properties
        
        /// <summary>
        /// Represents the assemble assemble audio source.
        /// </summary>
        public static AudioSource assembleAudio
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the disassemble audio source.
        /// </summary>
        public static AudioSource disassembleAudio
        {
            get;
            private set;
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

        #endregion

        #region Methods

        /// <summary>
        /// Sets, <see cref="assembleAudio"/> and <see cref="aisassembleAudio"/> to the audio.
        /// </summary>
        public static void intializeAssembleSounds()
        {
            // Written, 10.08.2018

            GameObject go = GameObject.Find("MasterAudio/CarBuilding/assemble");
            assembleAudio = go.GetComponent<AudioSource>();
            go = GameObject.Find("MasterAudio/CarBuilding/disassemble");
            disassembleAudio = go.GetComponent<AudioSource>();
        }
        /// <summary>
        /// Displays an interaction text and optional hand symbol. If no parameters are passed into the method (default parameters), turns off the interaction.
        /// </summary>
        /// <param name="inText">The text to display.</param>
        /// <param name="showHand">Whether or not to display the hand symbol.</param>
        public static void guiInteract(string inText = "", bool showHand = true)
        {
            // Written, 30.09.2018

            if (inText == "")
            {
                guiUse = false;
                guiInteraction = inText;
                return;
            }
            guiUse = showHand;
            guiInteraction = inText;

        }

        #endregion
    }
}
